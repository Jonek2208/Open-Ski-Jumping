using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HillProfile;

public class JudgesController : MonoBehaviour
{
    public enum JumpState
    {
        Stairs, Gate, Inrun, Flight, LandingArea, Brake
    }

    JumpState state;

    public Hill hill;
    public MeshScript meshScript;
    public JumpUIManager jumpUIManager;
    // public CompetitionManager competitionManager;
    public JumperController2 jumperController;
    public GameObject gateObject;
    public Vector3 jumperPosition;
    public Quaternion jumperRotation;

    private float[] deductions = { 0, 0, 0 };
    private float[] maxDeductions = { 5, 5, 7 };
    private float dist;

    private float fl0, fl1;
    private float dx0, dx1, dx2;

    public bool inEditor;
    private bool showResults;

    void Start()
    {
        HillInit();
        jumpUIManager.UIReset();
        jumpUIManager.SetGateSliderRange(hill.gates);
        jumpUIManager.SetGateSlider(1);
        SetGate(1);
        showResults = false;
    }

    public void PointDeduction(int type, float val)
    {
        deductions[type] += val;
        deductions[type] = Mathf.Min(deductions[type], maxDeductions[type]);
    }

    private float[] GetJudgesPoints()
    {
        float model = 20 - (deductions[0] + deductions[1] + deductions[2]) + (int)(Random.Range(0, 2)) * 0.5f;
        float bias = 0;
        if (model < 11.0f)
        {
            bias = 2.5f;
        }
        else if (model < 15.0f)
        {
            bias = 2.0f;
        }
        else if (model < 17.0f)
        {
            bias = 1.5f;
        }
        else if (model < 18.5f)
        {
            bias = 1.0f;
        }
        else
        {
            bias = 0.5f;
        }
        bias *= 2;
        bias += 1;

        float[] points = { 0, 0, 0, 0, 0 };
        for (int i = 0; i < 5; i++)
        {
            points[i] = Mathf.Clamp(model + (int)(Random.Range(-bias, bias)) * 0.5f, 0, 20);
        }

        return points;
    }

    float PtsPerMeter(float k)
    {
        if (k < 25) return 4.8f;
        else if (k < 30) return 4.4f;
        else if (k < 35) return 4.0f;
        else if (k < 40) return 3.6f;
        else if (k < 50) return 3.2f;
        else if (k < 60) return 2.8f;
        else if (k < 70) return 2.4f;
        else if (k < 80) return 2.2f;
        else if (k < 100) return 2.0f;
        else if (k < 165) return 1.8f;
        else return 1.2f;
    }

    public void Judge()
    {
        float[] stylePoints = GetJudgesPoints();
        bool[] valid = new bool[5];
        float minPts = 20, maxPts = 0;
        int lo = 0, hi = 0;

        float total = 0;

        for (int i = 0; i < 5; i++)
        {
            total += stylePoints[i];
            if (maxPts <= stylePoints[i])
            {
                maxPts = stylePoints[i];
                hi = i;
            }
        }

        for (int i = 0; i < 5; i++)
        {
            if (i != hi && minPts >= stylePoints[i])
            {
                minPts = stylePoints[i];
                lo = i;
            }
        }

        total -= stylePoints[lo] + stylePoints[hi];
        total += (hill.w >= 165 ? 120 : 60) + (dist - hill.w) * PtsPerMeter(hill.w);
        total = Mathf.Max(0, total);

        jumpUIManager.SetPoints(stylePoints, lo, hi, total);
    }

    public void FlightStability(float jumperAngle)
    {
        fl1 = jumperAngle;

        dx2 = fl1 - fl0;

        float eps = 0.2f;
        if (Mathf.Abs(dx2 - dx1) > eps)
        {
            PointDeduction(0, 0.5f);
        }
        fl0 = fl1;
        dx0 = dx1;
        dx1 = dx2;
    }


    public void HillInit()
    {
        hill = new Hill(meshScript.profileData);
        // Debug.Log(hill.w);
        hill.Calculate();
    }

    public float Distance(Vector2[] landingAreaPoints, Vector3 contact)
    {
        for (int i = 0; i < landingAreaPoints.Length - 1; i++)
        {
            float diff1 = contact.x - landingAreaPoints[i].x;
            float diff2 = landingAreaPoints[i + 1].x - contact.x;
            if (diff1 >= 0 && diff2 > 0)
            {
                if (diff1 >= diff2)
                {
                    return (float)(i) + 0.5f;
                }
                else
                {
                    return i;
                }
            }
        }
        return landingAreaPoints.Length;
    }


    public void SpeedMeasurement(float speed)
    {
        jumpUIManager.SetSpeed(speed * 3.6f);
    }

    public void DistanceMeasurement(Vector3 contact)
    {
        dist = Distance(meshScript.landingAreaPoints, contact);
        jumpUIManager.SetDistance(dist);
    }

    public void SetGate(int gate)
    {
        jumperPosition = new Vector3(hill.GatePoint(gate).x, hill.GatePoint(gate).y, 0);
        jumperRotation.eulerAngles = new Vector3(0, 0, -hill.gamma);
    }

    public void NewJump()
    {

        state = JumpState.Gate;
        jumperController.ResetValues();
        jumperController.GetComponent<Transform>().position = jumperPosition;
        gateObject.GetComponent<Transform>().position = jumperPosition;
        jumperController.GetComponent<Transform>().rotation = jumperRotation;

        fl0 = fl1 = 0;
        dx0 = dx1 = dx2 = 0;
        deductions[0] = deductions[1] = deductions[2] = 0;

        jumpUIManager.UIReset();

    }

    public void SetWind(float val)
    {
        jumperController.windForce = val;
    }

}
