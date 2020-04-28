using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace OpenSkiJumping.ListView
{
    public class ListItem : ListItemBehaviour
    {
        public ToggleExtension toggleExtension;
        public void UpdateContent(int index, IList<ResultData> val)
        {
            GetComponentInChildren<TMP_Text>().text = $"{val[index].result:F1} {val[index].firstName} {val[index].lastName}";
            toggleExtension.SetElementId(index);
        }
    }

    [Serializable]
    public class ResultData
    {
        public float result;
        public string firstName;
        public string lastName;
    }
}