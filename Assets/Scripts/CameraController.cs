using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;
    public GameObject jumperObject;

    public bool fixedPosition;
    public bool fixedRotation;

    
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        GetComponent<Transform>().position = jumperObject.GetComponent<Transform>().position + offset;
    }
}
