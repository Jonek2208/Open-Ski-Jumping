#define AUTOASSIGNLEGS
#define AUTOASSIGNFOOTHEIGHT
using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public partial class SimpleFootIK_AF
	{
		// Declare properties

		AnimFollow.AnimFollow_AF animFollow;
		public Transform ragdoll;

		Animator animator;
		public LayerMask layerMask;
		public string[] ignoreLayers = {"Water"};
		float deltaTime;

		RaycastHit raycastHitLeftFoot;
		RaycastHit raycastHitRightFoot;
		RaycastHit raycastHitToe;
		[Range(4f, 20f)] public float raycastLength = 5f; // Character must not be higher above ground than this.
		[Range(.2f, .9f)] public float maxStepHeight = .5f;

		[Range(0f, 1f)] public float footIKWeight = 1f;
		
		[Range(1f, 100f)] public float footNormalLerp = 40f; // Lerp smoothing of foot normals
		[Range(1f, 100f)] public float footTargetLerp = 40f; // Lerp smoothing of foot position
		[Range(1f, 100f)] public float transformYLerp = 20f; // Lerp smoothing of transform following terrain
		[HideInInspector] public float extraYLerp = 1f;	// Used by ragdollControl
		
		[Range(0f, 1f)] public float maxIncline = .8f; // Foot IK not aktiv on inclines steeper than arccos(maxIncline);

		public bool followTerrain = true;
		[HideInInspector] public bool userNeedsToFixStuff = false;

#if AUTOASSIGNFOOTHEIGHT
		float footHeight; // Is set in Awake as the difference between foot positon and transform.position. At Awake the character's transform.position must be level with feet soles.
#else
		public float footHeight; // Set manually in inspector
#endif

#if AUTOASSIGNLEGS		
		Transform leftToe;
		Transform leftFoot;
		Transform leftCalf;
		Transform leftThigh;
		Transform rightToe;
		Transform rightFoot;
		Transform rightCalf;
		Transform rightThigh;
#else
		public Transform leftToe; // Set manually in inspector
		public Transform leftFoot;
		public Transform leftCalf;
		public Transform leftThigh;
		public Transform rightToe;
		public Transform rightFoot;
		public Transform rightCalf;
		public Transform rightThigh;
#endif

		Quaternion leftFootRotation;
		Quaternion rightFootRotation;

		Vector3 leftFootTargetPos;
		Vector3 leftFootTargetNormal;
		Vector3 lastLeftFootTargetPos;
		Vector3 lastLeftFootTargetNormal;
		Vector3 rightFootTargetPos;
		Vector3 rightFootTargetNormal;
		Vector3 lastRightFootTargetPos;
		Vector3 lastRightFootTargetNormal;

		Vector3 footForward;
			 	
		float leftLegTargetLength;
		float rightLegTargetLength;
		float thighLength;
		float thighLengthSquared;
		float calfLength;
		float calfLengthSquared;
		float reciDenominator;

		float leftKneeAngle;
		float leftThighAngle;
		float rightKneeAngle;
		float rightThighAngle;
	}
}
