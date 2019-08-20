using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HillProfile;

public class JudgesController : MonoBehaviour
{
    void Start()
    {

    }


    public float Distance(Vector2[] landingAreaPoints, Vector3 contact)
    {
        for (int i = 0; i < landingAreaPoints.Length - 1; i++)
        {
            float diff1 = contact.x - landingAreaPoints[i].x;
            float diff2 = landingAreaPoints[i+1].x - contact.x;
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
}
