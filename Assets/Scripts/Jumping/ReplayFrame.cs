using System;
using UnityEngine;

namespace OpenSkiJumping.Jumping
{
    [Serializable]
    public class ReplayFrame
    {
        private Transform[] bodyParts;
        public ReplayFrame(Transform[] value)
        {
            bodyParts = value;
        }
    }
}