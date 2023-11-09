using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotateTowardsMoveDirection : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float rotationSpeed = 5f; // Adjust this value to control the speed of rotation
    [SerializeField] private LayerMask groundLayer; // Assign the ground layer here in the Inspector
    [SerializeField] private float groundCheckDistance = 0.5f; // Adjust this value based on your object's size

    private void Start()
    {
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // For fast moving object
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Prevent rotation on the X and Z axes
    }

    void Update()
    {
        if (!IsGrounded())
        {
            RotateTowardsVelocityDirection();
        }
    }

    private bool IsGrounded()
    {
        // Using a Raycast to detect if the object is grounded
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void RotateTowardsVelocityDirection()
    {
        if (rb.velocity.magnitude > 0.1f) // Threshold to prevent jittery rotation when velocity is low
        {
            Vector3 direction = rb.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 bounceDirection;

        // Check if the collision is with the ground
        if (collision.gameObject.layer == groundLayer.value)
        {
            // Reflect only in the up direction
            bounceDirection = Vector3.Reflect(rb.velocity, Vector3.up);
        }
        else
        {
            // Reflect based on the collision's normal
            bounceDirection = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
        }

        rb.velocity = bounceDirection;
    }
}
