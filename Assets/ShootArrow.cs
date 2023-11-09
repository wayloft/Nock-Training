using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootArrow : MonoBehaviour
{
    [SerializeField] Transform arrowPlacement;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] LineRenderer lineR;

    [SerializeField] GameObject inactiveShotUI;
    [SerializeField] GameObject activeShotUI;

    [SerializeField] Transform rightHandBall;

    [SerializeField] Transform redBallThing;
    [SerializeField] Transform handLookAtSphere;
    [SerializeField] AnimationCurve bowPullbackPowerCurve;

    [SerializeField] float destroyTime = 6f;

    WaitForSeconds waitForDestroyTime;

    XRIDefaultInputActions defaultInputActions;
    
    InputAction rightHandPosition;
    InputAction rightHandGrab;

    float forceAmount = 5f;

    bool notTriggered = true;

    private void Awake()
    {
        defaultInputActions = new XRIDefaultInputActions();
        rightHandGrab = defaultInputActions.XRIRightHandInteraction.Activate;
        rightHandPosition = defaultInputActions.XRIRightHand.Position;
    }

    private void OnEnable()
    {
        rightHandGrab.Enable();
        rightHandPosition.Enable();
    }

    private void OnDisable()
    {
        rightHandGrab.Disable();
        rightHandPosition.Disable();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Hand" && notTriggered)
        {
            if(rightHandGrab.ReadValue<float>() > .9f)
            {
                StartCoroutine(ReadyToShoot());
            }
        }
    }

    void Shoot(float clampValue)
    {
        GameObject tempArrow = Instantiate(arrowPrefab, arrowPlacement.transform.position, handLookAtSphere.rotation);
        Rigidbody rBody = tempArrow.GetComponent<Rigidbody>();
        //MassHolder massHolder = tempArrow.GetComponent<MassHolder>();
        //rBody.centerOfMass = massHolder.com.position;
        rBody.AddForce(CalculateSlingshotProjectileVelocity(), ForceMode.Impulse);

        //Destroys the arrow after an amount of time
        DestroyAfterTime(tempArrow);


    }

    private Vector3 CalculateSlingshotProjectileVelocity()
    {
        //get vector and distance
        Vector3 slingshotPullbackVector = redBallThing.position - rightHandBall.position;
        float pullbackDist = Vector3.Distance(redBallThing.position, rightHandBall.position);

        //get the percent of the pullback distance
        float percent = Mathf.InverseLerp(0.1f, 1.00f, pullbackDist);
        //apply the percent to the curve
        float powerByCurve = Mathf.Lerp(6f, 100f, bowPullbackPowerCurve.Evaluate(percent));

        //cap power
        if (powerByCurve > 75f)
            powerByCurve = 75f;

        return Vector3.ClampMagnitude(slingshotPullbackVector, 2f) * powerByCurve;
    }

    IEnumerator ReadyToShoot()
    {
        notTriggered = false;

        inactiveShotUI.SetActive(false);
        activeShotUI.SetActive(true);

        lineR.useWorldSpace = true;
        

        while(rightHandGrab.ReadValue<float>() > 0.9f)
        {

            handLookAtSphere.transform.position = rightHandBall.position;

            lineR.SetPosition(0, redBallThing.position);
            lineR.SetPosition(1, rightHandBall.position);

            yield return null;
        }

        /*
        var heading = rightHandPosition.ReadValue<Vector3>() - arrowPlacement.position;
        var distance = heading.magnitude;
        var direction = heading / distance;
        arrowPlacement.localEulerAngles = direction;

        forceAmount = distance - 0.22f;
        */

        //print(distance);
        //float clampedValue = Mathf.Clamp(forceAmount, 0, 100);
        //print(clampedValue);

        Shoot(0);

        lineR.useWorldSpace = false;
        lineR.SetPosition(0, new Vector3(0, 0, 0));
        lineR.SetPosition(1, new Vector3(0, 0, 0));

        inactiveShotUI.SetActive(true);
        activeShotUI.SetActive(false);

        notTriggered = true;


        yield return null;
    }

    IEnumerator DestroyAfterTime(GameObject arrowObj)
    {
        yield return waitForDestroyTime;
        if(arrowObj.activeInHierarchy)
        {
            Destroy(arrowObj);
        }

    }
}
