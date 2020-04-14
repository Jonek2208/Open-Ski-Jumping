using System;
using System.Collections.Generic;
using Competition.Persistent;

namespace UI.JumpersMenu
{
    public interface IJumpersMenuView
    {
        Competitor SelectedJumper { get; }
        IEnumerable<Competitor> Jumpers { set; }

        string FirstName { get; set; }
        string LastName { get; set; }
        string CountryCode { get; set; }
        int Gender { get; set; }
        string SuitTopFront { get; set; }
        string SuitTopBack { get; set; }
        string SuitBottomFront { get; set; }
        string SuitBottomBack { get; set; }
        string Helmet { get; set; }
        string Skis { get; set; }
        string ImagePath { get; set; }
        
        event Action OnSelectionChanged;
        event Action OnCurrentJumperChanged;
        event Action OnAdd;
        event Action OnRemove;

        void SelectJumper(Competitor jumper);
        void HideJumperInfo();
        void ShowJumperInfo();
        void LoadImage(string path);
    }
}