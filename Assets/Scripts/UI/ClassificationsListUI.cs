using System.Collections.Generic;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using TMPro;
using UnityEngine;

namespace OpenSkiJumping.UI
{
    public class ClassificationsListUI : ListDisplay
    {
        public TMP_InputField nameInput;
        public TMP_Dropdown dropdown1;
        public TMP_Dropdown dropdown2;

        public bool updated;

        public List<ClassificationInfo> classificationsList;

        public override void ListInit()
        {
            classificationsList = new List<ClassificationInfo>();
            updated = false;
        }

        public override void ShowElementInfo(int index)
        {
            changeEventListener.enabled = false;
            nameInput.text = classificationsList[index].name;
            dropdown1.value = (int) (classificationsList[index].classificationType) / 2;
            dropdown2.value = (int) (classificationsList[index].classificationType) % 2;
            changeEventListener.enabled = true;
        }

        public void LoadList(List<ClassificationInfo> tmpList)
        {
            ClearListElement();
            ListInit();
            if (tmpList == null)
            {
                tmpList = new List<ClassificationInfo>();
            }

            foreach (var item in tmpList)
            {
                classificationsList.Add(item);
                AddListElement(NewListElement(item));
            }
        }

        public GameObject NewListElement(ClassificationInfo classification)
        {
            GameObject tmp = Instantiate(elementPrefab);
            SetValue(tmp, classification);
            return tmp;
        }

        public void SetValue(GameObject tmp, ClassificationInfo classification)
        {
            tmp.GetComponentInChildren<TMP_Text>().text = classification.name;
        }

        public void Add()
        {
            ClassificationInfo classification = new ClassificationInfo
                {name = "New Tournament", classificationType = ClassificationType.Place};
            classificationsList.Add(classification);
            AddListElement(NewListElement(classification));
            updated = false;
        }

        public void Save()
        {
            changeEventListener.enabled = false;
            classificationsList[currentIndex].name = nameInput.text;
            classificationsList[currentIndex].classificationType =
                (ClassificationType) (2 * dropdown1.value + dropdown2.value);
            changeEventListener.enabled = true;
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
}