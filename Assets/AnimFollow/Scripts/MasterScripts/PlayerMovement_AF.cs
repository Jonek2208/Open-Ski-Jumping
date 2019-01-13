using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public class PlayerMovement_AF : MonoBehaviour
	{
		// Add this script to the master

		public readonly int version = 7; // The version of this script

		Animator anim;			// Reference to the animator component.
		HashIDs_AF hash;			// Reference to the HashIDs.

		public float animatorSpeed = 1.3f; // Read by RagdollControl
		public float speedDampTime = .1f;	// The damping for the speed parameter
		float mouseInput;
		public float mouseSensitivityX = 100f;
		public bool inhibitMove = false; // Set from RagdollControl
		[HideInInspector] public Vector3 glideFree = Vector3.zero; // Set from RagdollControl
		Vector3 glideFree2 = Vector3.zero;
		[HideInInspector] public bool inhibitRun = false; // Set from RagdollControl

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void Awake ()
		{
			// Setting up the references.
			if (!(anim = GetComponent<Animator>()))
			{
				Debug.LogWarning("Missing Animator on " + this.name);
				inhibitMove = true;
			}
			if (!(hash = GetComponent<HashIDs_AF>()))
			{
				Debug.LogWarning("Missing Script: HashIDs on " + this.name);
				inhibitMove = true;
			}
			if (anim.avatar)
				if (!anim.avatar.isValid)
					Debug.LogWarning("Animator avatar is not valid");
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		void OnAnimatorMove ()
		{
			glideFree2 = Vector3.Lerp (glideFree2, glideFree, .05f);
			transform.position += anim.deltaPosition + glideFree2;
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		
		void FixedUpdate ()
		{
			if (inhibitMove)
				return;

			transform.Rotate(0f, Input.GetAxis("Mouse X") * mouseSensitivityX * Time.fixedDeltaTime, 0f);

			MovementManagement(Input.GetAxis("Vertical"), Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftControl));
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public void MovementManagement (float vertical, bool walk, bool sneaking)
		{
			walk = walk || inhibitRun;
			// Set the sneaking parameter to the sneak input.
			anim.SetBool(hash.sneakingBool, sneaking);
			
			// If there is some axis input...
			if(vertical >= .1f && !walk)
			{
				// ... set the speed parameter to 5.5f.
				anim.SetFloat(hash.speedFloat, 5.5f, speedDampTime, Time.fixedDeltaTime);
			}
			else if(vertical >= .1f && walk)
			{
				// ... set the speed parameter to 5.5f.
				anim.SetFloat(hash.speedFloat, 2.5f, speedDampTime, Time.fixedDeltaTime);
			}
			else if(vertical <= -.1f)
			{
				// ... set the speed parameter to -3f.
				anim.SetFloat(hash.speedFloat, -3f, speedDampTime, Time.fixedDeltaTime);
			}
			else
				// Otherwise set the speed parameter to 0.
				anim.SetFloat(hash.speedFloat, 0, speedDampTime, Time.fixedDeltaTime);
		}
	}
}
