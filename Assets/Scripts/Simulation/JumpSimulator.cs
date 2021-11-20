using System;
using OpenSkiJumping.Hills;
using UnityEngine;

namespace OpenSkiJumping.Simulation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/JumpSimulator")]
    public class JumpSimulator : ScriptableObject
    {
        [SerializeField] private Hill hill;
        [SerializeField] private float takeOffSpeed;
        [SerializeField] private float timeDelta;
        [SerializeField] private float aeroForceScale;


        [SerializeField] private Vector2 gravity = new Vector2(0, -9.81f);

        private const float Eps = 0.1f;

        public void SetHill(Hill newHill) => hill = newHill;

        private Vector2 GetInrunVelocity(Vector2 position)
        {
            var takeOffTableTangent = new Vector2(Mathf.Cos(hill.alphaR), -Mathf.Sin(hill.alphaR));
            var takeOffTableNormalized = new Vector2(Mathf.Sin(hill.alphaR), Mathf.Cos(hill.alphaR));
            var speed = Mathf.Sqrt(2.0f * 9.81f * position.y);
            return takeOffTableTangent * speed + takeOffTableNormalized * takeOffSpeed;
        }

        private Vector2 GetInrunVelocity(int gate) => GetInrunVelocity(hill.GatePoint(gate));

        private static double GetDrag(double angle)
        {
            return 0.001822 + 0.000096017 * angle + 0.00000222578 * angle * angle -
                0.00000018944 * angle * angle * angle + 0.00000000352 * angle * angle * angle * angle;
        }

        private static double GetLift(double angle)
        {
            return 0.000933 + 0.00023314 * angle - 0.00000008201 * angle * angle -
                0.0000001233 * angle * angle * angle + 0.00000000169 * angle * angle * angle * angle;
        }

        private float Distance(Vector2 position)
        {
            for (var i = 0; i < hill.landingAreaPoints.Length - 1; i++)
            {
                var diff1 = position.x - hill.landingAreaPoints[i].x;
                var diff2 = hill.landingAreaPoints[i + 1].x - position.x;
                if (!(diff1 >= 0) || !(diff2 > 0)) continue;
                if (diff1 >= diff2) return i + 0.5f;
                return i;
            }

            return hill.landingAreaPoints.Length;
        }

        private float SimulateJumpWithVelocity(Vector2 startVelocity, float windSpeed)
        {
            var position = new Vector2(0, 0);
            var velocity = startVelocity;
            while (position.y >= hill.LandingArea(position.x))
            {
                var velocityAir = velocity + velocity.normalized * windSpeed;
                var liftVec = new Vector2(-velocityAir.normalized.y, velocityAir.normalized.x);
                var angle = -Mathf.Rad2Deg * Mathf.Atan2(velocityAir.y, velocityAir.x);
                var lift = GetLift(angle);
                var drag = GetDrag(angle);

                var force = -velocityAir.normalized * ((float) drag * velocityAir.sqrMagnitude) +
                            liftVec * ((float) lift * velocityAir.sqrMagnitude);
                force *= aeroForceScale;

                velocity += (force + gravity) * timeDelta;
                position += velocity * timeDelta;
            }

            return Distance(position);
        }

        private float SimulateJump(Vector2 startPosition, float windSpeed)
        {
            var takeOffVelocity = GetInrunVelocity(startPosition);
            return SimulateJumpWithVelocity(takeOffVelocity, windSpeed);
        }

        public int GetGateForWind(float windSpeed)
        {
            var winnerDist = hill.w + hill.l2 * 0.9f;
            int lo = 1, hi = hill.gates;
            // Debug.Log($"Winner dist: {winnerDist}");

            while (lo <= hi)
            {
                var mid = lo + (hi - lo) / 2;
                var inrunVelocity = GetInrunVelocity(mid);
                var dist = SimulateJumpWithVelocity(inrunVelocity, windSpeed);
                // Debug.Log($"{mid} | {inrunVelocity} | {dist}");
                if (dist < winnerDist - Eps)
                    lo = mid + 1;
                else if (winnerDist + Eps < dist)
                    hi = mid - 1;
                else
                    return mid;
            }

            return lo;
        }

        public (float head, float tail, float gate) GetCompensations()
        {
            var initGate = GetGateForWind(0);
            var initGatePos = hill.GatePoint(initGate);
            var initDist = SimulateJump(initGatePos, 0);
            var headWindDist = SimulateJump(initGatePos, 3);
            var tailWindDist = SimulateJump(initGatePos, -3);
            var lowerGatePos = initGatePos - 3 * (hill.A - hill.B) / hill.es;
            var lowerGateDist = SimulateJump(lowerGatePos, 0);
            var headWindFactor = (headWindDist - initDist) / 3.0f;
            var tailWindFactor = (initDist - tailWindDist) / 3.0f;
            var gateFactor = (initDist - lowerGateDist) / 3.0f;
            Debug.Log($"{headWindFactor} | {tailWindFactor} | {gateFactor}");

            return (headWindFactor, tailWindFactor, gateFactor);
        }
    }
}