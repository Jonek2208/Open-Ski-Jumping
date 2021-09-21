using System;
using UnityEngine;

namespace OpenSkiJumping.Hills.TerrainGenerator
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MapLocalTangentPlane")]
    public class MapLocalTangentPlane : ScriptableObject
    {
        public Vector3 centerCoords;
        [SerializeField] private float azimuth;
        [SerializeField] private Vector3 center;
        [SerializeField] private Quaternion rotation;
        

        private void OnEnable()
        {
            SetValues();
        }

        public void SetValues()
        {
            center = Geodetic.Geog2Ecef(centerCoords, false);
            rotation = Geodetic.Rotation(centerCoords.x, centerCoords.y, azimuth, false);
        }


        public Vector3 FromGeog(Vector3 point)
        {
            return rotation * (Geodetic.Geog2Ecef(point, false) - center);
        }

        public Vector3 ToGeog(Vector3 point)
        {
            return Geodetic.Ecef2Geog(Quaternion.Inverse(rotation) * point + center, false);
        }
    }
}