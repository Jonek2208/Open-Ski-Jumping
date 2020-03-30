using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LISTDIRECTION
{
    HORIZONTAL = 0,
    VERTICAL = 1
}

public interface IListViewElement<T>
{
    void UpdateContent(int index, T val);
}

public class ListView<ItemData, Item> : MonoBehaviour where Item : MonoBehaviour, IListViewElement<ItemData>
{
    public RectTransform content;
    public Mask mask;
    public RectTransform listItem;
    public ScrollRect scrollRect;
    public float Spacing;
    public LISTDIRECTION Direction = LISTDIRECTION.HORIZONTAL;
    private RectTransform maskRT;
    private int numVisible;
    private int numBuffer = 2;
    private float containerHalfSize;
    private float prefabSize;

    [SerializeField]
    private List<ItemData> items = new List<ItemData>();

    private Dictionary<int, int[]> itemDict = new Dictionary<int, int[]>();
    private List<RectTransform> listItemRect = new List<RectTransform>();
    private List<Item> listItems = new List<Item>();
    private int numItems = 0;
    private Vector3 startPos;
    private Vector3 offsetVec;
    private float scrollBarPosition = 1;

    public List<ItemData> Items { get => items; set { items = value; } }

    // Use this for initialization
    private void Start()
    {
        ShowHelper();
        this.scrollRect.onValueChanged.AddListener(ReorderItemsByPos);
    }

    private void OnDestroy()
    {
        this.scrollRect.onValueChanged.RemoveListener(ReorderItemsByPos);
    }

    public void Add(ItemData val)
    {
        this.items.Add(val);
        Refresh();
    }

    public void RemoveAt(int index)
    {
        this.items.RemoveAt(index);
        Refresh();
    }

    public void Refresh()
    {
        float tmp = this.scrollBarPosition;
        ShowHelper();
        this.scrollBarPosition = tmp;
        RefreshShownValue();
    }

    public void Refresh(float targetPosition)
    {
        ShowHelper();
        this.scrollBarPosition = targetPosition;
        RefreshShownValue();
    }

    private void ShowHelper()
    {
        this.scrollBarPosition = 1;
        this.maskRT = mask.GetComponent<RectTransform>();
        this.content.anchoredPosition3D = new Vector3(0, 0, 0);
        Vector2 prefabScale = listItem.rect.size;
        this.prefabSize = (Direction == LISTDIRECTION.HORIZONTAL ? prefabScale.x : prefabScale.y) + Spacing;
        this.content.sizeDelta = Direction == LISTDIRECTION.HORIZONTAL ? (new Vector2(prefabSize * items.Count, prefabScale.y)) : (new Vector2(prefabScale.x, prefabSize * items.Count));
        this.containerHalfSize = Direction == LISTDIRECTION.HORIZONTAL ? (content.rect.size.x * 0.5f) : (content.rect.size.y * 0.5f);

        this.numVisible = Mathf.CeilToInt((Direction == LISTDIRECTION.HORIZONTAL ? maskRT.rect.size.x : maskRT.rect.size.y) / prefabSize);

        this.offsetVec = Direction == LISTDIRECTION.HORIZONTAL ? Vector3.right : Vector3.down;
        this.startPos = content.anchoredPosition3D - (offsetVec * containerHalfSize) + (offsetVec * ((Direction == LISTDIRECTION.HORIZONTAL ? prefabScale.x : prefabScale.y) * 0.5f));
        this.numItems = Mathf.Min(items.Count, numVisible + numBuffer);

        for (int i = 0; i < Mathf.Min(numItems, listItems.Count); i++)
        {
            GameObject obj = listItems[i].gameObject;
            RectTransform t = listItemRect[i];
            t.anchoredPosition3D = startPos + (offsetVec * i * prefabSize);
            obj.SetActive(true);
        }

        for (int i = listItems.Count; i < numItems; i++)
        {
            GameObject obj = (GameObject)Instantiate(listItem.gameObject, content.transform);
            RectTransform t = obj.GetComponent<RectTransform>();
            t.anchoredPosition3D = startPos + (offsetVec * i * prefabSize);
            this.listItemRect.Add(t);
            this.itemDict.Add(t.GetInstanceID(), new int[] { i, i });
            obj.SetActive(true);

            Item li = obj.GetComponentInChildren<Item>();
            this.listItems.Add(li);
            li.UpdateContent(i, items[i]);
        }

        for (int i = numItems; i < listItems.Count; i++)
        {
            this.listItems[i].gameObject.SetActive(false);
        }

        this.listItem.gameObject.SetActive(false);
    }

    public void ReorderItemsByPos(Vector2 normPos)
    {
        // Debug.Log($"Reorder Num Items {numItems} List Item Rect: {listItemRect.Count}");
        this.scrollBarPosition = (Direction == LISTDIRECTION.HORIZONTAL ? normPos.x : normPos.y);
        RefreshShownValue();
    }

    public void RefreshShownValue()
    {
        float normPos = scrollBarPosition;
        if (Direction == LISTDIRECTION.VERTICAL) normPos = 1f - normPos;
        int numOutOfView = Mathf.CeilToInt(normPos * (items.Count - numVisible));   //number of elements beyond the left boundary (or top)
        int firstIndex = Mathf.Max(0, numOutOfView - numBuffer);   //index of first element beyond the left boundary (or top)
        int originalIndex = firstIndex % numItems;

        int newIndex = firstIndex;
        for (int i = originalIndex; i < numItems; i++)
        {
            moveItemByIndex(listItemRect[i], newIndex);
            this.listItems[i].UpdateContent(newIndex, items[newIndex]);
            newIndex++;
        }
        for (int i = 0; i < originalIndex; i++)
        {
            moveItemByIndex(listItemRect[i], newIndex);
            this.listItems[i].UpdateContent(newIndex, items[newIndex]);
            newIndex++;
        }
    }

    private void moveItemByIndex(RectTransform item, int index)
    {
        int id = item.GetInstanceID();
        this.itemDict[id][0] = index;
        item.anchoredPosition3D = startPos + (offsetVec * index * prefabSize);
    }
}