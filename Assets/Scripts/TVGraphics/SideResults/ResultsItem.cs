using System;
using UnityEngine;

namespace OpenSkiJumping.TVGraphics.SideResults
{
    [Serializable]
    public class ResultsItem
    {
        public int rank;
        public string nameText;
        public string countryFlagText;
        public Sprite countryFlagImage;
        public decimal result;
    }
}