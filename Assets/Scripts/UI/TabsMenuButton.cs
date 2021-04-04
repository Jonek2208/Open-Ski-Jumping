using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    public class TabsMenuButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text text;
        [SerializeField] private int buttonIndex;

        public event Action<int> OnButtonClicked;

        public int ButtonIndex
        {
            get => buttonIndex;
            set => buttonIndex = value;
        }

        public void Awake()
        {
            button.onClick.AddListener(() => OnButtonClicked?.Invoke(ButtonIndex));
        }

        public void SetStyle(UISkinData skinData)
        {
            text.color = skinData.color;
        }
    }
}