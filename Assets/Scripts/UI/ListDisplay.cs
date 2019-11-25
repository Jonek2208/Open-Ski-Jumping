using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class ListDisplay : MonoBehaviour
{
    public GameObject elementPrefab;
    public GameObject contentObject;
    public GameObject elementPanel;
    public Button AddButton;
    public Button SaveButton;
    public Button DeleteButton;

    [SerializeField]
    protected List<GameObject> elementsList;
    [SerializeField]
    protected int currentIndex;

    void Start()
    {
        currentIndex = -1;
        elementPanel.SetActive(false);
        elementsList = new List<GameObject>();
        ListInit();
    }

    public abstract void ListInit();
    public abstract void ShowElementInfo(int index);

    public void AddListElement(GameObject tmp)
    {
        elementPanel.SetActive(true);
        tmp.GetComponent<ListDisplayElement>().listDisplay = this;
        tmp.GetComponent<ListDisplayElement>().index = elementsList.Count;
        tmp.GetComponent<Toggle>().group = contentObject.GetComponent<ToggleGroup>();
        tmp.transform.SetParent(contentObject.transform, false);
        currentIndex = elementsList.Count;
        elementsList.Add(tmp);
        tmp.GetComponent<Toggle>().isOn = true; 
    }

    public void SaveListElement()
    {
    }

    public void ClearListElement()
    {
        for (int i = 0; i < elementsList.Count; i++) { Destroy(elementsList[i]); }
        elementsList.Clear();
        currentIndex = -1;
        elementPanel.SetActive(false);
    }

    public void DeleteListElement()
    {
        Destroy(elementsList[currentIndex]);
        elementsList.RemoveAt(currentIndex);
        for (int i = currentIndex; i < elementsList.Count; i++)
        {
            elementsList[i].GetComponent<ListDisplayElement>().index = i;
        }
        currentIndex = -1;
        elementPanel.SetActive(false);
    }

    public void OnListElementClick(int index)
    {
        elementPanel.SetActive(true);
        currentIndex = index;
        ShowElementInfo(index);
    }
}
