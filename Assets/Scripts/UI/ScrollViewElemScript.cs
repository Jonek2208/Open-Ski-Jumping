using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewElemScript : MonoBehaviour
{
    public Button button;
    public CalendarCreatorScript calendarCreatorScript;
    public int index;

    void Start()
    {
        GetComponent<Toggle>().isOn = true;
    }

    public void OnClick(bool val)
    {
        if (val)
        {
            // calendarCreatorScript.UpdateClassificationsPanel(index);
        }
    }
}
