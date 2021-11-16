using System;
using OpenSkiJumping.Competition.Runtime;
using OpenSkiJumping.Hills;
using OpenSkiJumping.New;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace OpenSkiJumping.Jumping
{
    public class JudgesController : MonoBehaviour
    {
        public JumperController2 jumperController;
        public Hill hill;
        public Transform hillTransform;

        [SerializeField] private int currentGate;
        [SerializeField] private decimal currentWind;
        public Transform gateTransform;
        public Vector3 jumperPosition;
        public Quaternion jumperRotation;

        // public CompetitorVariable competitorVariable;
        public RuntimeJumpData jumpData;
        public UnityEvent OnPointsGiven;
        public UnityEvent onDistanceMeasurement;
        public UnityEvent onSpeedMeasurement;

        private readonly decimal[] deductions = {0, 0, 0};
        private readonly decimal[] maxDeductions = {5, 5, 7};
        private decimal dist;

        private float fl0, fl1;
        private float dx0, dx1, dx2;

        public bool inEditor;
        private bool showResults;

        void Start()
        {
            if (inEditor)
            {
                PrepareHill();
            }
        }

        public void PrepareHill()
        {
            // Debug.Log("INITIALIZING: " + pd.name);
            // HillInit(pd);
            // jumpUIManager.UIReset();
            // jumpUIManager.SetGateSliderRange(hill.gates);
            // jumpUIManager.SetGateSlider(1);
            // SetGate(1);
            // showResults = false;
        }

        public void PointDeduction(int type, decimal val)
        {
            deductions[type] += val;
            deductions[type] = Math.Min(deductions[type], maxDeductions[type]);
        }

        private decimal[] GetJudgesPoints()
        {
            var model = 20m - (deductions[0] + deductions[1] + deductions[2]) + Random.Range(0, 2) * 0.5m;
            decimal bias = 0;
            if (model < 11.0m)
            {
                bias = 2.5m;
            }
            else if (model < 15.0m)
            {
                bias = 2.0m;
            }
            else if (model < 17.0m)
            {
                bias = 1.5m;
            }
            else if (model < 18.5m)
            {
                bias = 1.0m;
            }
            else
            {
                bias = 0.5m;
            }

            bias *= 2;
            bias += 1;

            decimal[] points = {0, 0, 0, 0, 0};
            for (var i = 0; i < 5; i++)
            {
                points[i] = (decimal) Math.Round(
                    Mathf.Clamp((float) model + (int) (Random.Range(-(float) bias, (float) bias)) * 0.5f, 0f, 20f), 2);
            }

            return points;
        }

        decimal PtsPerMeter(float k)
        {
            if (k < 25) return 4.8m;
            if (k < 30) return 4.4m;
            if (k < 35) return 4.0m;
            if (k < 40) return 3.6m;
            if (k < 50) return 3.2m;
            if (k < 60) return 2.8m;
            if (k < 70) return 2.4m;
            if (k < 80) return 2.2m;
            if (k < 100) return 2.0m;
            if (k < 165) return 1.8m;
            return 1.2m;
        }

        public void Judge()
        {
            var stylePoints = GetJudgesPoints();
            jumpData.JudgesMarks = stylePoints;
            jumpData.Wind = currentWind;
            // decimal total = 0;

            // CompCal.JumpResult jmp = new CompCal.JumpResult(dist, stylePoints, 0, 0, 0);
            // total += jmp.judgesTotalPoints;
            // total += (hill.w >= 165 ? 120 : 60) + (dist - (decimal)hill.w) * PtsPerMeter(hill.w);
            // total = System.Math.Max(0, total);

            // for (int i = 0; i < 5; i++)
            // {
            //     competitorVariable.judgesMarks[i].MarkValue = (float)jmp.judgesMarks[i];
            //     competitorVariable.judgesMarks[i].IsCounted = jmp.judgesMask[i];
            // }
            // competitorVariable.result.Value = (float)total;
            OnPointsGiven.Invoke();
        }

        public void FlightStability(float jumperAngle)
        {
            fl1 = jumperAngle;

            dx2 = fl1 - fl0;

            var eps = 0.2f;
            if (Mathf.Abs(dx2 - dx1) > eps)
            {
                PointDeduction(0, 0.5m);
            }

            fl0 = fl1;
            dx0 = dx1;
            dx1 = dx2;
        }

        public decimal Distance(Vector2[] landingAreaPoints, Vector3 contact)
        {
            contact -= hillTransform.position;

            for (var i = 0; i < landingAreaPoints.Length - 1; i++)
            {
                var diff1 = contact.x - landingAreaPoints[i].x;
                var diff2 = landingAreaPoints[i + 1].x - contact.x;
                if (diff1 >= 0 && diff2 > 0)
                {
                    if (diff1 >= diff2)
                    {
                        return i + 0.5m;
                    }
                    return i;
                }
            }

            return landingAreaPoints.Length;
        }

        public void OnJumperStart()
        {
            jumpData.ResetValues();
        }

        public void OnSpeedMeasurement(float speed)
        {
            jumpData.Speed = (decimal) speed * 3.6m;
            onSpeedMeasurement.Invoke();
        }

        public void OnDistanceMeasurement(Vector3 contact)
        {
            dist = Distance(hill.landingAreaPoints, contact);
            jumpData.Distance = dist;
            onDistanceMeasurement.Invoke();
        }

        public void SetGate(float val)
        {
            var gate = Mathf.RoundToInt(val);
            jumpData.Gate = gate;
            currentGate = gate;
            jumperPosition = new Vector3(hill.GatePoint(gate).x, hill.GatePoint(gate).y, 0);
            jumperRotation.eulerAngles = new Vector3(0, 0, -hill.gamma);
            NewJump();
        }

        public void NewJump()
        {
            jumperController.ResetValues();
            jumperController.GetComponent<Transform>().position =
                hillTransform.TransformPoint(jumperPosition + Vector3.up);
            gateTransform.GetComponent<Transform>().position = hillTransform.TransformPoint(jumperPosition);
            jumperController.GetComponent<Transform>().rotation = jumperRotation;

            fl0 = fl1 = 0;
            dx0 = dx1 = dx2 = 0;
            deductions[0] = deductions[1] = deductions[2] = 0;

            // jumpUIManager.UIReset();
        }

        public void SetWind(float val)
        {
            currentWind = (decimal) val;
            jumperController.windForce = val;
            jumpData.Wind = currentWind;
        }
    }
}