using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	[RequireComponent(typeof(AudioSource))]
	public class ShootStuff_AF : MonoBehaviour
	{
		public Camera theCamera;
		Rect guiBox = new Rect(5, 5, 160, 120);
		public Texture crosshairTexture;
		RaycastHit raycastHit;
		public float bulletForce = 8000f;

		bool userNeedsToFixStuff = false;

		void Awake ()
		{
			if (!theCamera)
			{
				Debug.LogWarning("You need to assign a camera to the ShootStuff script on " + this.name);
				userNeedsToFixStuff = true;
			}
			else if (!crosshairTexture)
			{
				Debug.LogWarning("You need to assign crosshairTexture in the ShootStuff script on " + this.name);
				userNeedsToFixStuff = true;
			}
			else
				Cursor.visible = false;

			if (GetComponent<AudioSource>().clip == null)
				Debug.LogWarning("Assign audio clip to audiosource on " + this.name + "\n");
		}

		void Update ()
		{
			if (userNeedsToFixStuff)
				return;

			if (Input.GetMouseButton(1) && !guiBox.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
			{
				theCamera.fieldOfView = 30f;
			}
			else
			{
				theCamera.fieldOfView = 60f;
			}

			if (Input.GetMouseButtonDown(0) && !guiBox.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
			{
				GetComponent<AudioSource>().Play();
				Ray rayen = theCamera.ScreenPointToRay(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
				if (Physics.Raycast(rayen, out raycastHit, 100f))
				{
					BulletHitInfo_AF bulletHitInfo = new BulletHitInfo_AF();
					bulletHitInfo.hitTransform = raycastHit.transform;
					bulletHitInfo.bulletForce = (raycastHit.point - transform.position).normalized * bulletForce;
					bulletHitInfo.hitNormal = raycastHit.normal;
					bulletHitInfo.hitPoint = raycastHit.point;

					raycastHit.transform.root.SendMessage("HitByBullet", bulletHitInfo, SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		void OnGUI ()
		{
			if (userNeedsToFixStuff)
				return;

			GUI.DrawTexture(new Rect(Input.mousePosition.x - 20, Screen.height - Input.mousePosition.y - 20, 40, 40), crosshairTexture, ScaleMode.ScaleToFit, true);
			GUI.Box(guiBox, "Fire = Left mouse\nB = Launch ball\nN = Slow motion\nZoom = Right mouse\n\nBullet force");
			bulletForce = GUI.HorizontalSlider(new Rect(10, 105, 150, 15), bulletForce, 1000f, 20000f);
		}
	}
}

