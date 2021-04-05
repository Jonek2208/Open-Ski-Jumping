using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private List<SelectableSkinSettings> selectables = new List<SelectableSkinSettings>();
        [SerializeField] private List<GraphicSkinSettings> graphics = new List<GraphicSkinSettings>();


        protected void OnSkinUI()
        {
            foreach (var it in selectables.Where(it => it.graphic && it.skinData))
            {
                it.graphic.colors = it.skinData.colors;
            }

            foreach (var it in graphics.Where(it => it.graphic && it.skinData))
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
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                OnSkinUI();
            }
#endif
        }
    }
}