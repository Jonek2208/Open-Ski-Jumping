using System.Collections.Generic;
using Competition.Persistent;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CalendarCreatorScript : MonoBehaviour
    {
        public DatabaseManager databaseManager;
        public FlagsData flagsData;
        public GameObject jumpersContentObject;
        public GameObject jumperPrefab;
        public GameObject popUpObject;
        public TMP_InputField calendarNameInput;
        public TMP_Text promptText;

        List<Competitor> allCompetitors;
        List<GameObject> competitorsObjList;

        private ClassificationsListUI classificationsListUI;
        private EventsListUI eventsListUI;
        private CalendarsListUI calendarsListUI;

        private bool editMode;

        public void CreateCompetitorsList()
        {
            competitorsObjList = new List<GameObject>();
            if (databaseManager.dbCompetitors.Loaded)
            {
                allCompetitors = databaseManager.dbCompetitors.Data;
                foreach (var c in allCompetitors)
                {
                    GameObject tmp = Instantiate(jumperPrefab, jumpersContentObject.transform, false);
                    tmp.GetComponentsInChildren<TMP_Text>()[0].text = c.lastName.ToUpper() + " " + c.firstName;
                    tmp.GetComponentsInChildren<Image>()[3].sprite = flagsData.GetFlag(c.countryCode);
                    competitorsObjList.Add(tmp);
                }
            }
        }

        public void ClearCompetitorsList()
        {
            if (competitorsObjList == null) return;
            foreach (var it in competitorsObjList)
            {
                Destroy(it);
            }
        }

        public void ShowPopUp()
        {
            if (editMode)
            {
                SaveCalendar();
            }
            else
            {
                promptText.text = "";
                popUpObject.SetActive(true);
            }
        }

        public void ClosePopUp()
        {
            popUpObject.SetActive(false);
        }

        public void OnSaveButton()
        {
            if (calendarNameInput.text.Equals(""))
            {
                promptText.text = "Calendar name must not be empty!";
            }
            else
            {
                SaveCalendar();
                ClosePopUp();
            }
        }

        public void SetAll(bool val)
        {
            foreach (var item in competitorsObjList)
            {
                item.GetComponentInChildren<Toggle>().isOn = val;
            }
        }

        public void LoadCalendar(Calendar calendar = null)
        {
            if (calendar == null)
            {
                editMode = false;
                calendar = new Calendar();
            }
            else
            {
                editMode = true;
                calendarNameInput.text = calendar.name;
            }

            ClearCompetitorsList();
            CreateCompetitorsList();
            classificationsListUI.LoadList(calendar.classifications);
            eventsListUI.LoadList(calendar.events);
        }

        public void SaveCalendar()
        {
            Calendar calendar = new Calendar();
            calendar.name = calendarNameInput.text;
            for (int i = 0; i < allCompetitors.Count; i++)
            {
                if (competitorsObjList[i].GetComponentInChildren<Toggle>().isOn)
                {
                    // calendar.competitorsIds.Add(allCompetitors[i]);
                }
            }

            calendar.classifications = classificationsListUI.classificationsList;
            calendar.events = eventsListUI.eventsList;
            for (int i = 0; i < calendar.events.Count; i++)
            {
                foreach (var jt in calendar.events[i].classifications)
                {
                    calendar.classifications[jt].events.Add(i);
                }
            }
            if (editMode)
            {
                calendarsListUI.ChangeCalendar(calendar);
            }
            else
            {
                calendarsListUI.AddCalendar(calendar);
            }
        }

        void Start()
        {
            calendarsListUI = GetComponent<CalendarsListUI>();
            classificationsListUI = GetComponent<ClassificationsListUI>();
            eventsListUI = GetComponent<EventsListUI>();
            LoadCalendar();
        }
    }
}