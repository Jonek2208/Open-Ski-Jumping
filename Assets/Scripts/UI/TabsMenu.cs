using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    public class TabsMenu : MonoBehaviour
    {
        [SerializeField] private List<TabsMenuButton> tabButtons;
        [SerializeField] private List<GameObject> menus;

        [SerializeField] private UISkinData nonActive;
        [SerializeField] private UISkinData active;

        [SerializeField] private int selectedIndex;
        [SerializeField] private float transitionDuration;
        [SerializeField] private Image activeTabIndicator;

        private RectTransform rectTransform;
        private Vector2 sizeDelta;
        private Vector3 pos;
        private Vector2 sizeDeltaBtn;
        private Vector3 posDeltaBtn;

        private void Awake()
        {
        }

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            sizeDelta = rectTransform.rect.size;
            pos = rectTransform.anchoredPosition3D;
            sizeDeltaBtn = new Vector2(sizeDelta.x / tabButtons.Count, sizeDelta.y);
            posDeltaBtn = new Vector3(sizeDeltaBtn.x, 0, 0);

            SetUpButtonsSize();
            for (var i = 0; i < tabButtons.Count; i++)
            {
                var btn = tabButtons[i];
                btn.ButtonIndex = i;
                btn.OnButtonClicked += HandleSelectionChange;
                btn.SetStyle(i == selectedIndex ? active : nonActive);
            }

            for (var i = 0; i < menus.Count; i++) menus[i].SetActive(selectedIndex == i);
        }

        public void HandleSelectionChange(int index)
        {
            if (index == selectedIndex) return;
            selectedIndex = index;
            for (var i = 0; i < menus.Count; i++)
            {
                menus[i].SetActive(selectedIndex == i);
                tabButtons[i].SetStyle(i == selectedIndex ? active : nonActive);
            }

            activeTabIndicator.rectTransform.DOAnchorPos3D(posDeltaBtn * selectedIndex,
                transitionDuration);
        }

        private void SetUpButtonsSize()
        {
            for (var i = 0; i < tabButtons.Count; i++)
            {
                var rt = tabButtons[i].GetComponent<RectTransform>();
                rt.anchoredPosition3D = posDeltaBtn * i;
                rt.sizeDelta = sizeDeltaBtn;
            }

            var indicatorRT = activeTabIndicator.rectTransform;
            var indicatorDelta = indicatorRT.sizeDelta;
            // indicatorRT.anchoredPosition3D = Vector3.down * indicatorDelta.y;
            indicatorRT.sizeDelta = new Vector2(sizeDeltaBtn.x, indicatorDelta.y);
        }
    }
}