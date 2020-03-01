using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private Transform tr;
    private Camera cam;
    public Transform jumperTransform;
    public bool fixedPosition;
    public bool fixedRotation;
    public bool fixedZoom;
    public float zoom;

    private Vector3 position;
    private Vector3 offset;
    private float angleSize;

    public Camera Camera { get => cam; set => cam = value; }
    public float AngleSize { get => angleSize; set => angleSize = value; }

    public void RecalculateAngleSize()
    {
        // angleSize = (jumperTransform.position - tr.position).magnitude * Mathf.Tan(Mathf.Deg2Rad * cam.fieldOfView / 2.0f);
        AngleSize = (jumperTransform.position - tr.position).magnitude * 2.0f / 360.0f * Mathf.PI * Camera.fieldOfView;
    }
    void Awake()
    {
        tr = GetComponent<Transform>();
        Camera = GetComponent<Camera>();
    }

    void Start()
    {
        offset = jumperTransform.position - tr.position;
        RecalculateAngleSize();
    }

    void Update()
    {
        if (fixedPosition)
        {
            tr.LookAt(jumperTransform);
            if (fixedZoom)
            {
                // cam.fieldOfView = 2.0f * Mathf.Atan(angleSize / (jumperTransform.position - tr.position).magnitude) * Mathf.Rad2Deg;
                Camera.fieldOfView = 360.0f * AngleSize / (jumperTransform.position - tr.position).magnitude / (2.0f * Mathf.PI);
            }
        }
        if (fixedRotation)
        {
            tr.position = jumperTransform.position + offset;
        }

        // if(GetComponent<Camera>().enabled) Debug.Log("offsetMagnitude: " + (float)(jumperObject.GetComponent<Transform>().position - GetComponent<Transform>().position).magnitude);

    }
}