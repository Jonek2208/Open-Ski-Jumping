using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	[RequireComponent(typeof(Rigidbody))]

	public class BallTest_AF : MonoBehaviour
	{
		public Transform hitTransform; // Set this to the transform you want the ball to hit when you press key "b"
		public float ballVelocity = 20f;
		public float massOfBall = 40f;
	//	public float scaleOfBall = .4f;

		void Awake ()
		{
			GetComponent<Rigidbody>().isKinematic = false;
	//		transform.localScale = Vector3.one * scaleOfBall;
			if (!hitTransform)
			{
				Debug.LogWarning("hitTransform on " + this.name + " is not assigned.");
			}
	//		else
	//			Debug.Log ("Press the B key to launch " + this.name + " towards " + hitTransform.name);
		}

		void Update ()
		{
			if (Input.GetKeyDown(KeyCode.B)) // if pressing key "b"
			{
				if (!hitTransform)
				{
					Debug.LogWarning("hitTransform on " + this.name + " is not assigned.");
					return;
				}
				GetComponent<Rigidbody>().mass = massOfBall;
				GetComponent<Rigidbody>().useGravity = false;
				GetComponent<Rigidbody>().velocity = (hitTransform.position - transform.position).normalized * ballVelocity; // Hurl ball towards hit transform
			}
		}
		
		void OnCollisionEnter (Collision collision)
		{
			GetComponent<Rigidbody>().useGravity = true; // Turn gravity on for the ball after the ball has hit something.
		}
	}
}
