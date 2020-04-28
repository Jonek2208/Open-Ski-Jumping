using UnityEngine;

namespace OpenSkiJumping.New
{
    public enum JumperState { Gate, Inrun, TakeOff, Flight, PreLanding, Landing, Outrun, Braking, Crash }

    public class JumperControllerRagdoll : MonoBehaviour
    {
        public Transform com;
        public JumperPose inrunPose;
        public JumperPose flightHighPose;
        public JumperPose flightLowPose;
        public JumperPose landingPose;
        public JointAnimation bodyJointR;
        public JointAnimation bodyJointL;
        public JointAnimation kneesJointR;
        public JointAnimation kneesJointL;
        public JointAnimation anklesJointR;
        public JointAnimation anklesJointL;
        public GameObject hips;
        public GameObject skiL;
        public GameObject skiR;
        private Rigidbody rb;
        private Rigidbody rbSkiL;
        private Rigidbody rbSkiR;

        [SerializeField]
        private JumperState state;
        public JumperState State { get => state; private set => state = value; }
        public float Distance { get; private set; }
        public bool Landed { get; private set; }
        public bool OnInrun { get; private set; }
        public bool oldMode;

        [Header("Colliders")]

        public Collider bodyCollider;

        [Space]

        [Header("Parameters")]
        public float mass;
        public float jumpSpeed;
        public float jumperAngle;

        public float brakeForce;
        [Space]

        [Header("Flight")]
        public double angle;
        public double drag;
        public double lift;
        [SerializeField]
        private float torqueVal;
        public float rotCoef = 1f;
        public float mouseSensitivity = 2f;
        [Space]

        [Header("Wind")]
        public float windForce;

        bool button0, button1;

        // public JudgesController judgesController;

        private int landing;
        private bool deductedforlanding;

        private bool judged;

        public GameObject[] jumperParts;


        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Inrun")
            {
                OnInrun = true;
            }
            if (!Landed && other.tag == "LandingArea")
            {
                // judgesController.DistanceMeasurement((distCollider1.transform.position + distCollider2.transform.position) / 2.0f);
                Landed = true;
            }
        }

        void InputHandle()
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (State)
                {
                    case JumperState.Gate:
                        State = JumperState.Inrun;
                        JumpStart();
                        break;
                    case JumperState.Inrun:
                        State = JumperState.TakeOff;
                        TakeOff();
                        break;
                    case JumperState.Flight:
                        State = JumperState.PreLanding;
                        Land();
                        break;
                }

            }

            if (Input.GetMouseButtonDown(1))
            {
                bodyJointR.torqueForce = 0;
                bodyJointL.torqueForce = 0;
                kneesJointR.torqueForce = 0;
                kneesJointL.torqueForce = 0;
                anklesJointR.torqueForce = 0;
                anklesJointL.torqueForce = 0;

                bodyJointR.Configure();
                bodyJointL.Configure();
                kneesJointR.Configure();
                kneesJointL.Configure();
                anklesJointR.Configure();
                anklesJointL.Configure();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Inrun")
            {
                OnInrun = false;
                // judgesController.SpeedMeasurement(rb.velocity.magnitude);
            }
        }

        void OnCollisionEnter(Collision other)
        {
            if (!Landed && other.collider.tag == "LandingArea")
            {
                if (State == JumperState.PreLanding && !deductedforlanding)
                {
                    // judgesController.PointDeduction(1, 1);
                    deductedforlanding = true;
                }
                else
                {
                    // judgesController.DistanceMeasurement((distCollider1.transform.position + distCollider2.transform.position) / 2.0f);

                    Crash();
                    Landed = true;
                }

            }
        }

        void Start()
        {
            State = JumperState.Gate;
            Landed = false;

            rb = hips.GetComponent<Rigidbody>();
            rbSkiL = skiL.GetComponent<Rigidbody>();
            rbSkiR = skiR.GetComponent<Rigidbody>();
            ResetValues();
        }

        void SetPose(JumperPose pose)
        {
            bodyJointR.targetRotation.x = pose.bodyAngle;
            bodyJointL.targetRotation.x = pose.bodyAngle;
            kneesJointR.targetRotation.x = pose.kneesAngle;
            kneesJointL.targetRotation.x = pose.kneesAngle;
            anklesJointR.targetRotation.x = pose.anklesAngle;
            anklesJointL.targetRotation.x = pose.anklesAngle;
        }

        void SetPose(JumperPose pose1, JumperPose pose2, float factor)
        {
            float val = Mathf.SmoothStep(0, 1, factor);
            bodyJointR.targetRotation.x = pose1.bodyAngle * val + (1 - val) * pose2.bodyAngle;
            bodyJointL.targetRotation.x = pose1.bodyAngle * val + (1 - val) * pose2.bodyAngle;
            kneesJointR.targetRotation.x = pose1.kneesAngle * val + (1 - val) * pose2.kneesAngle;
            kneesJointL.targetRotation.x = pose1.kneesAngle * val + (1 - val) * pose2.kneesAngle;
            anklesJointR.targetRotation.x = pose1.anklesAngle * val + (1 - val) * pose2.anklesAngle;
            anklesJointL.targetRotation.x = pose1.anklesAngle * val + (1 - val) * pose2.anklesAngle;
        }

        public void ResetValues()
        {

            // hips.GetComponent<Transform>().position = GetComponent<Transform>().position;
            // hips.GetComponent<Transform>().rotation = Quaternion.identity;

            rb.isKinematic = true;
            State = JumperState.Gate;
            Landed = false;
            jumperAngle = 1;
            button0 = button1 = false;
            deductedforlanding = false;
            judged = false;
        }


        void Update()
        {
            InputHandle();

            if (State == JumperState.Flight)
            {
                jumperAngle += Input.GetAxis("Mouse Y") * mouseSensitivity;
                jumperAngle /= 1.05f;
                jumperAngle = Mathf.Clamp(jumperAngle, -1, 1);

                SetPose(flightHighPose, flightLowPose, (jumperAngle + 1) / 2);

                // judgesController.FlightStability(jumperAngle);

                Vector3 torque = new Vector3(0.0f, 0.0f, jumperAngle * rotCoef * mouseSensitivity);
                rb.AddTorque(torque, ForceMode.VelocityChange);
                // rb.AddForceAtPosition(Vector3.down * jumperAngle * rotCoef * mouseSensitivity, com.position, ForceMode.Acceleration);
                // Debug.Log(torque);
                // rb.AddTorque(torque, ForceMode.VelocityChange);
            }

            // if (rb.transform.position.x > judgesController.hill.U.x)
            // {
            //     State = JumperState.Braking;
            //     Brake();
            // }
        }

        private double Lift(double angle)
        {
            if (-15 <= angle && angle <= 50)
            {
                return 0.000933 + 0.00023314 * angle - 0.00000008201 * angle * angle - 0.0000001233 * angle * angle * angle + 0.00000000169 * angle * angle * angle * angle;
            }
            return 0;
        }

        private double Drag(double angle)
        {
            if (-15 <= angle && angle <= 50)
            {
                return 0.001822 + 0.000096017 * angle + 0.00000222578 * angle * angle - 0.00000018944 * angle * angle * angle + 0.00000000352 * angle * angle * angle * angle;
            }
            return 0;
        }

        void FixedUpdate()
        {
            Vector3 vel = rb.velocity + rb.velocity.normalized * windForce;
            Vector3 liftVec = new Vector3(-vel.normalized.y, vel.normalized.x, 0.0f);
            double tmp = (rbSkiL.rotation.eulerAngles.z + rbSkiL.rotation.eulerAngles.z) * 0.5;
            if (tmp > 180) tmp -= 360;

            angle = -Mathf.Atan(rb.velocity.normalized.y / rb.velocity.normalized.x) * 180 / Mathf.PI + tmp;

            lift = Lift(angle);
            drag = Drag(angle);

            if (State == JumperState.Flight)
            {
                rb.AddForce(-vel.normalized * (float)drag * vel.sqrMagnitude * mass / rb.mass, ForceMode.Acceleration);
                rb.AddForce(liftVec * (float)lift * vel.sqrMagnitude * mass / rb.mass, ForceMode.Acceleration);
                // Vector3 torque = new Vector3(0.0f, 0.0f, Mathf.Pow(Mathf.Sin(2 * (float)angle * Mathf.Deg2Rad), 3) * Time.fixedDeltaTime * 100f);
                Vector3 torque = new Vector3(0.0f, 0.0f, (90 - (float)angle) * Time.fixedDeltaTime * 0.5f * mass/* * 70.0f*/);
                torqueVal = torque.z;
                rb.AddRelativeTorque(torque);
            }
            if (State == JumperState.Braking)
            {
                Vector3 brakeVec = Vector3.left;
                // float distToEnd = judgesController.hill.U.x + 100 - rb.position.x;

                rb.AddForce(brakeVec * rb.mass * rb.velocity.x / 2);
            }
        }

        public void JumpStart()
        {
            rb.isKinematic = false;
            SetPose(inrunPose);
        }

        public void TakeOff()
        {
            Debug.Log("TakeOff");

            Vector3 jumpDirection = rb.velocity.normalized;
            jumpDirection = new Vector3(-jumpDirection.y, jumpDirection.x, 0);
            rb.AddForce(jumpSpeed * jumpDirection * mass / rb.mass, ForceMode.VelocityChange);

            State = JumperState.Flight;
            SetPose(flightHighPose);
        }

        public void Land()
        {
            State = JumperState.PreLanding;
            landing = 1;
            if (button0 && button1)
            {
                landing = 0;

            }
            if (landing == 0)
            {
                // judgesController.PointDeduction(1, 1m);
                landing = -1;
            }

            SetPose(landingPose);
        }

        public void Crash()
        {
            if (State == JumperState.Flight)
            {
                // judgesController.PointDeduction(1, 3);
            }

            // judgesController.PointDeduction(2, 7);
            if (!judged)
            {
                // judgesController.Judge();
                judged = true;
            }
        }

        public void Brake()
        {
            if (!judged)
            {
                // judgesController.Judge();
                judged = true;
            }
        }
    }
}