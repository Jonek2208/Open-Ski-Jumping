using System;
using OpenSkiJumping.Hills;
using UnityEngine;

namespace OpenSkiJumping.Jumping
{
    [Serializable]
    public class InrunSimulationData
    {
        
    }

    public static class SimulationUtils
    {
        public static InrunSimulationData SimulateEuler(InrunSimulationData old)
        {
            return new InrunSimulationData();
        } 
    }
    public class JumpingPhysics : MonoBehaviour
    {
        public Hill hill;
        public float k;
        public float rho;
        public int inrunStraightSeg;
        
        public Vector2 SimulateInrunStraight(float inrunLength)
        {
            var res = new Vector2();
            var s = inrunLength - (hill.t + hill.l);
            var ds = s / inrunStraightSeg;
            return res;
        }
    }
}