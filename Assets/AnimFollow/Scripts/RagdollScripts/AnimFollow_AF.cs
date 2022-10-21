#define RAGDOLLCONTROL
#define SIMPLEFOOTIK
using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public class AnimFollow_AF : MonoBehaviour
	{
		// Add this script to the ragdoll
		
		public readonly int version = 7; // The version of this script

		// Variables (expand #region by clicking the plus)
#region

#if RAGDOLLCONTROL
		RagdollControl_AF ragdollControl;
#endif

		public GameObject master; // ASSIGN IN INSPECTOR!
		public Transform[] masterTransforms; // These are all auto assigned
		public Transform[] masterRigidTransforms = new Transform[1];
		public Transform[] slaveTransforms;
		public Rigidbody[] slaveRigidbodies = new Rigidbody[1];
		public Vector3[] rigidbodiesPosToCOM;
		public Transform[] slaveRigidTransforms = new Transform[1];
		public Transform[] slaveExcludeTransforms;

		Quaternion[] localRotations1 = new Quaternion[1];
		Quaternion[] localRotations2 = new Quaternion[1];

		public float fixedDeltaTime = 0.01f; // If you choose to go to longer times you need to lower PTorque, PLocalTorque and PForce or the system gets unstable. Can be done, longer time is better performance but worse mimicking of master.
		float reciFixedDeltaTime; // 1f / fixedDeltaTime
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// The ranges are not set in stone. Feel free to extend the ranges
		[Range(0f, 100f)] public float maxTorque = 100f; // Limits the world space torque
		[Range(0f, 100f)] public float maxForce = 100f; // Limits the force
		[Range(0f, 10000f)] public float maxJointTorque = 10000f; // Limits the force
		[Range(0f, 10f)] public float jointDamping = .6f; // Limits the force

		public float[] maxTorqueProfile = {100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f}; // Individual limits per limb
		public float[] maxForceProfile = {1f, .2f, .2f, .2f, .2f, 1f, 1f, .2f, .2f, .2f, .2f, .2f};
		public float[] maxJointTorqueProfile = {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f};
		public float[] jointDampingProfile = {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f};

		[Range(0f, .64f)] public float PTorque = .16f; // For all limbs Torque strength
		[Range(0f, 160f)] public float PForce = 30f;
		
		[Range(0f, .008f)] public float DTorque = .002f; // Derivative multiplier to PD controller
		[Range(0f, .064f)] public float DForce = .01f;
		
	//	public float[] PTorqueProfile = {20f, 30f, 10f, 30f, 10f, 30f, 30f, 30f, 10f, 30f, 10f}; // Per limb world space torque strength
		public float[] PTorqueProfile = {20f, 30f, 10f, 30f, 10f, 30f, 30f, 30f, 30f, 10f, 30f, 10f}; // Per limb world space torque strength for EthanRagdoll_12 (twelve rigidbodies)
		public float[] PForceProfile = {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f};
		
		// The ranges are not set in stone. Feel free to extend the ranges
		[Range(0f, 340f)] public float angularDrag = 100f; // Rigidbodies angular drag. Unitys parameter
		[Range(0f, 2f)] public float drag = .5f; // Rigidbodies drag. Unitys parameter
		float maxAngularVelocity = 1000f; // Rigidbodies maxAngularVelocity. Unitys parameter
		
		[SerializeField] bool torque = false; // Use World torque to controll the ragdoll (if true)
		[SerializeField] bool force = true; // Use force to controll the ragdoll
		[HideInInspector] public bool mimicNonRigids = true; // Set all local rotations of the transforms without rigidbodies to match the local rotations of the master
		[HideInInspector] [Range(2, 100)] public int secondaryUpdate = 2;
		int frameCounter;
		public bool hideMaster = true;
		public bool useGravity = true; // Ragdoll is affected by Unitys gravity
		bool userNeedsToAssignStuff = false;
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		float torqueAngle; // Återanvänds för localTorque, därför ingen variabel localTorqueAngle
		Vector3 torqueAxis;
		Vector3 torqueError;
		Vector3 torqueSignal;
		Vector3[] torqueLastError = new Vector3[1];
		Vector3 torqueVelError;
		[HideInInspector] public Vector3 totalTorqueError; // Total world space angular error of all limbs. This is a vector.
		
		Vector3 forceAxis;
		Vector3 forceSignal;
		Vector3 forceError;
		Vector3[] forceLastError = new Vector3[1];
		Vector3 forceVelError;
		[HideInInspector] public Vector3 totalForceError; // Total world position error. a vector.
		public float[] forceErrorWeightProfile = {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f}; // Per limb error weight
		
		float masterAngVel;
		Vector3 masterAngVelAxis;
		float slaveAngVel;
		Vector3 slaveAngVelAxis;
		Quaternion masterDeltaRotation;
		Quaternion slaveDeltaRotation;
		Quaternion[] lastMasterRotation = new Quaternion[1];
		Quaternion[] lastSlaveRotation = new Quaternion[1];
		Quaternion[] lastSlavelocalRotation = new Quaternion[1];
		Vector3[] lastMasterPosition = new Vector3[1];
		Vector3[] lastSlavePosition = new Vector3[1];
		
		Quaternion[] startLocalRotation = new Quaternion[1];
		ConfigurableJoint[] configurableJoints = new ConfigurableJoint[1];
		Quaternion[] localToJointSpace = new Quaternion[1];
		JointDrive jointDrive = new JointDrive();
#endregion

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void Awake() // Initialize
		{
			int i = 0; // Just some counters
			int j = 0;
			int k = 0;
			int l = 0;
			Time.fixedDeltaTime = fixedDeltaTime; // Set the physics loop update intervall
	//		Debug.Log("The script AnimFollow has set the fixedDeltaTime to " + fixedDeltaTime); // Remove this line if you don't need the "heads up"
			reciFixedDeltaTime = 1f / fixedDeltaTime; // Cache the reciprocal
			
			if (!master)
			{
				UnityEngine.Debug.LogWarning("master not assigned in AnimFollow script on " + this.name + "\n");
				userNeedsToAssignStuff = true;
				return;
			}
#if SIMPLEFOOTIK
			else if (!master.GetComponent<SimpleFootIK_AF>())
			{
				UnityEngine.Debug.LogWarning("Missing script SimpleFootIK_AF on " + master.name + ".\nAdd it or comment out the directive from top line in the AnimFollow script." + "\n");
				userNeedsToAssignStuff = true;
			}
#else
			else if (master.GetComponent<SimpleFootIK_AF>())
			{
				UnityEngine.Debug.LogWarning("There is a SimpleFootIK script on\n" + master.name + " But the directive in the AnimFollow script is commented out");
				userNeedsToAssignStuff = true;
			}
#endif
			else if (hideMaster)
			{
				SkinnedMeshRenderer visible;
				MeshRenderer visible2;
				if (visible = master.GetComponentInChildren<SkinnedMeshRenderer>())
				{
					visible.enabled = false;
					SkinnedMeshRenderer[] visibles;
					visibles = master.GetComponentsInChildren<SkinnedMeshRenderer>();
					foreach (SkinnedMeshRenderer visiblen in visibles)
						visiblen.enabled = false;
				}
				if (visible2 = master.GetComponentInChildren<MeshRenderer>())
				{
					visible2.enabled = false;
					MeshRenderer[] visibles2;
					visibles2 = master.GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer visiblen2 in visibles2)
						visiblen2.enabled = false;
				}
			}
			#if RAGDOLLCONTROL
			if (!(ragdollControl = GetComponent<RagdollControl_AF>()))
			{
				UnityEngine.Debug.LogWarning("Missing script RagdollControl on " + this.name + ".\nAdd it or comment out the directive from top line in the AnimFollow script." + "\n");
				userNeedsToAssignStuff = true;
			}
			#else
			if (GetComponent<RagdollControl_AF>())
			{
				UnityEngine.Debug.LogWarning("There is a RagdollControl script on\n" + this.name + " But the directive in the AnimFollow script is commented out");
				userNeedsToAssignStuff = true;
			}
			#endif

			slaveTransforms = GetComponentsInChildren<Transform>(); // Get all transforms in ragdoll. THE NUMBER OF TRANSFORMS MUST BE EQUAL IN RAGDOLL AS IN MASTER!
			masterTransforms = master.GetComponentsInChildren<Transform>(); // Get all transforms in master. 
			System.Array.Resize(ref localRotations1, slaveTransforms.Length);
			System.Array.Resize(ref localRotations2, slaveTransforms.Length);
			System.Array.Resize(ref rigidbodiesPosToCOM, slaveTransforms.Length);

			if (!(masterTransforms.Length == slaveTransforms.Length))
			{
				UnityEngine.Debug.LogWarning(this.name + " does not have a valid master.\nMaster transform count does not equal slave transform count." + "\n");
				userNeedsToAssignStuff = true;
				return;
			}

			// Resize Arrays (expand #region)
			#region
			slaveRigidbodies = GetComponentsInChildren<Rigidbody>();
			j = slaveRigidbodies.Length;
			System.Array.Resize(ref masterRigidTransforms, j);
			System.Array.Resize(ref slaveRigidTransforms, j);

			System.Array.Resize(ref maxTorqueProfile, j);
			System.Array.Resize(ref maxForceProfile, j);
			System.Array.Resize(ref maxJointTorqueProfile, j);
			System.Array.Resize(ref jointDampingProfile, j);
			System.Array.Resize(ref PTorqueProfile, j);
			System.Array.Resize(ref PForceProfile, j);
			System.Array.Resize(ref forceErrorWeightProfile, j);
			
			System.Array.Resize(ref torqueLastError, j);
			System.Array.Resize(ref forceLastError, j);
			
			System.Array.Resize(ref lastMasterRotation, j);
			System.Array.Resize(ref lastSlaveRotation, j);
			System.Array.Resize(ref lastSlavelocalRotation, j);
			System.Array.Resize(ref lastMasterPosition, j);
			System.Array.Resize(ref lastSlavePosition, j);
			
			System.Array.Resize(ref startLocalRotation, j);
			System.Array.Resize(ref configurableJoints, j);
			System.Array.Resize(ref localToJointSpace, j);
			#endregion
			
//			int j = 0;
//			foreach (Transform ragdollRigidTransform in ragdollRigidTransforms) // Set up configurable joints and rigidbodies
			j = 0;
			foreach (Transform slaveTransform in slaveTransforms) // Sort the transform arrays
			{			
				if (slaveTransform.GetComponent<Rigidbody>())
				{
					slaveRigidTransforms[j] = slaveTransform;
					masterRigidTransforms[j] = masterTransforms[i];
					if (slaveTransform.GetComponent<ConfigurableJoint>())
					{
						configurableJoints[j] = slaveTransform.GetComponent<ConfigurableJoint>();
						Vector3 forward = Vector3.Cross (configurableJoints[j].axis, configurableJoints[j].secondaryAxis);
						Vector3 up = configurableJoints[j].secondaryAxis;
						localToJointSpace[j] = Quaternion.LookRotation(forward, up);
						startLocalRotation[j] = slaveTransform.localRotation * localToJointSpace[j];
						jointDrive = configurableJoints[j].slerpDrive;
						jointDrive.mode = JointDriveMode.Position;
						configurableJoints[j].slerpDrive = jointDrive;
						l++;
					}
					else if (j > 0)
					{
						UnityEngine.Debug.LogWarning("Rigidbody " + slaveTransform.name + " on " + this.name + " is not connected to a configurable joint" + "\n");
						userNeedsToAssignStuff = true;
						return;
					}
					rigidbodiesPosToCOM[j] = Quaternion.Inverse(slaveTransform.rotation) * (slaveTransform.GetComponent<Rigidbody>().worldCenterOfMass - slaveTransform.position); 
					j++;
				}
				else
				{
					bool excludeBool = false;
					foreach (Transform exclude in slaveExcludeTransforms)
					{
						if (slaveTransform == exclude)
						{
							excludeBool = true;
							break;
						}
					}

					if (!excludeBool)
					{
						slaveTransforms[k] = slaveTransform;
						masterTransforms[k] = masterTransforms[i];
						localRotations1[k] = slaveTransform.localRotation;
						k++;
					}
				}
				i++;
			}
			localRotations2 = localRotations1;
			System.Array.Resize(ref masterTransforms, k);
			System.Array.Resize(ref slaveTransforms, k);
			System.Array.Resize(ref localRotations1, k);
			System.Array.Resize(ref localRotations2, k);
			
			if (l == 0)
			{
				UnityEngine.Debug.LogWarning("There are no configurable joints on the ragdoll " + this.name + "\nDrag and drop the ReplaceJoints script on the ragdoll." + "\n");
				userNeedsToAssignStuff = true;
				return;
			}
			else
			{
				SetJointTorque (maxJointTorque);
				EnableJointLimits(false);
			}

			if (slaveRigidTransforms.Length == 0 )
				UnityEngine.Debug.LogWarning("There are no rigid body components on the ragdoll " + this.name + "\n");
			else if (slaveRigidTransforms.Length < 12)
				UnityEngine.Debug.Log("This version of AnimFollow works better with one extra colleder in the spine on " + this.name + "\n");

			if (PTorqueProfile[PTorqueProfile.Length - 1] == 0f)
				UnityEngine.Debug.Log("The last entry in the PTorqueProfile is zero on " + this.name +".\nIs that intentional?\nDrop ResizeProfiles on the ragdoll and adjust the values." + "\n");

			if (slaveExcludeTransforms.Length == 0)
			{
				UnityEngine.Debug.Log("Should you not assign some slaveExcludeTransforms to the AnimFollow script on " + this.name + "\n");
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void Start()
		{	
			if (userNeedsToAssignStuff)
				return;
			
			int i = 0;
			foreach(Transform slaveRigidTransform in slaveRigidTransforms) // Set some of the Unity parameters
			{
				slaveRigidTransform.GetComponent<Rigidbody>().useGravity = useGravity;
				slaveRigidTransform.GetComponent<Rigidbody>().angularDrag = angularDrag;
				slaveRigidTransform.GetComponent<Rigidbody>().drag = drag;
				slaveRigidTransform.GetComponent<Rigidbody>().maxAngularVelocity = maxAngularVelocity;
				i++;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if RAGDOLLCONTROL && !SIMPLEFOOTIK
		void FixedUpdate ()
		{
			DoAnimFollow();
		}
#endif

		public void DoAnimFollow()
		{
			if (userNeedsToAssignStuff)
				return;
			
#if RAGDOLLCONTROL
			ragdollControl.DoRagdollControl();
			if (ragdollControl.stayDeadOnHeadShot && ragdollControl.shotInHead)
				return;
#endif

			totalTorqueError = Vector3.zero;
			totalForceError = Vector3.zero;

			if (frameCounter % secondaryUpdate == 0)
			{
				if (mimicNonRigids)
					for (int i = 2; i < slaveTransforms.Length - 1; i++) // Set all local rotations of the transforms without rigidbodies to match the local rotations of the master
						localRotations2[i] = masterTransforms[i].localRotation;
				SetJointTorque (maxJointTorque, jointDamping);
			}
			if (frameCounter % 2 == 0)
			{
				for (int i = 2; i < slaveTransforms.Length - 1; i++) // Set all local rotations of the transforms without rigidbodies to match the local rotations of the master
				{
					if (secondaryUpdate > 2)
					{
						localRotations1[i] = Quaternion.Lerp(localRotations1[i], localRotations2[i], 2f / secondaryUpdate);
						slaveTransforms[i].localRotation = localRotations1[i];
					}
					else
						slaveTransforms[i].localRotation = localRotations2[i];
				}
			}

			for (int i = 0; i < slaveRigidTransforms.Length; i++) // Do for all rigid bodies
			{
				slaveRigidbodies[i].angularDrag = angularDrag; // Set rigidbody drag and angular drag in real-time
				slaveRigidbodies[i].drag = drag;

				Quaternion targetRotation;
				if (torque) // Calculate and apply world torque
				{
					targetRotation = masterRigidTransforms[i].rotation * Quaternion.Inverse(slaveRigidTransforms[i].rotation);
					targetRotation.ToAngleAxis(out torqueAngle, out torqueAxis);
					torqueError = FixEuler(torqueAngle) * torqueAxis;

					if(torqueAngle != 360f)
					{
						totalTorqueError += torqueError;
						PDControl (PTorque * PTorqueProfile[i], DTorque, out torqueSignal, torqueError, ref torqueLastError[i], reciFixedDeltaTime);
					}
					else
					   torqueSignal = new Vector3(0f, 0f, 0f);

					torqueSignal = Vector3.ClampMagnitude(torqueSignal, maxTorque * maxTorqueProfile[i]);
					slaveRigidbodies[i].AddTorque(torqueSignal, ForceMode.VelocityChange); // Add torque to the limbs
				}

				// Force error
				Vector3 masterRigidTransformsWCOM = masterRigidTransforms[i].position + masterRigidTransforms[i].rotation * rigidbodiesPosToCOM[i];
				forceError = masterRigidTransformsWCOM - slaveRigidTransforms[i].GetComponent<Rigidbody>().worldCenterOfMass; // Doesn't work if collider is trigger
				totalForceError += forceError * forceErrorWeightProfile[i];
				
				if (force) // Calculate and apply world force
				{
					PDControl (PForce * PForceProfile[i], DForce, out forceSignal, forceError, ref forceLastError[i], reciFixedDeltaTime);
					forceSignal = Vector3.ClampMagnitude(forceSignal, maxForce * maxForceProfile[i]);
					slaveRigidbodies[i].AddForce(forceSignal, ForceMode.VelocityChange);
				}
				
				if (i > 0)
					configurableJoints[i].targetRotation = Quaternion.Inverse(localToJointSpace[i]) * Quaternion.Inverse(masterRigidTransforms[i].localRotation) * startLocalRotation[i];
			}
			frameCounter++;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public void SetJointTorque (float positionSpring, float positionDamper)
		{
			for (int i = 1; i < configurableJoints.Length; i++) // Do for all configurable joints
			{
				jointDrive.positionSpring = positionSpring * maxJointTorqueProfile[i];
				jointDrive.positionDamper = positionDamper * jointDampingProfile[i];
				configurableJoints[i].slerpDrive = jointDrive;
			}
			maxJointTorque = positionSpring;
			jointDamping = positionDamper;
		}
		
		public void SetJointTorque (float positionSpring)
		{
			for (int i = 1; i < configurableJoints.Length; i++) // Do for all configurable joints
			{
				jointDrive.positionSpring = positionSpring * maxJointTorqueProfile[i];
				configurableJoints[i].slerpDrive = jointDrive;
			}
			maxJointTorque = positionSpring;
		}
			
		public void EnableJointLimits (bool jointLimits)
		{
			for (int i = 1; i < configurableJoints.Length; i++) // Do for all configurable joints
			{
				if (jointLimits)
				{
					configurableJoints[i].angularXMotion = ConfigurableJointMotion.Limited;
					configurableJoints[i].angularYMotion = ConfigurableJointMotion.Limited;
					configurableJoints[i].angularZMotion = ConfigurableJointMotion.Limited;
				}
				else
				{
					configurableJoints[i].angularXMotion = ConfigurableJointMotion.Free;
					configurableJoints[i].angularYMotion = ConfigurableJointMotion.Free;
					configurableJoints[i].angularZMotion = ConfigurableJointMotion.Free;
				}
			}
		}

		private float FixEuler (float angle) // For the angle in angleAxis, to make the error a scalar
		{
			if (angle > 180f)
				return angle - 360f;
			else
				return angle;
		}
		
		public static void PDControl (float P, float D, out Vector3 signal, Vector3 error, ref Vector3 lastError, float reciDeltaTime) // A PD controller
		{
			// theSignal = P * (theError + D * theDerivative) This is the implemented algorithm.
			signal = P * (error + D * ( error - lastError ) * reciDeltaTime);
			lastError = error;
		}
	}
}