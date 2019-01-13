using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public class ToggleSlomo_AF : MonoBehaviour
	{
		public float slomoOnKeyN = .3f;
		bool slomo = false;

	//	void Awake () 
	//	{
	//		Debug.Log ("Press the N key to toggle slow motion");
	//	}

		void Update () // The camera is not smooth unless in FixedUpdate
		{
			if (Input.GetKeyDown(KeyCode.N) && !slomo)
			{
				Time.timeScale = slomoOnKeyN;
				slomo = true;
			}
			else if (slomo && Input.GetKeyDown(KeyCode.N))
			{
				Time.timeScale = 1f;
				slomo = false;
			}
		}
	}
}
