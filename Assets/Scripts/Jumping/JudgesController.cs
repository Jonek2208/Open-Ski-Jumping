using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HillProfile;
using UnityEngine.Events;

public class JudgesController : MonoBehaviour
{
    public Hill hill;
    public MeshScript meshScript;
    public JumpUIManager jumpUIManager;
    public CompetitionManager competitionManager;
    public JumperController2 jumperController;
    public GameObject gateObject;
    public Vector3 jumperPosition;
    public Quaternion jumperRotation;

    public CompetitorVariable competitorVariable;
    public StringVariable hillName;
    public UnityEvent OnShowResults;

    private decimal[] deductions = { 0, 0, 0 };
    private decimal[] maxDeductions = { 5, 5, 7 };
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

    public void PrepareHill(ProfileData pd = null)
    {
        Debug.Log("INITIALIZING: " + pd.name);
        HillInit(pd);
        jumpUIManager.UIReset();
        jumpUIManager.SetGateSliderRange(hill.gates);
        jumpUIManager.SetGateSlider(1);
        SetGate(1);
        showResults = false;
    }

    public void PointDeduction(int type, decimal val)
    {
        deductions[type] += val;
        deductions[type] = System.Math.Min(deductions[type], maxDeductions[type]);
    }

    private decimal[] GetJudgesPoints()
    {
        decimal model = 20m - (deductions[0] + deductions[1] + deductions[2]) + (int)(Random.Range(0, 2)) * 0.5m;
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

        decimal[] points = { 0, 0, 0, 0, 0 };
        for (int i = 0; i < 5; i++)
        {
            points[i] = (decimal)System.Math.Round(Mathf.Clamp((float)model + (int)(Random.Range(-(float)bias, (float)bias)) * 0.5f, 0f, 20f), 2);
        }

        return points;
    }

    decimal PtsPerMeter(float k)
    {
        if (k < 25) return 4.8m;
        else if (k < 30) return 4.4m;
        else if (k < 35) return 4.0m;
        else if (k < 40) return 3.6m;
        else if (k < 50) return 3.2m;
        else if (k < 60) return 2.8m;
        else if (k < 70) return 2.4m;
        else if (k < 80) return 2.2m;
        else if (k < 100) return 2.0m;
        else if (k < 165) return 1.8m;
        else return 1.2m;
    }

    public void Judge()
    {
        decimal[] stylePoints = GetJudgesPoints();
        decimal total = 0;

        CompCal.JumpResult jmp = new CompCal.JumpResult(dist, stylePoints, 0, 0, 0);
        total += jmp.judgesTotalPoints;
        total += (hill.w >= 165 ? 120 : 60) + (dist - (decimal)hill.w) * PtsPerMeter(hill.w);
        total = System.Math.Max(0, total);
        for (int i = 0; i < 5; i++)
        {
            competitorVariable.judgesMarks[i].MarkValue = (float)jmp.judgesMarks[i];
            competitorVariable.judgesMarks[i].IsCounted = jmp.judgesMask[i];
        }
    }

    public void FlightStability(float jumperAngle)
    {
        fl1 = jumperAngle;

        dx2 = fl1 - fl0;

        float eps = 0.2f;
        if (Mathf.Abs(dx2 - dx1) > eps)
        {
            PointDeduction(0, 0.5m);
        }
        fl0 = fl1;
        dx0 = dx1;
        dx1 = dx2;
    }

    public void HillInit(ProfileData pd = null)
    {
        if (pd == null)
        {
            pd = meshScript.profileData;
        }

        // Temporary - until hillsdatabase & editor will be updated
        pd.a = 100; pd.rA = 0; pd.betaA = 0; pd.b1 = 2.5f; pd.b2 = 10; pd.bK = 20; pd.bU = 25;

        hill = new Hill(pd);

        hillName.Value = pd.name;
        hill.Calculate();
        meshScript.profileData = pd;
        meshScript.GenerateMesh();
    }

    public decimal Distance(Vector2[] landingAreaPoints, Vector3 contact)
    {
        for (int i = 0; i < landingAreaPoints.Length - 1; i++)
        {
            float diff1 = contact.x - landingAreaPoints[i].x;
            float diff2 = landingAreaPoints[i + 1].x - contact.x;
            if (diff1 >= 0 && diff2 > 0)
            {
                if (diff1 >= diff2)
                {
                    return (decimal)(i) + 0.5m;
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
        competitorVariable.speed.Value = speed * 3.6f;
    }

    public void DistanceMeasurement(Vector3 contact)
    {
        dist = Distance(meshScript.landingAreaPoints, contact);
        competitorVariable.distance.Value = (float)dist;
    }

    public void SetGate(int gate)
    {
        jumperPosition = new Vector3(hill.GatePoint(gate).x, hill.GatePoint(gate).y, 0);
        jumperRotation.eulerAngles = new Vector3(0, 0, -hill.gamma);
        NewJump();
    }

    public void NewJump()
    {
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
