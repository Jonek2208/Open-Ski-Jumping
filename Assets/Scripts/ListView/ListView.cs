using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.ListView
{
    public enum ListDirection
    {
        Vertical,
        Horizontal
    }

    public enum SelectionType
    {
        None,
        Single,
        Free
    }

    public class ListView<TItemData, TItem> : MonoBehaviour where TItem : ListItemBehaviour
    {
        private const int NumBuffer = 2;
        private readonly Dictionary<int, int[]> itemDict = new Dictionary<int, int[]>();
        private readonly List<RectTransform> listItemRect = new List<RectTransform>();
        private readonly List<TItem> listItems = new List<TItem>();
        private float containerHalfSize;
        [SerializeField] private RectTransform content;
        [SerializeField] private ListDirection direction = ListDirection.Vertical;

        [SerializeField] private IList<TItemData> items = new List<TItemData>();
        [SerializeField] private RectTransform listItem;
        [SerializeField] private Mask mask;


        private RectTransform maskRT;
        private int numItems;
        private int numVisible;
        private Vector3 offsetVec;
        private float prefabSize;
        private float scrollBarPosition = 1;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private SelectionType selectionType;
        [SerializeField] private float spacing;
        private Vector3 startPos;

        public SelectionType SelectionType
        {
            get => selectionType;
            set => selectionType = value;
        }

        public int SelectedIndex { get; set; } = -1;

        public TItem SelectedItem => SelectedIndex == -1 ? null : listItems[SelectedIndex];

        public IList<TItemData> Items
        {
            get => items;
            set => items = value;
        }

        public Action<int, TItem> BindItem { get; set; }

        public void ClampSelectedIndex()
        {
            SelectedIndex = Mathf.Max(0, SelectedIndex);
            SelectedIndex = Mathf.Min(SelectedIndex, items.Count - 1);
        }

        public event Action<int> OnSelectionChanged;

        public void Initialize(Action<int, TItem> fun)
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
            if (numItems == 0) SelectedIndex = -1;
            ShowHelper();
            var tmp = scrollBarPosition;
            scrollBarPosition = tmp;
            RefreshShownValue();
        }


        public void Reset()
        {
            ShowHelper();
            RefreshShownValue();
        }

        public void ScrollToIndex(int index)
        {
            if (items.Count == 0) return;
            var x = content.rect.size.y;
            var a = maskRT.rect.size.y;
            scrollBarPosition = 1 - Mathf.Clamp01(index / (float) items.Count * x / (x - a) - a / 2f / x);
            scrollRect.verticalNormalizedPosition = scrollBarPosition;
        }

        private void ShowHelper()
        {
            scrollBarPosition = 1;
            maskRT = mask.GetComponent<RectTransform>();
            content.anchoredPosition3D = new Vector3(0, 0, 0);
            var prefabScale = listItem.rect.size;
            prefabSize = (direction == ListDirection.Horizontal ? prefabScale.x : prefabScale.y) + spacing;
            content.sizeDelta = direction == ListDirection.Horizontal
                ? new Vector2(prefabSize * items.Count, prefabScale.y)
                : new Vector2(prefabScale.x, prefabSize * items.Count);
            var rect = content.rect;
            containerHalfSize = direction == ListDirection.Horizontal
                ? rect.size.x * 0.5f
                : rect.size.y * 0.5f;

            var rectRT = maskRT.rect;
            numVisible =
                Mathf.CeilToInt((direction == ListDirection.Horizontal ? rectRT.size.x : rectRT.size.y) /
                                prefabSize);

            offsetVec = direction == ListDirection.Horizontal ? Vector3.right : Vector3.down;
            startPos = content.anchoredPosition3D - offsetVec * containerHalfSize +
                       offsetVec * ((direction == ListDirection.Horizontal ? prefabScale.x : prefabScale.y) * 0.5f);
            numItems = Mathf.Min(items.Count, numVisible + NumBuffer);

            for (var i = 0; i < Mathf.Min(numItems, listItems.Count); i++)
            {
                var obj = listItems[i].gameObject;
                var t = listItemRect[i];
                t.anchoredPosition3D = startPos + offsetVec * i * prefabSize;
                obj.SetActive(true);
            }

            for (var i = listItems.Count; i < numItems; i++)
            {
                var obj = Instantiate(listItem.gameObject, content.transform);
                var t = obj.GetComponent<RectTransform>();
                t.anchoredPosition3D = startPos + offsetVec * i * prefabSize;
                listItemRect.Add(t);
                itemDict.Add(t.GetInstanceID(), new[] {i, i});
                obj.SetActive(true);

                var li = obj.GetComponentInChildren<TItem>();
                li.OnSelect += HandleSelectionChanged;
                listItems.Add(li);
                MakeItem(i, li);
            }

            for (var i = numItems; i < listItems.Count; i++) listItems[i].gameObject.SetActive(false);

            listItem.gameObject.SetActive(false);
        }

        public void ReorderItemsByPos(Vector2 normPos)
        {
            scrollBarPosition = Mathf.Clamp01(direction == ListDirection.Horizontal ? normPos.x : normPos.y);
            RefreshShownValue();
        }

        public void RefreshShownValue()
        {
            if (numItems == 0) return;
            var normPos = scrollBarPosition;
            if (direction == ListDirection.Vertical) normPos = 1f - normPos;
            var numOutOfView = Mathf.CeilToInt(normPos * (items.Count - numVisible));
            var firstIndex = Mathf.Max(0, numOutOfView - NumBuffer);
            var originalIndex = firstIndex % numItems;

            var newIndex = firstIndex;
            for (var i = originalIndex; i < numItems; i++)
            {
                MoveItemByIndex(listItemRect[i], newIndex);
                MakeItem(newIndex, listItems[i]);
                newIndex++;
            }

            for (var i = 0; i < originalIndex; i++)
            {
                MoveItemByIndex(listItemRect[i], newIndex);
                MakeItem(newIndex, listItems[i]);
                newIndex++;
            }
        }

        private void MoveItemByIndex(RectTransform item, int index)
        {
            var id = item.GetInstanceID();
            itemDict[id][0] = index;
            item.anchoredPosition3D = startPos + offsetVec * index * prefabSize;
        }

        private void MakeItem(int index, TItem item)
        {
            BindItem(index, item);
            item.Index = index;
            item.SelectionType = selectionType;
            if (selectionType == SelectionType.None) return;

            item.SetSelectionWithoutNotify(index == SelectedIndex);
        }

        private void HandleSelectionChanged(int index)
        {
            if (selectionType == SelectionType.None) return;

            if (index != SelectedIndex)
            {
                SelectedIndex = index;
                RefreshShownValue();
                OnSelectionChanged?.Invoke(index);
            }
        }
    }
}