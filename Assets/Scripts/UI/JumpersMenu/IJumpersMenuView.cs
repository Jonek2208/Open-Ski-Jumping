using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.UI.JumpersMenu
{
    public interface IJumpersMenuView
    {
        Competitor SelectedJumper { get; set; }
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
        string Skin { get; set; }
        string ImagePath { get; set; }
        
        event Action OnSelectionChanged;
        event Action OnCurrentJumperChanged;
        event Action OnAdd;
        event Action OnRemove;
        
        bool JumperInfoEnabled { set; }
        void LoadImage(string path);
    }
}