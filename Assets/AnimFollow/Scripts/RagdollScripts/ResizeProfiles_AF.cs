using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AnimFollow
{
	[ExecuteInEditMode]
	public class ResizeProfiles_AF : MonoBehaviour
	{
		public readonly int version = 7; // The version of this script

		AnimFollow_AF animFollow_S;

		void Start ()
		{
			if (animFollow_S = GetComponent<AnimFollow_AF>())
			{
				if (animFollow_S.version != version)
					Debug.LogWarning("AnimFollow on " + this.transform.name + " is not version " + version + " but the ResizeProfiles script is");

				int j = GetComponentsInChildren<Rigidbody>().Length;

				int i = animFollow_S.maxTorqueProfile.Length;
				System.Array.Resize(ref animFollow_S.maxTorqueProfile, j);
				for (int n = 1; n <= j - i ; n++)
					animFollow_S.maxTorqueProfile[j - n] = animFollow_S.maxTorqueProfile[i - 1];

				i = animFollow_S.maxForceProfile.Length;
				System.Array.Resize(ref animFollow_S.maxForceProfile, j);
				for (int n = 1; n <= j - i ; n++)
					animFollow_S.maxForceProfile[j - n] = animFollow_S.maxForceProfile[i - 1];

				i = animFollow_S.maxJointTorqueProfile.Length;
				System.Array.Resize(ref animFollow_S.maxJointTorqueProfile, j);
				for (int n = 1; n <= j - i ; n++)
					animFollow_S.maxJointTorqueProfile[j - n] = animFollow_S.maxJointTorqueProfile[i - 1];

				i = animFollow_S.jointDampingProfile.Length;
				System.Array.Resize(ref animFollow_S.jointDampingProfile, j);
				for (int n = 1; n <= j - i ; n++)
					animFollow_S.jointDampingProfile[j - n] = animFollow_S.jointDampingProfile[i - 1];

				i = animFollow_S.PTorqueProfile.Length;
				System.Array.Resize(ref animFollow_S.PTorqueProfile, j);
				for (int n = 1; n <= j - i ; n++)
					animFollow_S.PTorqueProfile[j - n] = animFollow_S.PTorqueProfile[i - 1];

				i = animFollow_S.PForceProfile.Length;
				System.Array.Resize(ref animFollow_S.PForceProfile, j);
				for (int n = 1; n <= j - i ; n++)
					animFollow_S.PForceProfile[j - n] = animFollow_S.PForceProfile[i - 1];

				i = animFollow_S.forceErrorWeightProfile.Length;
				System.Array.Resize(ref animFollow_S.forceErrorWeightProfile, j);
				for (int n = 1; n <= j - i ; n++)
					animFollow_S.forceErrorWeightProfile[j - n] = animFollow_S.forceErrorWeightProfile[i - 1];
			}
			else
			{
				Debug.LogWarning("There is no AnimFollow script on this game object. \nUnable to resize profiles");
			}
			
			#if UNITY_EDITOR
			EditorUtility.SetDirty(animFollow_S);
			#endif
			DestroyImmediate(this);
		}
	}
}
