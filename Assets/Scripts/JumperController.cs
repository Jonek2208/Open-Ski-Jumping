using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    public int State { get; private set; }
    public float Distance { get; private set; }
    public bool Landed { get; private set; }
    public bool OnInrun { get; private set; }
    public bool oldMode;

    [Header("Colliders")]

    public Collider distCollider;

    [Space]

    [Header("Parameters")]
    public float jumpSpeed;
    public float jumperAngle = 0f;

    public float brakeForce;
    [Space]

    [Header("Flight")]
    public double angle;
    public double drag = 0.001d;
    public double lift = 0.001d;
    public float rotCoef;
    public float smoothCoef = 0.01f;
    public float sensCoef = 0.01f;
    [Space]

    [Header("Wind")]
    public Vector2 windDir;
    public float windForce;
    public float dirChange;
    public float forceChange;


    //This should be loaded from file
    float[,] tab = new float[,]
            {
            {0.0f,  -3.2f,    0.0f,   0.0f},
            {5.0f,  -3.88f,   0.0f, 0.0f},
            {10.0f,  -4.82f,    0.0f,   0.0f},
            {15.0f,  -5.99f,    0.0f,   0.0f},
            {20.0f,  -7.39f,    0.0f,   0.0f},
            {25.0f,  -9.01f,    0.0f,   0.0f},
            {30.0f,  -10.85f,    0.0f,   0.0f},
            {35.0f,  -12.88f,    0.0f,   0.0f},
            {40.0f,  -15.1f,    0.0f,   0.0f},
            {45.0f,  -17.5f,    0.0f,   0.0f},
            {50.0f,  -20.07f,    0.0f,   0.0f},
            {55.0f,  -22.81f,    0.0f,   0.0f},
            {60.0f,  -25.69f,    0.0f,   0.0f},
            {65.0f,  -28.71f,    0.0f,   0.0f},
            {70.0f,  -31.86f,    0.0f,   0.0f},
            {75.0f,  -35.14f,    0.0f,   0.0f},
            {80.0f,  -38.52f,    0.0f,   0.0f},
            {85.0f,  -42f,    0.0f,   0.0f},
            {90.0f,  -45.58f,    0.0f,   0.0f},
            {95.0f,  -49.23f,    0.0f,   0.0f},
            {100.0f,  -52.95f,    0.0f,   0.0f},
            {105.0f,  -56.74f,    0.0f,   0.0f},
            {110.0f,  -60.58f,    0.0f,   0.0f},
            {115.0f,  -64.45f,    0.0f,   0.0f},
            {117.29f,  -66.24f,    0.0f,   0.0f},
            {120.0f,  -68.34f,    0.0f,   0.0f},
            {125.0f,  -72.12f,    0.0f,   0.0f},
            {130.0f,  -75.79f,    0.0f,   0.0f},
            {135.0f,  -79.34f,    0.0f,   0.0f},
            {137.97f,  -81.4f,    0.0f,   0.0f},
            {140.0f,  -82.79f,    0.0f,   0.0f},
            {145.0f,  -86.13f,    0.0f,   0.0f},
            {150.0f,  -89.37f,    0.0f,   0.0f},
            {153.87f,  -91.81f,    0.0f,   0.0f},
            {155.0f,  -92.51f,    0.0f,   0.0f},
            {160.0f,  -95.44f,    0.0f,   0.0f},
            {165.0f,  -98.14f,    0.0f,   0.0f},
            {170.0f,  -100.61f,    0.0f,   0.0f},
            {175.0f,  -102.83f,    0.0f,   0.0f},
            {180.0f,  -104.82f,    0.0f,   0.0f},
            {185.0f,  -106.57f,    0.0f,   0.0f},
            {190.0f,  -108.09f,    0.0f,   0.0f},
            {195.0f,  -109.38f,    0.0f,   0.0f},
            {200.0f,  -110.43f,    0.0f,   0.0f},
            {205.0f,  -111.26f,    0.0f,   0.0f},
            {210.0f,  -111.85f,    0.0f,   0.0f},
            {215.0f,  -112.21f,    0.0f,   0.0f},
            {220.0f,  -112.35f,    0.0f,   0.0f},
            {220.47f,  -112.35f,    0.0f,   0.0f},
            {225.0f,  -112.35f,    0.0f,   0.0f}
            };


    void OnTriggerEnter(Collider other)
    {
        //    print(distCollider.transform.position);
        if (other.tag == "Inrun")
        {
            OnInrun = true;
        }
        if (!Landed && other.tag == "LandingArea")
        {
            if (State != 3)
            {
                Crash();
            }
            Landed = true;
            DistanceMeasurement(distCollider.transform.position);
        }
        if (other.tag == "Outrun")
        {
            Brake();
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Inrun")
        {
            OnInrun = false;
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

        //preprocessing
        {
            for (int i = 1; i < tab.Length / 4; i++)
            {
                tab[i, 2] = Mathf.Sqrt((tab[i - 1, 0] - tab[i, 0]) * (tab[i - 1, 0] - tab[i, 0]) + (tab[i - 1, 1] - tab[i, 1]) * (tab[i - 1, 1] - tab[i, 1]));
                tab[i, 3] = tab[i - 1, 3] + tab[i, 2];
                //print("[" + i + "]" + tab[i, 2] + ", " + tab[i, 3]);
            }
        }

    }

    void Update()
    {
        animator.SetInteger("JumperState", State);
        if (State == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            Gate();
        }
        else if (State == 1 && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        else if ((State == 2 || State == 3) && Input.GetKeyDown(KeyCode.Space))
        {
            Land();
        }
        if (State == 2)
        {
            //double pitchMoment = 3.29363 - 0.11567 * angle - 0.00333928 * angle * angle + 0.0000573605f * angle * angle * angle;
            //double pitchMoment = 0.5f;
            if (oldMode)
            {
                jumperAngle = Input.GetAxis("Vertical");
            }
            else
            {
                jumperAngle -= jumperAngle * jumperAngle * Mathf.Sign(jumperAngle) * smoothCoef;
                jumperAngle += Input.GetAxis("Vertical") * sensCoef;
                jumperAngle = Mathf.Clamp(jumperAngle, -1, 1);
            }

            //rb.AddTorque(0.0f, 0.0f, (float)pitchMoment);

            Vector3 torque = new Vector3(0.0f, 0.0f, -jumperAngle * rotCoef/* * 70.0f*/);
            rb.AddRelativeTorque(torque);
            animator.SetFloat("JumperAngle", jumperAngle);
            Debug.Log("angle: " + angle + " jumperAngle: " + jumperAngle);
        }
        if ((Landed && State != 3 && !land) || (State == 2 && (angle < -10.0f || angle > 80.0f) && animator.GetCurrentAnimatorStateInfo(0).IsName("Flight")))
        {
            //Crash();
        }
    }

    void FixedUpdate()
    {
        //ToDo

        //wind
        //windDir += Random.insideUnitCircle * dirChange;
        //windDir.Normalize();
        //windForce += Random.Range(-1.0f, 1.0f) * forceChange;
        //windForce = Mathf.Clamp(windForce, 0.0f, 4.0f);

        Vector3 vel = rb.velocity + rb.velocity.normalized * windForce;
        Debug.Log(vel);
        Vector3 liftVec = new Vector3(-vel.normalized.y, vel.normalized.x, 0.0f);
        double tmp = rb.rotation.eulerAngles.z;
        if (tmp > 180) tmp -= 360;
        angle = -Mathf.Atan(rb.velocity.normalized.y / rb.velocity.normalized.x) * 180 / Mathf.PI + tmp;
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
            //rb.AddForceAtPosition(-vel.normalized * (float)drag * vel.sqrMagnitude, rb.transform.position);
            //rb.AddForceAtPosition(liftVec * (float)lift * vel.sqrMagnitude, rb.transform.position);
        }
        if (State == 4)
        {
            Vector3 brakeVec = Vector3.left * brakeForce;
            rb.AddForce(brakeVec);
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

        if (OnInrun)
        {
            Vector3 jumpDirection = rb.velocity.normalized;
            jumpDirection = new Vector3(-jumpDirection.y, jumpDirection.x, 0);
            rb.velocity += jumpSpeed * jumpDirection;
            //rb.AddTorque(0.0f, 0.0f, 10f);
        }

    }

    /*public void Flight(float val)
    {

    }
    */

    public void DistanceMeasurement(Vector3 position)
    {
        //Distance = position.magnitude/1.005f;
        int it = 0;
        Debug.Log("distance: " + Distance);

        //Debug.Log(it + " " + position.x + " " + tab[it, 0]);
        while (it < tab.Length / 4 && position.x > tab[it, 0])
        {
            //Debug.Log(it + " " + position.x + " " + tab[it, 0]);
            it++;
        }
        it--;
        Vector2 last = new Vector2(position.x - tab[it, 0], position.y - tab[it, 1]);
        //Debug.Log(tab[it, 3] + " " + last.magnitude);
        Distance = tab[it, 3] + last.magnitude;
        //Debug.Log(Distance);
    }
    
    bool land = false;
    public void Land()
    {
        //TODO
        if(land == false)
        {
            animator.SetFloat("Landing", 0);
            land = true;
            State = 3;
        }
        else
        {
            animator.SetFloat("Landing", 1);
            
        }        
    }

    public void Crash()
    {
        //Na plecy i na brzuch
        //State = ;
        animator.SetBool("JumperCrash", true);  
    }

    public void Brake()
    {
        //ToDo
        State = 4;
    }
}