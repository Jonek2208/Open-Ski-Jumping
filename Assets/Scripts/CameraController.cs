using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;
    public GameObject jumperObject;

    public bool fixedPosition;
    public bool fixedRotation;
    public bool fixedZoom;

    public Vector3 position;

    public float zoom;


    void Start()
    {

    }

    void FixedUpdate()
    {
        if (fixedPosition)
        {
            offset = jumperObject.GetComponent<Transform>().position - position;
            float offsetMagnitude = offset.magnitude;
            Quaternion rotation = new Quaternion();
            // Vector3 offset2 = offset - (fixedZoom ? (offsetMagnitude - zoom) / offsetMagnitude : zoom) * offset;
            rotation.eulerAngles = new Vector3(Mathf.Rad2Deg * Mathf.Atan2(-offset.y, Mathf.Sqrt(offset.x * offset.x + offset.z * offset.z)), Mathf.Rad2Deg * Mathf.Atan2(offset.x, offset.z), 0 * Mathf.Rad2Deg * Mathf.Atan2(offset.z, offset.y));
            GetComponent<Transform>().rotation = rotation;
            GetComponent<Transform>().position = position + (fixedZoom ? (offsetMagnitude - zoom) / offsetMagnitude : zoom) * offset;
        }
        else
        {
            GetComponent<Transform>().position = jumperObject.GetComponent<Transform>().position + offset;
        }



    }
}
