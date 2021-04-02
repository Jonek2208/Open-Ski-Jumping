using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    [CreateAssetMenu(menuName = "ScriptableObjects/UI/SelectableSkinData")]
    public class SelectableSkinData : ScriptableObject
    {
        public ColorBlock colors = ColorBlock.defaultColorBlock;
    }
}