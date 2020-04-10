using System;
using UnityEngine;

[CreateAssetMenu(menuName = "HillElements/HillsFactory")]
public class HillsFactory : ScriptableObject
{
    public class Wrapper<T>
    {
        [SerializeField] private string name;
        [SerializeField] private T value;

        public string Name { get => name; set => name = value; }
        public T Value { get => value; set => this.value = value; }
    }

    [Serializable] public class InrunTrackWrapper : Wrapper<InrunTrack> { }
    [Serializable] public class GateStairsWrapper : Wrapper<GateStairs> { }
    [Serializable] public class LandingAreaWrapper : Wrapper<LandingArea> { }
    [Serializable] public class GuardrailWrapper : Wrapper<Guardrail> { }


    [SerializeField] private InrunTrackWrapper[] inrunTracks;
    [SerializeField] private GateStairsWrapper[] gateStairs;
    [SerializeField] private LandingAreaWrapper[] landingAreas;
    [SerializeField] private GuardrailWrapper[] guardrails;

}