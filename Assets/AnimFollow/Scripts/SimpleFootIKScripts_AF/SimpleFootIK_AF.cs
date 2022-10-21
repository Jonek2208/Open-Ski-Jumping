using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public partial class SimpleFootIK_AF : MonoBehaviour
	{	
		void Awake()
		{
			Awake2();
		}

		void FixedUpdate()
		{
			deltaTime = Time.fixedDeltaTime;
			DoSimpleFootIK();
		}

		void DoSimpleFootIK()
		{	
			if (userNeedsToFixStuff)
			{
				animFollow.DoAnimFollow(); // Only here to make the dead on headshot feature work properly
				return;
			}

			ShootIKRays();

			PositionFeet();

			animFollow.DoAnimFollow();
		}
	}
}