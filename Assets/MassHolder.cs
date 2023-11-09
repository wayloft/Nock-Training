using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassHolder : MonoBehaviour
{
    public Transform com;
    [SerializeField] Rigidbody rb;

    private void Awake()
    {
        rb.centerOfMass = com.position;
    }
}
