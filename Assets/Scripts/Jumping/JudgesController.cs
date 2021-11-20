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

        private readonly decimal[] _deductions = {0, 0, 0};
        private readonly decimal[] _maxDeductions = {5, 5, 7};
        private decimal _dist;

        private float _fl0, _fl1;
        private float _dx0, _dx1, _dx2;

        public bool inEditor;
        private bool _showResults;

        private void Start()
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
            _deductions[type] += val;
            _deductions[type] = Math.Min(_deductions[type], _maxDeductions[type]);
        }

        private decimal[] GetJudgesPoints()
        {
            var model = 20m - (_deductions[0] + _deductions[1] + _deductions[2]) + Random.Range(0, 2) * 0.5m;
            var bias = model switch
            {
                < 11.0m => 2.5m,
                < 15.0m => 2.0m,
                < 17.0m => 1.5m,
                < 18.5m => 1.0m,
                _ => 0.5m
            };

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

        public void Judge()
        {
            var stylePoints = GetJudgesPoints();
            jumpData.JudgesMarks = stylePoints;
            jumpData.Wind = currentWind;
            OnPointsGiven.Invoke();
        }

        public void FlightStability(float jumperAngle)
        {
            _fl1 = jumperAngle;

            _dx2 = _fl1 - _fl0;

            const float eps = 0.2f;
            if (Mathf.Abs(_dx2 - _dx1) > eps)
            {
                PointDeduction(0, 0.5m);
            }

            _fl0 = _fl1;
            _dx0 = _dx1;
            _dx1 = _dx2;
        }

        public decimal Distance(Vector2[] landingAreaPoints, Vector3 contact)
        {
            contact -= hillTransform.position;

            for (var i = 0; i < landingAreaPoints.Length - 1; i++)
            {
                var diff1 = contact.x - landingAreaPoints[i].x;
                var diff2 = landingAreaPoints[i + 1].x - contact.x;
                if (diff1 < 0 || diff2 <= 0) continue;
                if (diff1 >= diff2)
                {
                    return i + 0.5m;
                }
                return i;
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
            _dist = Distance(hill.landingAreaPoints, contact);
            jumpData.Distance = _dist;
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
            jumperController.transform.position = jumperPosition + Vector3.up;
            gateTransform.transform.position = hillTransform.TransformPoint(jumperPosition);
            jumperController.transform.rotation = jumperRotation;

            _fl0 = _fl1 = 0;
            _dx0 = _dx1 = _dx2 = 0;
            _deductions[0] = _deductions[1] = _deductions[2] = 0;
        }

        public void SetWind(float val)
        {
            currentWind = (decimal) val;
            jumperController.windForce = val;
            jumpData.Wind = currentWind;
        }
    }
}