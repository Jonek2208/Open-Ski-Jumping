using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollScript : MonoBehaviour {

	void SetKinematic(bool val)
	{
		Rigidbody [] bodies = GetComponentsInChildren<Rigidbody>();

		foreach(Rigidbody rb in bodies)
		{
			rb.isKinematic = val;
		}
	}
	void Start () 
	{
		SetKinematic(true);
	}

	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			SetKinematic(false);
			GetComponent<Animator>().enabled = false;
		}
	}
}
