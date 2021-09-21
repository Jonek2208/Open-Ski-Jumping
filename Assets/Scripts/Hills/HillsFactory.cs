using System;
using OpenSkiJumping.Hills.Guardrails;
using OpenSkiJumping.Hills.InrunTracks;
using OpenSkiJumping.Hills.LandingAreas;
using OpenSkiJumping.Hills.StairsOld;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    [CreateAssetMenu(menuName = "HillElements/HillsFactory")]
    public class HillsFactory : ScriptableObject
    {
        public GateStairsWrapper[] gateStairs;
        public GuardrailWrapper[] guardrails;


        public InrunTrackWrapper[] inrunTracks;
        public LandingAreaWrapper[] landingAreas;

        public class Wrapper<T>
        {
            [SerializeField] private string name;
            [SerializeField] private T value;

            public string Name
            {
                get => name;
                set => name = value;
            }

            public T Value
            {
                get => value;
                set => this.value = value;
            }
        }

        [Serializable]
        public class InrunTrackWrapper : Wrapper<InrunTrack>
        {
        }

        [Serializable]
        public class GateStairsWrapper : Wrapper<GateStairs>
        {
        }

        [Serializable]
        public class LandingAreaWrapper : Wrapper<LandingArea>
        {
        }

        [Serializable]
        public class GuardrailWrapper : Wrapper<Guardrail>
        {
        }
    }
}