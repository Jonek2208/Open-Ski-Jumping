using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public class Limb_AF : MonoBehaviour
	{
		public readonly int version = 7; // The version of this script

		// This script is distributed (automatically by RagdollControl) to all rigidbodies and reports to the RagdollControl script if any limb is currently colliding.

		RagdollControl_AF ragdollControl;
		string[] ignoreCollidersWithTag;
			
		void OnEnable()
		{
			ragdollControl = transform.root.GetComponentInChildren<RagdollControl_AF>();
			ignoreCollidersWithTag  = ragdollControl.ignoreCollidersWithTag;
		}
		
		void OnCollisionEnter(Collision collision)
		{
			bool ignore = false;
			if (!(collision.transform.name == "Terrain") && collision.transform.root != this.transform.root)
			{
				foreach (string ignoreTag in ignoreCollidersWithTag)
				{
					if (collision.transform.tag == ignoreTag)
					{
						ignore = true;
						break;
					}
				}

				if (!ignore)
				{
					ragdollControl.numberOfCollisions++;
					ragdollControl.collisionSpeed = collision.relativeVelocity.magnitude;
//					Debug.Log (collision.transform.name + "\nincreasing");
				}
			}
		}
		
		void OnCollisionExit(Collision collision)
		{
			bool ignore = false;
			if (!(collision.transform.name == "Terrain") && collision.transform.root != this.transform.root)
			{
				foreach (string ignoreTag in ignoreCollidersWithTag)
				{
					if (collision.transform.tag == ignoreTag)
					{
						ignore = true;
						break;
					}
				}

				if (!ignore)
				{
					ragdollControl.numberOfCollisions--;
	//				Debug.Log (collision.transform.name + "\ndecreasing");
				}
			}
		}
	}
}
