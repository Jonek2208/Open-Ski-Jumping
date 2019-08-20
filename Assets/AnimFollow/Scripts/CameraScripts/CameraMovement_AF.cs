using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public class CameraMovement_AF : MonoBehaviour
{
		public float movementSmooth = 1.5f;			// The relative speed at which the camera will catch up.
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

	//		if (Mathf.Abs(Vector3.Dot(lookAtTransform.right, Vector3.up)) < .5)
	//			Debug.Log("The lookAtTransform on " + this.name + " should have its right (red arrow) direction in the vertical up or down direction for the camera to follow properly");

			// Setting the relative position as the initial relative position of the camera in the scene.
			relCameraPos =  new Vector3(Vector3.Dot(transform.position - lookAtTransform.position, lookAtTransform.forward), transform.position.y - lookAtTransform.position.y, Vector3.Dot(transform.position - lookAtTransform.position, lookAtTransform.up));
		}
		
		
		void FixedUpdate () // The camera is not smooth unless in FixedUpdate
		{
			// Lerp the camera's position between it's current position and it's new position.
			Vector3 absCameraPos = lookAtTransform.position + relCameraPos.x * new Vector3(lookAtTransform.forward.x, 0f, lookAtTransform.forward.z).normalized + relCameraPos.y * Vector3.up + relCameraPos.z * new Vector3(lookAtTransform.up.x, 0f, lookAtTransform.up.z).normalized;
			transform.position = Vector3.Lerp(transform.position, absCameraPos, movementSmooth * Time.deltaTime);

			// Make sure the camera is looking at the player.
			SmoothLookAt();
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
