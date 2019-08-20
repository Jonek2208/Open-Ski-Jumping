using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public class CameraMovement1_AF : MonoBehaviour
	{
		public float movementSmooth = 15f;			// The relative speed at which the camera will catch up.
		public float rotationSmooth = 7f;			// The relative speed at which the camera will catch up.

		public Transform lookAtTransform;
		Vector3 relCameraPos;				// The relative position of the camera from the player.
		Vector3 absCameraPos;				// The position the camera is trying to reach.

		void Awake ()
		{
			if (!lookAtTransform)
			{
				Debug.LogWarning("The lookAtTransform is not assigned on " + this.name); 
				lookAtTransform = this.transform;
			}
			else if (lookAtTransform.root.GetComponentsInChildren<Rigidbody>().Length == 0)
				Debug.Log("The Camera " + this.name + " is looking at a model with no rigid body components.\nIf this is a AnimFollow system it is better to look at the ragdoll");

			// Setting the relative position as the initial relative position of the camera in the scene.
			relCameraPos =  transform.position - lookAtTransform.position;
		}
		
		
		void FixedUpdate () // The camera is not smooth unless in FixedUpdate
		{
			if (!lookAtTransform) // LookAtTransform may have been destroyed by headshot
				return;

			// Lerp the camera's position between it's current position and it's new position.
			Vector3 absCameraPos = lookAtTransform.position + relCameraPos;
			transform.position = Vector3.Lerp(transform.position, absCameraPos, movementSmooth * Time.deltaTime);

			// Make sure the camera is looking at the player.
	//		SmoothLookAt();
		}	
		
		void SmoothLookAt ()
		{
			// Create a vector from the camera towards the player.
			Vector3 relPlayerPosition = lookAtTransform.position + .01f * Vector3.up - transform.position;
	//		Debug.DrawLine(player.position, transform.position);

			// Create a rotation based on the relative position of the player being the forward vector.
			Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
			
			// Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
			transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, rotationSmooth * Time.deltaTime);
		}
	}
}