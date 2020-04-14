using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ListView
{
    public enum LISTDIRECTION
    {
        HORIZONTAL = 0,
        VERTICAL = 1
    }

    public class ListView<ItemData, Item> : MonoBehaviour where Item : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private Mask mask;
        [SerializeField] private RectTransform listItem;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float Spacing;
        [SerializeField] private LISTDIRECTION Direction = LISTDIRECTION.HORIZONTAL;
        private RectTransform maskRT;
        private int numVisible;
        private readonly int numBuffer = 2;
        private float containerHalfSize;
        private float prefabSize;

        [SerializeField] private IList<ItemData> items = new List<ItemData>();
        private readonly Dictionary<int, int[]> itemDict = new Dictionary<int, int[]>();
        private readonly List<RectTransform> listItemRect = new List<RectTransform>();
        private readonly List<Item> listItems = new List<Item>();
        private int numItems;
        private Vector3 startPos;
        private Vector3 offsetVec;
        private float scrollBarPosition = 1;
        public IList<ItemData> Items { get => items; set => items = value; }
        public Action<int, Item> BindItem { get; set; }

        public void Initialize(Action<int, Item> fun)
        {
            BindItem = fun;
            ShowHelper();
            scrollRect.onValueChanged.AddListener(ReorderItemsByPos);
        }

        private void OnDestroy()
        {
            scrollRect.onValueChanged.RemoveListener(ReorderItemsByPos);
        }

        public void Refresh()
        {
            float tmp = scrollBarPosition;
            ShowHelper();
            scrollBarPosition = tmp;
            RefreshShownValue();
        }

        public void Refresh(float targetPosition)
        {
            ShowHelper();
            scrollBarPosition = targetPosition;
            RefreshShownValue();
        }

        public void ScrollToIndex(int index)
        {
            float x = content.rect.size.y;
            float a = maskRT.rect.size.y;
            scrollBarPosition = 1 - Mathf.Clamp01((index / (float)items.Count * x) / (x - a) - a / 2f / x);
            scrollRect.verticalNormalizedPosition = scrollBarPosition;
        }

        private void ShowHelper()
        {
            scrollBarPosition = 1;
            maskRT = mask.GetComponent<RectTransform>();
            content.anchoredPosition3D = new Vector3(0, 0, 0);
            Vector2 prefabScale = listItem.rect.size;
            prefabSize = (Direction == LISTDIRECTION.HORIZONTAL ? prefabScale.x : prefabScale.y) + Spacing;
            content.sizeDelta = Direction == LISTDIRECTION.HORIZONTAL ? (new Vector2(prefabSize * items.Count, prefabScale.y)) : (new Vector2(prefabScale.x, prefabSize * items.Count));
            containerHalfSize = Direction == LISTDIRECTION.HORIZONTAL ? (content.rect.size.x * 0.5f) : (content.rect.size.y * 0.5f);

            numVisible = Mathf.CeilToInt((Direction == LISTDIRECTION.HORIZONTAL ? maskRT.rect.size.x : maskRT.rect.size.y) / prefabSize);

            offsetVec = Direction == LISTDIRECTION.HORIZONTAL ? Vector3.right : Vector3.down;
            startPos = content.anchoredPosition3D - (offsetVec * containerHalfSize) + (offsetVec * ((Direction == LISTDIRECTION.HORIZONTAL ? prefabScale.x : prefabScale.y) * 0.5f));
            numItems = Mathf.Min(items.Count, numVisible + numBuffer);

            for (int i = 0; i < Mathf.Min(numItems, listItems.Count); i++)
            {
                GameObject obj = listItems[i].gameObject;
                RectTransform t = listItemRect[i];
                t.anchoredPosition3D = startPos + (offsetVec * i * prefabSize);
                obj.SetActive(true);
            }

            for (int i = listItems.Count; i < numItems; i++)
            {
                GameObject obj = Instantiate(listItem.gameObject, content.transform);
                RectTransform t = obj.GetComponent<RectTransform>();
                t.anchoredPosition3D = startPos + (offsetVec * i * prefabSize);
                listItemRect.Add(t);
                itemDict.Add(t.GetInstanceID(), new[] { i, i });
                obj.SetActive(true);

                Item li = obj.GetComponentInChildren<Item>();
                listItems.Add(li);
                BindItem(i, li);
            }

            for (int i = numItems; i < listItems.Count; i++)
            {
                listItems[i].gameObject.SetActive(false);
            }

            listItem.gameObject.SetActive(false);
        }

        public void ReorderItemsByPos(Vector2 normPos)
        {
            scrollBarPosition = (Direction == LISTDIRECTION.HORIZONTAL ? normPos.x : normPos.y);
            RefreshShownValue();
        }

        public void RefreshShownValue()
        {
            float normPos = scrollBarPosition;
            if (Direction == LISTDIRECTION.VERTICAL) normPos = 1f - normPos;
            int numOutOfView = Mathf.CeilToInt(normPos * (items.Count - numVisible)); 
            int firstIndex = Mathf.Max(0, numOutOfView - numBuffer); 
            int originalIndex = firstIndex % numItems;

            int newIndex = firstIndex;
            for (int i = originalIndex; i < numItems; i++)
            {
                MoveItemByIndex(listItemRect[i], newIndex);
                BindItem(newIndex, listItems[i]);
                newIndex++;
            }
            for (int i = 0; i < originalIndex; i++)
            {
                MoveItemByIndex(listItemRect[i], newIndex);
                BindItem(newIndex, listItems[i]);
                newIndex++;
            }
        }

        private void MoveItemByIndex(RectTransform item, int index)
        {
            int id = item.GetInstanceID();
            itemDict[id][0] = index;
            item.anchoredPosition3D = startPos + (offsetVec * index * prefabSize);
        }
    }
}