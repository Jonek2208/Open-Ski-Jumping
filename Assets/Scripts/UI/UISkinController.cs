using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    [Serializable]
    public class SelectableSkinSettings
    {
        public Selectable graphic;
        public SelectableSkinData skinData;
    }

    [Serializable]
    public class GraphicSkinSettings
    {
        public Graphic graphic;
        public UISkinData skinData;
    }

    [ExecuteInEditMode]
    public class UISkinController : MonoBehaviour
    {
        [SerializeField] private SelectableSkinSettings[] selectables;
        [SerializeField] private GraphicSkinSettings[] graphics;


        protected void OnSkinUI()
        {
            foreach (var it in selectables)
            {
                it.graphic.colors = it.skinData.colors;
            }

            foreach (var it in graphics)
            {
                it.graphic.color = it.skinData.color;
            }
        }

        protected void Awake()
        {
            OnSkinUI();
        }

        protected void Update()
        {
            if (Application.isEditor)
            {
                OnSkinUI();
            }
        }
    }
    
}