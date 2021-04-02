using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    [CreateAssetMenu(menuName = "ScriptableObjects/UI/UISkinData")]
    public class UISkinData : ScriptableObject
    {
        public Color color = Color.white;
    }
}