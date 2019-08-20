#define AUTOASSIGNLEGS
using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public partial class SimpleFootIK_AF
	{
		void Awake2()
		{
			foreach (string ignoreLayer in ignoreLayers)
			{
				layerMask = layerMask | (1 << LayerMask.NameToLayer(ignoreLayer)); // Use to avoid IK raycasts to hit colliders on the character (ragdoll must be on an ignored layer)
			}
			layerMask = ~layerMask;

			if (!ragdoll)
			{
				Debug.LogWarning("ragdoll not assigned in SimpleFootIK script on " + this.name + "\nThis Foot IK is for use with an AnimFollow system" + "\n");
				userNeedsToFixStuff = true;
			}
			else
			{
				if (!(animFollow = ragdoll.GetComponent<AnimFollow.AnimFollow_AF>()))
				{
					Debug.LogWarning("Missing script: AnimFollow on " + ragdoll.name + "\nThis Foot IK is for use with an AnimFollow system" + "\n");
					userNeedsToFixStuff = true;
				}

				bool ragdollOnIgnoredLayer = false;
				foreach (string ignoreLayer in ignoreLayers)
				{
					if (ragdoll.gameObject.layer.Equals(LayerMask.NameToLayer(ignoreLayer)))
					{
						ragdollOnIgnoredLayer = true;
						break;
					}
				}

				if (!ragdollOnIgnoredLayer)
				{
					Debug.LogWarning("Layer for " + ragdoll.name + " and its children must be set to an ignored layer" + "\n");
					userNeedsToFixStuff = true;
				}
			}

#if AUTOASSIGNLEGS
			// For the auto assigning to work the characters legs must be the same transform structure as Ethan in the example scene and
			// the character should be humanoid with feets named something like RightFoot and LeftFoot.
			Transform[] characterTransforms = GetComponentsInChildren<Transform>();
			for (int n = 0; n < characterTransforms.Length; n++)
			{
				if ((characterTransforms[n].name.ToLower().Contains("foot") && characterTransforms[n].name.ToLower().Contains("left")))
				{
					leftToe = characterTransforms[n + 1];
					leftFoot = characterTransforms[n];
					leftCalf = characterTransforms[n - 1];
					leftThigh = characterTransforms[n - 2];
					if (rightFoot)
						break;
				}
				if (characterTransforms[n].name.ToLower().Contains("foot") && characterTransforms[n].name.ToLower().Contains("right"))
				{
					rightToe = characterTransforms[n + 1];
					rightFoot = characterTransforms[n];
					rightCalf = characterTransforms[n - 1];
					rightThigh = characterTransforms[n - 2];
					if (leftFoot)
						break;
				}
			}
			if (!(leftToe && rightToe))
			{
				Debug.LogWarning("Auto assigning of legs failed. Look at lines 32-57 in script IK_Setup" + "\n");
				userNeedsToFixStuff = true;
				return;
			}
#endif
			
			thighLength = (rightThigh.position - rightCalf.position).magnitude;
			thighLengthSquared = (rightThigh.position - rightCalf.position).sqrMagnitude;
			calfLength = (rightCalf.position - rightFoot.position).magnitude;
			calfLengthSquared = (rightCalf.position - rightFoot.position).sqrMagnitude;
			reciDenominator = -.5f / calfLength / thighLength;

#if AUTOASSIGNFOOTHEIGHT 
			// Character should be spawned upright (line from feets to head points as vector3.up)
			footHeight = (rightFoot.position.y + leftFoot.position.y) * .5f - transform.position.y;
#else
			if (footHeight == 0f)
				footHeight = .132f;
#endif
		}
	}
}
