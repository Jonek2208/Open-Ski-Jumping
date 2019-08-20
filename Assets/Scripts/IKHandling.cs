using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandling : MonoBehaviour {

	Animator anim;

	public float IKWeight = 1.0f;

	public Transform leftIKTarget;
	public Transform rightIKTarget;

	public Transform hintLeft;
	public Transform hintRight;



	void Start () 
	{
		anim = GetComponent<Animator>();
	}
	
	void Update () 
	{
		
	}

	void OnAnimatorIK()
	{
		anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, IKWeight);
		anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, IKWeight);

		anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftIKTarget.position);
		anim.SetIKPosition(AvatarIKGoal.RightFoot, rightIKTarget.position);

		anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, IKWeight);
		anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, IKWeight);

		anim.SetIKHintPosition(AvatarIKHint.LeftKnee, hintLeft.position);
		anim.SetIKHintPosition(AvatarIKHint.RightKnee, hintRight.position);

		anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, IKWeight);
		anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, IKWeight);

		anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftIKTarget.rotation);
		anim.SetIKRotation(AvatarIKGoal.RightFoot, rightIKTarget.rotation);
	}
}
