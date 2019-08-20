using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HillProfile;

namespace HillDataSerialization
{
	public enum ProfileType { ICR1992, ICR1996, ICR2008 };
	[System.Serializable]
	public class HillData
	{
		public Vector2[] inrunCoords;
		public Vector2[] landingAreaCords;

		public Vector2[] linesCoords;
		public Vector2[] outrunCoords;
	}
	

	[System.Serializable]
	public class ProfileData
	{
		public string name;
		public float terrainSteepness;
		public Hill.ProfileType type;
	
		public int gates;
		public float w;
		public float hn;
        public float gamma;
		public float alpha;
        public float e;
		public float es;
		public float t;
        public float r1;
        public float betaP;
		public float betaK;
		public float betaL;
        public float s;
		public float l1;
		public float l2;
        public float rL;
		public float r2L;
		public float r2;

		public float a;
		public float rA;
		public float betaA;
		public float b1;
		public float b2;
		public float bK;
		public float bU;
		public float d;
		public float q;
	}

	[System.Serializable]

	public class AllData
	{
		public List<ProfileData> profileData;
	}
}
