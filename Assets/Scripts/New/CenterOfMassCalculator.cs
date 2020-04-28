using System.Collections.Generic;
using UnityEngine;

namespace OpenSkiJumping.New
{
    public class CenterOfMassCalculator : MonoBehaviour
    {
        public List<Vector3> positions = new List<Vector3>();
        public LineRenderer line;
        public Transform marker;
        public GameObject rootObject;
        private Vector3 comPosition;
        private Rigidbody[] bodies;

        void Start()
        {
            bodies = rootObject.GetComponentsInChildren<Rigidbody>();
        }

        void LateUpdate()
        {
            comPosition = Vector3.zero;
            float massSum = 0;
            foreach (var body in bodies)
            {
                massSum += body.mass;
                comPosition += body.position * body.mass;
            }
            comPosition /= massSum;
            marker.position = comPosition;
            positions.Add(comPosition);
        }

        public void UpdateLine()
        {
            line.positionCount = positions.Count;
            line.SetPositions(positions.ToArray());
        }
    }
}