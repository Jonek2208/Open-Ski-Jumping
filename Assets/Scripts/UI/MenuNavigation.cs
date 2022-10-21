using System;
using System.Collections.Generic;
using OpenSkiJumping.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = System.Object;

namespace OpenSkiJumping.UI
{

    [Serializable]
    public class TransitionData
    {
        public string name;
        public Button button;
        public GameObject viewComponent;
    }

    public class MenuNavigation : MonoBehaviour
    {
        [SerializeField] private List<TransitionData> transitionsList;

        public void MenuTransition(GameObject view)
        {
            gameObject.SetActive(false);
            view.SetActive(true);
        }

        public void Load()
        {
            gameObject.SetActive(true);
        }

        private void Awake()
        {
            foreach (var transitionData in transitionsList)
            {
                transitionData.button.onClick.AddListener(() => MenuTransition(transitionData.viewComponent));
            }
        }
    }
}