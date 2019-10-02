using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CompetitionClasses;

public class ClassificationsListUI : ListDisplay
{
    public TMPro.TMP_InputField nameInput;
    public TMPro.TMP_Dropdown dropdown1;
    public TMPro.TMP_Dropdown dropdown2;

    public List<Classification> classificationsList;


    public override void ListInit()
    {
        classificationsList = new List<Classification>();
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
    public override void ShowElementInfo(int index)
    {
        nameInput.text = classificationsList[index].name;
        dropdown1.value = (int)(classificationsList[index].classificationType) / 2;
        dropdown2.value = (int)(classificationsList[index].classificationType) % 2;
    }

    public void Add()
    {
        Classification classification = new Classification("New Tournament", ClassificationType.IndividualPlace);
        classificationsList.Add(classification);
        AddListElement(NewListElement(classification));
    }

    public void Save()
    {
        classificationsList[currentIndex].name = nameInput.text;
        classificationsList[currentIndex].classificationType = (ClassificationType)(2 * dropdown1.value + dropdown2.value);
        SetValue(elementsList[currentIndex], classificationsList[currentIndex]);
    }

    public void Delete()
    {
        classificationsList.RemoveAt(currentIndex);
        DeleteListElement();
    }

}
