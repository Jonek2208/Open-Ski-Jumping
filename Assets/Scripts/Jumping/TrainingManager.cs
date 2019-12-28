using System;
using System.Collections.Generic;
using UnityEngine;

using CompCal;
using UnityEngine.Events;

public class TrainingManager : MonoBehaviour
{
    public SkiJumperData currentJumper;
    public FloatVariable leaderPoints;
    public FloatVariable currentJumperPoints;
    public CompetitorVariable competitorVariable;
    public CompetitorVariable competitorVariable2;

    public UnityEvent OnHillChange;
    void Start()
    {
        OnHillChange.Invoke();
    }
}
