using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public class ragdollHitByBullet_AF : MonoBehaviour
	{
		// This script should be added to the root containing the ragdoll

		RagdollControl_AF ragdollControl;

		public ParticleSystem blood;
		public ParticleSystem bloodClone;
		bool userNeedsToFixStuff = false;

		void Awake ()
		{
			if (!blood)
			{
				Debug.LogWarning("You need to assign blood prefab in the ragdollHitByBullet script on " + this.name);
				userNeedsToFixStuff = true;
			}
			if (!(ragdollControl = GetComponentInChildren<RagdollControl_AF>()))
			{
				Debug.LogWarning("The ragdollHitByBullet script on " + this.name + " requires a RagdollControl script to work");
				userNeedsToFixStuff = true;
			}
		}

		void HitByBullet (BulletHitInfo_AF bulletHitInfo)
		{
			if (userNeedsToFixStuff)
				return;

			bloodClone = Instantiate(blood, bulletHitInfo.hitPoint, Quaternion.LookRotation(bulletHitInfo.hitNormal)) as ParticleSystem;
			ragdollControl.shotByBullet = true;
			bloodClone.transform.parent = bulletHitInfo.hitTransform;
			bloodClone.Play();
			Destroy(bloodClone.gameObject, 1f);

			if (bulletHitInfo.hitTransform.name.Contains("Head"))
				ragdollControl.shotInHead = true;

			StartCoroutine(AddForceToLimb(bulletHitInfo));
		}

		IEnumerator AddForceToLimb (BulletHitInfo_AF bulletHitInfo)
		{
			yield return new WaitForFixedUpdate();
			bulletHitInfo.hitTransform.GetComponent<Rigidbody>().AddForceAtPosition(bulletHitInfo.bulletForce, bulletHitInfo.hitPoint);

		}
	}
}
