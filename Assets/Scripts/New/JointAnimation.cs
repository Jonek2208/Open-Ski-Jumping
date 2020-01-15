using UnityEngine;

public class JointAnimation : MonoBehaviour
{
    public bool invert;
    public float torqueForce;
    public float angularDamping;
    public float maxForce;
    public float springForce;
    public float springDamping;

    public Vector3 targetVel;

    public Vector3 targetRotation;
    public ConfigurableJoint joint;
    private GameObject limb;
    private JointDrive drive;
    private SoftJointLimitSpring spring;
    private Quaternion startingRotation;

    void Start()
    {
        Configure();
    }

    public void Configure()
    {
        drive.positionSpring = torqueForce;
        drive.positionDamper = angularDamping;
        drive.maximumForce = maxForce;

        spring.spring = springForce;
        spring.damper = springDamping;

        joint.slerpDrive = drive;
        joint.linearLimitSpring = spring;
        joint.rotationDriveMode = RotationDriveMode.Slerp;
        joint.projectionMode = JointProjectionMode.None;
        joint.targetAngularVelocity = targetVel;
        joint.configuredInWorldSpace = false;
        joint.swapBodies = true;

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        startingRotation = Quaternion.Inverse(Quaternion.Euler(targetRotation));
    }

    void LateUpdate()
    {
        if (invert)
        {
            joint.targetRotation = Quaternion.Inverse(Quaternion.Euler(targetRotation) * startingRotation);
        }
        else
        {
            joint.targetRotation = Quaternion.Euler(targetRotation) * startingRotation;
        }
    }
}