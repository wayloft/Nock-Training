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

    void Shoot()
    {
        

        GameObject tempArrow = Instantiate(arrowPrefab, arrowPlacement.transform.position, handLookAtSphere.rotation);
        Rigidbody rBody = tempArrow.GetComponent<Rigidbody>();
        MassHolder massHolder = tempArrow.GetComponent<MassHolder>();
        rBody.centerOfMass = massHolder.com.position;
        rBody.AddForce(tempArrow.transform.forward * (forceAmount * 30), ForceMode.Impulse);
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

        var heading = rightHandPosition.ReadValue<Vector3>() - arrowPlacement.position;
        var distance = heading.magnitude;
        var direction = heading / distance;
        arrowPlacement.localEulerAngles = direction;

        forceAmount = distance;
        print(forceAmount * 10);

        Shoot();

        lineR.useWorldSpace = false;
        lineR.SetPosition(0, new Vector3(0, 0, 0));
        lineR.SetPosition(1, new Vector3(0, 0, 0));

        inactiveShotUI.SetActive(true);
        activeShotUI.SetActive(false);

        notTriggered = true;

        yield return null;
    }
}
