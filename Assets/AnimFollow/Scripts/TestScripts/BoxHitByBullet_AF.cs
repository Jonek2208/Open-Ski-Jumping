using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public class BoxHitByBullet_AF : MonoBehaviour
	{
		// This script should be added to the root containing the box

		void HitByBullet (BulletHitInfo_AF bulletHitInfo)
		{
			bulletHitInfo.hitTransform.GetComponent<Rigidbody>().AddForceAtPosition(bulletHitInfo.bulletForce, bulletHitInfo.hitPoint);
		}
	}
}
