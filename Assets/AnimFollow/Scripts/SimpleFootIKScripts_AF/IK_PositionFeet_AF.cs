using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public partial class SimpleFootIK_AF
	{
		[HideInInspector] public Vector3 leftFootPosition;
		[HideInInspector] public Vector3 rightFootPosition;

		void PositionFeet()
		{
			float leftLegTargetLength;
			float rightLegTargetLength;
			float leftKneeAngle;
			float rightKneeAngle;

			// Save before PositionFeet
			Quaternion leftFootRotation = leftFoot.rotation;
			Quaternion rightFootRotation = rightFoot.rotation;
			
			float leftFootElevationInAnim = Vector3.Dot(leftFoot.position - transform.position, transform.up) - footHeight;
			float rightFootElevationInAnim = Vector3.Dot(rightFoot.position - transform.position, transform.up) - footHeight;
			
			// Here goes the maths			
			leftFootTargetNormal = Vector3.Lerp(Vector3.up, raycastHitLeftFoot.normal, footIKWeight);
			leftFootTargetNormal = Vector3.Lerp(lastLeftFootTargetNormal, leftFootTargetNormal, footNormalLerp * deltaTime);
			lastLeftFootTargetNormal = leftFootTargetNormal;
			rightFootTargetNormal = Vector3.Lerp(Vector3.up, raycastHitRightFoot.normal, footIKWeight);
			rightFootTargetNormal = Vector3.Lerp(lastRightFootTargetNormal, rightFootTargetNormal, footNormalLerp * deltaTime);
			lastRightFootTargetNormal = rightFootTargetNormal;
			
			leftFootTargetPos = raycastHitLeftFoot.point;
			leftFootTargetPos = Vector3.Lerp(lastLeftFootTargetPos, leftFootTargetPos, footTargetLerp * deltaTime);
			lastLeftFootTargetPos = leftFootTargetPos;
			leftFootTargetPos = Vector3.Lerp(leftFoot.position, leftFootTargetPos + leftFootTargetNormal * footHeight + leftFootElevationInAnim * Vector3.up, footIKWeight);
			
			rightFootTargetPos = raycastHitRightFoot.point;
			rightFootTargetPos = Vector3.Lerp(lastRightFootTargetPos, rightFootTargetPos, footTargetLerp * deltaTime);
			lastRightFootTargetPos = rightFootTargetPos;
			rightFootTargetPos = Vector3.Lerp(rightFoot.position, rightFootTargetPos + rightFootTargetNormal * footHeight + rightFootElevationInAnim * Vector3.up, footIKWeight);
			
			
			leftLegTargetLength = Mathf.Min((leftFootTargetPos - leftThigh.position).magnitude, calfLength + thighLength - .01f);
			leftLegTargetLength = Mathf.Max(leftLegTargetLength, .2f);
			leftKneeAngle = Mathf.Acos((Mathf.Pow(leftLegTargetLength, 2f) - calfLengthSquared - thighLengthSquared) * reciDenominator);
			leftKneeAngle *= Mathf.Rad2Deg;
			float currKneeAngle;
			Vector3 currKneeAxis;
			Quaternion currKneeRotation = Quaternion.FromToRotation(leftCalf.position - leftThigh.position, leftFoot.position - leftCalf.position);
			currKneeRotation.ToAngleAxis(out currKneeAngle, out currKneeAxis);
			if (currKneeAngle > 180f)
			{
				currKneeAngle = 360f - currKneeAngle;
				currKneeAxis *= -1f;
			}
			leftCalf.Rotate(currKneeAxis, 180f - leftKneeAngle - currKneeAngle, Space.World);
			leftThigh.rotation = Quaternion.FromToRotation(leftFoot.position - leftThigh.position, leftFootTargetPos - leftThigh.position) * leftThigh.rotation;
			
			rightLegTargetLength = Mathf.Min((rightFootTargetPos - rightThigh.position).magnitude, calfLength + thighLength - .01f);
			rightLegTargetLength = Mathf.Max(rightLegTargetLength, .2f);
			rightKneeAngle = Mathf.Acos((Mathf.Pow(rightLegTargetLength, 2f) - calfLengthSquared - thighLengthSquared) * reciDenominator);
			rightKneeAngle *= Mathf.Rad2Deg;
			currKneeRotation = Quaternion.FromToRotation(rightCalf.position - rightThigh.position, rightFoot.position - rightCalf.position);
			currKneeRotation.ToAngleAxis(out currKneeAngle, out currKneeAxis);
			if (currKneeAngle > 180f)
			{
				currKneeAngle = 360f - currKneeAngle;
				currKneeAxis *= -1f;
			}
			rightCalf.Rotate(currKneeAxis, 180f - rightKneeAngle - currKneeAngle, Space.World);
			rightThigh.rotation = Quaternion.FromToRotation(rightFoot.position - rightThigh.position, rightFootTargetPos - rightThigh.position) * rightThigh.rotation;
			
			leftFootPosition = leftFoot.position; // - leftFootTargetNormal * footHeight;
			rightFootPosition = rightFoot.position; // - rightFootTargetNormal * footHeight;
			
			leftFoot.rotation = Quaternion.FromToRotation(transform.up, leftFootTargetNormal) * leftFootRotation;
			rightFoot.rotation = Quaternion.FromToRotation(transform.up, rightFootTargetNormal) * rightFootRotation;
		}
	}
}
