using UnityEngine;
using System.Collections;

namespace CTI {

	[RequireComponent (typeof (WindZone))]
	public class CTI_CustomWind : MonoBehaviour {

		private WindZone m_WindZone;

		private Vector3 WindDirection;
		private float WindStrength;
		private float WindTurbulence;

	    public float WindMultiplier = 1.0f;

	    private bool init = false;
	    private int TerrainLODWindPID;

	    void Init () {
			m_WindZone = GetComponent<WindZone>();
			TerrainLODWindPID = Shader.PropertyToID("_TerrainLODWind");
		}

		void OnValidate () {
			Update ();
		}
		
		void Update () {
			if (!init) {
				Init ();
			}
			WindDirection = this.transform.forward;

			if(m_WindZone == null) {
				m_WindZone = GetComponent<WindZone>();
			}
			WindStrength = m_WindZone.windMain * WindMultiplier;
			WindStrength += m_WindZone.windPulseMagnitude * (1.0f + Mathf.Sin(Time.time * m_WindZone.windPulseFrequency) + 1.0f + Mathf.Sin(Time.time * m_WindZone.windPulseFrequency * 3.0f) ) * 0.5f;
			WindTurbulence = m_WindZone.windTurbulence * m_WindZone.windMain * WindMultiplier;

			WindDirection.x *= WindStrength;
			WindDirection.y *= WindStrength;
			WindDirection.z *= WindStrength;

			Shader.SetGlobalVector(TerrainLODWindPID, new Vector4(WindDirection.x, WindDirection.y, WindDirection.z, WindTurbulence) );
		}
	}
}