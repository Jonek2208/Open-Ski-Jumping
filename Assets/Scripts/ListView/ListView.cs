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

public class ListView<T, T1> : MonoBehaviour where T1 : MonoBehaviour, IListViewElement<T>
{
    public RectTransform content;
    public Mask mask;
    public RectTransform listItem;
    public float Spacing;
    public LISTDIRECTION Direction = LISTDIRECTION.HORIZONTAL;
    private RectTransform maskRT;
    private int numVisible;
    private int numBuffer = 2;
    private float containerHalfSize;
    private float prefabSize;

    [SerializeField]
    private List<T> items = new List<T>();

    private Dictionary<int, int[]> itemDict = new Dictionary<int, int[]>();
    private List<RectTransform> listItemRect = new List<RectTransform>();
    private List<T1> listItems = new List<T1>();
    private int numItems = 0;
    private Vector3 startPos;
    private Vector3 offsetVec;
    private float scrollBarPosition = 0;

    public List<T> Items { get => items; set { items = value; Show(); } }

    // Use this for initialization
    void Start()
    {
        Show();
    }

    public void AddItem(T val)
    {
        items.Add(val);
        Show();
    }

    public void Show()
    {
        content.anchoredPosition3D = new Vector3(0, 0, 0);

        maskRT = mask.GetComponent<RectTransform>();
        Vector2 prefabScale = listItem.rect.size;
        prefabSize = (Direction == LISTDIRECTION.HORIZONTAL ? prefabScale.x : prefabScale.y) + Spacing;
        // if (items.Count == 0) return;
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
            GameObject obj = (GameObject)Instantiate(listItem.gameObject, content.transform);
            RectTransform t = obj.GetComponent<RectTransform>();
            t.anchoredPosition3D = startPos + (offsetVec * i * prefabSize);
            listItemRect.Add(t);
            itemDict.Add(t.GetInstanceID(), new int[] { i, i });
            obj.SetActive(true);

            T1 li = obj.GetComponentInChildren<T1>();
            listItems.Add(li);
            li.UpdateContent(i, items[i]);
        }

        for (int i = numItems; i < listItems.Count; i++)
        {
            listItems[i].gameObject.SetActive(false);
        }

        listItem.gameObject.SetActive(false);
        if (items.Count > 0)
        {
            ReorderItemsByPos(scrollBarPosition);
        }
        content.anchoredPosition3D += offsetVec * (containerHalfSize - ((Direction == LISTDIRECTION.HORIZONTAL ? maskRT.rect.size.x : maskRT.rect.size.y) * 0.5f));
    }

    public void ReorderItemsByPos(float normPos)
    {
        scrollBarPosition = normPos;
        if (Direction == LISTDIRECTION.VERTICAL) normPos = 1f - normPos;
        int numOutOfView = Mathf.CeilToInt(normPos * (items.Count - numVisible));   //number of elements beyond the left boundary (or top)
        int firstIndex = Mathf.Max(0, numOutOfView - numBuffer);   //index of first element beyond the left boundary (or top)
        int originalIndex = firstIndex % numItems;

        int newIndex = firstIndex;
        for (int i = originalIndex; i < numItems; i++)
        {
            moveItemByIndex(listItemRect[i], newIndex);
            listItems[i].UpdateContent(newIndex, items[newIndex]);
            newIndex++;
        }
        for (int i = 0; i < originalIndex; i++)
        {
            moveItemByIndex(listItemRect[i], newIndex);
            listItems[i].UpdateContent(newIndex, items[newIndex]);
            newIndex++;
        }
    }

    private void moveItemByIndex(RectTransform item, int index)
    {
        int id = item.GetInstanceID();
        itemDict[id][0] = index;
        item.anchoredPosition3D = startPos + (offsetVec * index * prefabSize);
    }
}
