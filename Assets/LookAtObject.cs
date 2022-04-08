using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    [SerializeField] Transform lookAtThis;

    private void FixedUpdate()
    {
        transform.LookAt(lookAtThis);
    }
}
