using System.Collections.Generic;
using UnityEngine;

using CompCal;

public class ClassificationsListUI : ListDisplay
{
    public TMPro.TMP_InputField nameInput;
    public TMPro.TMP_Dropdown dropdown1;
    public TMPro.TMP_Dropdown dropdown2;

    public bool updated;

    public List<Classification> classificationsList;

    public override void ListInit()
    {
        classificationsList = new List<Classification>();
        updated = false;
    }

    public override void ShowElementInfo(int index)
    {
        nameInput.text = classificationsList[index].name;
        dropdown1.value = (int)(classificationsList[index].classificationType) / 2;
        dropdown2.value = (int)(classificationsList[index].classificationType) % 2;
    }

    public void LoadList(List<CompCal.Classification> tmpList)
    {
        ClearListElement();
        ListInit();
        if (tmpList == null) { tmpList = new List<Classification>(); }
        foreach (var item in tmpList)
        {
            classificationsList.Add(item);
            AddListElement(NewListElement(item));
        }
    }

    public GameObject NewListElement(Classification classification)
    {
        GameObject tmp = Instantiate(elementPrefab);
        SetValue(tmp, classification);
        return tmp;
    }

    public void SetValue(GameObject tmp, Classification classification)
    {
        tmp.GetComponentInChildren<TMPro.TMP_Text>().text = classification.name;
    }

    public void Add()
    {
        Classification classification = new Classification("New Tournament", ClassificationType.IndividualPlace);
        classificationsList.Add(classification);
        AddListElement(NewListElement(classification));
        updated = false;
    }

    public void Save()
    {
        classificationsList[currentIndex].name = nameInput.text;
        classificationsList[currentIndex].classificationType = (ClassificationType)(2 * dropdown1.value + dropdown2.value);
        SetValue(elementsList[currentIndex], classificationsList[currentIndex]);
        updated = false;
    }

    public void Delete()
    {
        classificationsList.RemoveAt(currentIndex);
        DeleteListElement();
        updated = false;
    }

}
