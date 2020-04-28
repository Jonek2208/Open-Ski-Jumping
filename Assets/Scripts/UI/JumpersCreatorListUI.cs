using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    public class JumpersCreatorListUI : ListDisplay
    {
        public CompetitorsRuntime databaseManager;
        public FlagsData flagsData;
        public TMP_InputField lastNameInput;
        public TMP_InputField firstNameInput;
        public TMP_InputField countryCodeInput;

        public SimpleColorPicker helmetColorPicker;

        // public string suitTopFrontColor;
        // public string suitTopBackColor;
        // public string suitBottomFrontColor;
        // public string suitBottomBackColor;
        public SimpleColorPicker suitTopFrontColorPicker;
        public SimpleColorPicker suitTopBackColorPicker;
        public SimpleColorPicker suitBottomFrontColorPicker;
        public SimpleColorPicker suitBottomBackColorPicker;

        public SimpleColorPicker skisColorPicker;
        // public TMPro.TMP_Dropdown genderDropdown;

        public bool updated;
        private bool initialized;

        public List<Competitor> competitorsList;
        // private void LoadJumpersData(out List<Competitor> tmpList)
        // {
        //     tmpList = new List<Competitor>();
        //     if (databaseManager.dbCompetitors.Loaded) { tmpList = databaseManager.dbCompetitors.Data; }
        // }


        public override void ListInit()
        {
            // LoadJumpersData(out competitorsList);
            foreach (var competitor in competitorsList)
            {
                AddListElement(NewListElement(competitor));
            }

            updated = false;
            initialized = true;
        }

        public GameObject NewListElement(Competitor competitor)
        {
            GameObject tmp = Instantiate(elementPrefab);
            SetValue(tmp, competitor);
            return tmp;
        }

        public void SetValue(GameObject tmp, Competitor competitor)
        {
            tmp.GetComponentsInChildren<TMP_Text>()[0].text =
                competitor.firstName + " " + competitor.lastName.ToUpper();
            tmp.GetComponentsInChildren<TMP_Text>()[1].text = competitor.countryCode;
            tmp.GetComponentsInChildren<Image>()[1].sprite = flagsData.GetFlag(competitor.countryCode);
        }

        public override void ShowElementInfo(int index)
        {
            firstNameInput.text = competitorsList[index].firstName;
            lastNameInput.text = competitorsList[index].lastName;
            countryCodeInput.text = competitorsList[index].countryCode;
            // Debug.Log("SHOWING: " + competitorsList[index].helmetColor + " " + competitorsList[index].suitTopFrontColor + " " + competitorsList[index].skisColor);
            helmetColorPicker.SetValueWithoutNotify(competitorsList[index].helmetColor);
            suitTopFrontColorPicker.SetValueWithoutNotify(competitorsList[index].suitTopFrontColor);
            suitTopBackColorPicker.SetValueWithoutNotify(competitorsList[index].suitTopBackColor);
            suitBottomFrontColorPicker.SetValueWithoutNotify(competitorsList[index].suitBottomFrontColor);
            suitBottomBackColorPicker.SetValueWithoutNotify(competitorsList[index].suitBottomBackColor);
            skisColorPicker.SetValueWithoutNotify(competitorsList[index].skisColor);
            // genderDropdown.value = (int)(competitorsList[index].gender);
        }

        public void Add()
        {
            Competitor competitor = new Competitor
                {lastName = "LastName", firstName = "FirstName", countryCode = "XXX"};
            competitorsList.Add(competitor);
            AddListElement(NewListElement(competitor));
            updated = false;
        }

        public void Save()
        {
            competitorsList[currentIndex].lastName = lastNameInput.text;
            competitorsList[currentIndex].firstName = firstNameInput.text;
            competitorsList[currentIndex].countryCode = countryCodeInput.text;
            competitorsList[currentIndex].helmetColor = helmetColorPicker.ToHex;
            competitorsList[currentIndex].suitTopFrontColor = suitTopFrontColorPicker.ToHex;
            competitorsList[currentIndex].suitTopBackColor = suitTopBackColorPicker.ToHex;
            competitorsList[currentIndex].suitBottomFrontColor = suitBottomFrontColorPicker.ToHex;
            competitorsList[currentIndex].suitBottomBackColor = suitBottomBackColorPicker.ToHex;
            competitorsList[currentIndex].skisColor = skisColorPicker.ToHex;
            // Debug.Log(competitorsList[currentIndex].helmetColor + " " + competitorsList[currentIndex].suitColor + " " + competitorsList[currentIndex].skisColor);
            Debug.Log("SAVED COMPETITOR DATA");
            // competitorsList[currentIndex].gender = (Gender)genderDropdown.value;
            SetValue(elementsList[currentIndex], competitorsList[currentIndex]);
            updated = false;
        }

        public void Delete()
        {
            competitorsList.RemoveAt(currentIndex);
            DeleteListElement();
            updated = false;
        }
    }
}