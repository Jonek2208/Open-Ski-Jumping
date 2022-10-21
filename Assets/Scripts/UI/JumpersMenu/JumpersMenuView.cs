using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.ScriptableObjects.Variables;
using OpenSkiJumping.UI.Base;
using OpenSkiJumping.UI.ListView;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.JumpersMenu
{
    public class JumpersMenuView : ViewBase, IJumpersMenuView
    {
        [SerializeField] private FlagsData flagsData;
        [SerializeField] private IconsData iconsData;
        [SerializeField] private ImageCacher imageCacher;
        private List<Competitor> _jumpers;

        [SerializeField] private CompetitorsRuntime jumpersRuntime;

        [SerializeField] private JumpersListView listView;
        [SerializeField] private GameEvent jumperEditorTransition;
        [SerializeField] private JumperVariable editedJumperVariable;


        [SerializeField] private Button addButton;

        private JumpersMenuPresenter _presenter;
        public event Action<Competitor> OnCurrentJumperChanged;
        public event Action OnAdd;
        public event Action<Competitor> OnRemove;

        public IEnumerable<Competitor> Jumpers
        {
            set
            {
                _jumpers = value.ToList();
                listView.Items = _jumpers;
                listView.SelectedIndex = Mathf.Clamp(listView.SelectedIndex, 0, _jumpers.Count - 1);
                listView.Refresh();
            }
        }

        protected override void Initialize()
        {
            ListViewSetup();
            RegisterCallbacks();
            _presenter = new JumpersMenuPresenter(this, jumpersRuntime, flagsData);
            OnAdd += () => { EditJumper(new Competitor()); };
        }


        public void EditJumper(Competitor jumper)
        {
            editedJumperVariable.Value = jumper;
            jumperEditorTransition.Raise();
            gameObject.SetActive(false);
        }

        public void HandleRemoveItem(int ind)
        {
            OnRemove?.Invoke(_jumpers[ind]);
        }

        public void HandleJumperClick(int id)
        {
            EditJumper(_jumpers[id]);
        }

        private void ListViewSetup()
        {
            listView.SelectionType = SelectionType.None;
            listView.Initialize(BindListViewItem);
        }

        private void RegisterCallbacks()
        {
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
        }

        private void ScrollToJumper(Competitor jumper)
        {
            var index = jumper == null ? 0 : _jumpers.IndexOf(jumper);
            index = Mathf.Clamp(index, 0, _jumpers.Count - 1);
            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
        }

        private static Action<Sprite, bool> SetJumperImage(Image img)
        {
            return (value, succeeded) =>
            {
                if (!succeeded)
                {
                    img.enabled = false;
                    return;
                }

                img.enabled = true;
                img.sprite = value;
            };
        }


        private void LoadImage(Image img, string path)
        {
            img.enabled = false;
            StartCoroutine(imageCacher.GetSpriteAsync(path, SetJumperImage(img)));
        }


        private void BindListViewItem(int index, JumpersListItem item)
        {
            var jumper = _jumpers[index];
            item.nameText.text = $"{jumper.firstName.ToUpper()} <b>{jumper.lastName.ToUpper()}</b>";
            item.countryFlagText.text = jumper.countryCode;
            item.countryFlagImage.sprite = flagsData.GetFlag(jumper.countryCode);
            item.genderIconImage.sprite = iconsData.GetGenderIcon(jumper.gender);
            item.birthdateText.text = $"{jumper.birthdate:yyyy-MM-dd}";
            LoadImage(item.jumperImage, jumper.imagePath);
        }
    }
}