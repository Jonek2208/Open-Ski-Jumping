using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.ListView
{
    [RequireComponent(typeof(Toggle))]
    public class ListItemBehaviour : MonoBehaviour
    {
        [SerializeField] private Toggle selection;
        [SerializeField] private SelectionType selectionType;

        public SelectionType SelectionType
        {
            get => selectionType;
            set => selectionType = value;
        }

        public int Index { get; set; }

        public bool Selection
        {
            get => selection.isOn;
            set => selection.isOn = value;
        }

        public event Action<int> OnSelect;

        protected void HandleSelectionChanged(bool value)
        {
            if (SelectionType == SelectionType.None)
                return;


            if (selection.isOn) OnSelect?.Invoke(Index);
            else selection.SetIsOnWithoutNotify(true);
        }

        public void SetSelectionWithoutNotify(bool value) => selection.SetIsOnWithoutNotify(value);

        protected void Awake()
        {
            selection = GetComponent<Toggle>();
        }

        protected void Start()
        {
            selection.onValueChanged.AddListener(HandleSelectionChanged);
        }
    }
}