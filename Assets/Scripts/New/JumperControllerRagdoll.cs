using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperControllerRagdoll : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    public GameObject rSki, lSki;
    public GameObject rSkiClone, lSkiClone;
    public int State { get; private set; }
    public float Distance { get; private set; }
    public bool Landed { get; private set; }
    public bool OnInrun { get; private set; }
    public bool oldMode;

    [Header("Colliders")]

    public SphereCollider distCollider1;
    public SphereCollider distCollider2;
    public Collider bodyCollider;

    public Collider rSkiCollider;
    public Collider lSkiCollider;


    [Space]

    [Header("Parameters")]
    public float jumpSpeed;
    public float jumperAngle = 0f;

    public float brakeForce;
    [Space]

    [Header("Flight")]
    public double angle = 0;
    public double drag = 0.001d;
    public double lift = 0.001d;
    public float rotCoef = 1f;
    public float smoothCoef = 0.01f;
    public float sensCoef = 0.01f;
    public float mouseSensitivity = 2f;
    [Space]

    [Header("Wind")]
    public Vector2 windDir;
    public float windForce;
    public float dirChange;
    public float forceChange;

    public GameObject modelObject;

    bool button0, button1;

    public JudgesController judgesController;
    public MouseScript mouseScript;
    // public ManagerScript managerScript;

    private int landing;
    private bool deductedforlanding = false;

    private bool judged = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Inrun")
        {
            OnInrun = true;
        }
        if (!Landed && other.tag == "LandingArea")
        {
            judgesController.DistanceMeasurement((distCollider1.transform.position + distCollider2.transform.position) / 2.0f);

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
            {
                Crash();
            }
            Landed = true;
            animator.SetFloat("DownForce", 1f);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Inrun")
        {
            OnInrun = false;
            judgesController.SpeedMeasurement(rb.velocity.magnitude);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!Landed && other.collider.tag == "LandingArea")
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
            {
                if (State == 3 && !deductedforlanding)
                {
                    judgesController.PointDeduction(1, 1);
                    deductedforlanding = true;
                }
                else
                {
                    judgesController.DistanceMeasurement((distCollider1.transform.position + distCollider2.transform.position) / 2.0f);

                    Crash();
                    Landed = true;
                }


            }

        }
    }

    void Start()
    {
        State = 0;
        Landed = false;

        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        //Wind
        //windDir.Set(0.0f, 0.0f);
        //windForce = 0.0f;

        ResetValues();
    }

    public void ResetValues()
    {
        State = 0;
        animator.SetBool("JumperCrash", false);
        animator.SetInteger("JumperState", State);
        animator.SetFloat("DownForce", 0f);
        Landed = false;
        rb.isKinematic = true;
        modelObject.GetComponent<Transform>().localPosition = new Vector3();
        jumperAngle = 1;
        button0 = button1 = false;
        rSkiClone.SetActive(false);
        lSkiClone.SetActive(false);
        rSki.SetActive(true);
        lSki.SetActive(true);
        deductedforlanding = false;
        judged = false;
    }


    void Update()
    {
        animator.SetInteger("JumperState", State);
        if (State == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            Gate();
            mouseScript.LockCursor();
        }
        else if (State == 1 && Input.GetMouseButtonDown(0))
        {
            Jump();
        }
        else if ((State == 2 || State == 3) && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            button0 |= Input.GetMouseButtonDown(0);
            button1 |= Input.GetMouseButtonDown(1);
            Land();
        }
        if (State == 2)
        {
            //double pitchMoment = 3.29363 - 0.11567 * angle - 0.00333928 * angle * angle + 0.0000573605f * angle * angle * angle;
            //double pitchMoment = 0.5f;
            if (oldMode)
            {
                jumperAngle += Time.deltaTime * Input.GetAxis("Mouse Y") * mouseSensitivity;
                jumperAngle = Mathf.Clamp(jumperAngle, -1, 1);
            }
            else
            {
                jumperAngle += Time.deltaTime * Input.GetAxis("Mouse Y") * mouseSensitivity;
                jumperAngle /= 1.05f;
                jumperAngle = Mathf.Clamp(jumperAngle, -1, 1);
                // jumperAngle -= jumperAngle * jumperAngle * Mathf.Sign(jumperAngle) * smoothCoef;
                // jumperAngle += Input.GetAxis("Moues Y") * sensCoef;
                // jumperAngle = Mathf.Clamp(jumperAngle, -1, 1);
            }

            judgesController.FlightStability(jumperAngle);

            //rb.AddTorque(0.0f, 0.0f, (float)pitchMoment);
            if (oldMode)
            {

            }
            else
            {
                Vector3 torque = new Vector3(0.0f, 0.0f, jumperAngle * rotCoef * mouseSensitivity/* * 70.0f*/);
                rb.AddRelativeTorque(torque);
            }

            animator.SetFloat("JumperAngle", jumperAngle);
            // Debug.Log("angle: " + angle + " jumperAngle: " + jumperAngle);
        }
        if (rb.transform.position.x > judgesController.hill.U.x)
        {
            Brake();

        }
        // if ((Landed && State != 3 && !land) || (State == 2 && (angle < -10.0f || angle > 80.0f) && animator.GetCurrentAnimatorStateInfo(0).IsName("Flight")))
        // {
        //     // Crash();
        // }
    }

    void FixedUpdate()
    {
        Vector3 vel = rb.velocity + rb.velocity.normalized * windForce;
        // Debug.Log(vel);
        Vector3 liftVec = new Vector3(-vel.normalized.y, vel.normalized.x, 0.0f);
        double tmp = rb.rotation.eulerAngles.z;
        if (tmp > 180) tmp -= 360;

        angle = -Mathf.Atan(rb.velocity.normalized.y / rb.velocity.normalized.x) * 180 / Mathf.PI + tmp;
        if (oldMode)
        {
            angle = -Mathf.Atan(rb.velocity.normalized.y / rb.velocity.normalized.x) * 180 / Mathf.PI + jumperAngle * 10;
        }
        if (-15.0f <= angle && angle <= 50)
        {
            lift = 0.000933d + 0.00023314d * angle - 0.00000008201d * angle * angle - 0.0000001233d * angle * angle * angle + 0.00000000169d * angle * angle * angle * angle;
            drag = 0.001822d + 0.000096017d * angle + 0.00000222578d * angle * angle - 0.00000018944d * angle * angle * angle + 0.00000000352d * angle * angle * angle * angle;
        }


        //Debug.Log("angle: " + angle + " drag: " + drag + " lift: " + lift);
        if (State == 2)
        {
            rb.AddForce(-vel.normalized * (float)drag * vel.sqrMagnitude/* * rb.mass*/);
            rb.AddForce(liftVec * (float)lift * vel.sqrMagnitude/* * rb.mass*/);
            Vector3 torque = new Vector3(0.0f, 0.0f, (90 - (float)angle) * Time.fixedDeltaTime*0.5f/* * 70.0f*/);
            rb.AddRelativeTorque(torque);

            //rb.AddForceAtPosition(-vel.normalized * (float)drag * vel.sqrMagnitude, rb.transform.position);
            //rb.AddForceAtPosition(liftVec * (float)lift * vel.sqrMagnitude, rb.transform.position);
        }
        if (State == 4)
        {
            Vector3 brakeVec = Vector3.left;
            float distToEnd = judgesController.hill.U.x + 100 - rb.position.x;

            rb.AddForce(brakeVec * rb.mass * rb.velocity.x / 2);
        }
    }

    public void Gate()
    {
        State = 1;

        rb.isKinematic = false;
    }

    public void Jump()
    {
        State = 2;

        Debug.Log(OnInrun);
        if (OnInrun)
        {
            Vector3 jumpDirection = rb.velocity.normalized;
            jumpDirection = new Vector3(-jumpDirection.y, jumpDirection.x, 0);
            rb.velocity += jumpSpeed * jumpDirection;
            //rb.AddTorque(0.0f, 0.0f, 10f);
        }

    }

    public void Land()
    {
        float angle = rb.GetComponent<Rigidbody>().transform.rotation.eulerAngles.z;
        angle = (angle + 180) % 360 - 180;
        rb.AddTorque(0, 0, -angle * 5);
        State = 3;
        landing = 1;
        animator.SetFloat("Landing", 1);
        if (button0 && button1)
        {
            animator.SetFloat("Landing", 0);
            landing = 0;

        }
        if (landing == 0)
        {
            Debug.Log("RAKAKAN MALY CVEL");
            judgesController.PointDeduction(1, 1f);
            landing = -1;
        }

    }

    public void Crash()
    {
        if (State == 3)
        {
            judgesController.PointDeduction(1, 3);
        }
        else
        {
            judgesController.PointDeduction(1, 5);
            judgesController.PointDeduction(0, 5);
        }
        //Na plecy i na brzuch
        //State = ;
        animator.SetBool("JumperCrash", true);
        rSkiClone.SetActive(true);
        lSkiClone.SetActive(true);
        rSki.SetActive(false);
        lSki.SetActive(false);
        lSkiClone.GetComponent<Rigidbody>().velocity = rb.velocity * 0.7f;
        lSkiClone.GetComponent<Transform>().position = lSki.GetComponent<Transform>().position;
        lSkiClone.GetComponent<Transform>().rotation = lSki.GetComponent<Transform>().rotation;

        rSkiClone.GetComponent<Rigidbody>().velocity = rb.velocity * 0.7f;
        rSkiClone.GetComponent<Transform>().position = rSki.GetComponent<Transform>().position;
        rSkiClone.GetComponent<Transform>().rotation = rSki.GetComponent<Transform>().rotation;
        judgesController.PointDeduction(2, 7);
        if (!judged)
        {
            judgesController.Judge();
            judged = true;
        }

    }

    public void Brake()
    {
        //ToDo
        if (State != 4)
        {
            if (!judged)
            {
                judgesController.Judge();
                judged = true;
            }
        }
        State = 4;
    }
}