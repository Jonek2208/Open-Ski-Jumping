using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;

namespace OpenSkiJumping.UI.HillsMenu
{
    public interface IHillsMenuView
    {
        ProfileData SelectedHill { get; set; }
        IHillInfoView HillInfoView { get; }
        IEnumerable<ProfileData> Hills { set; }

        event Action OnSelectionChanged;
        event Action OnAdd;
        event Action OnRemove;

        event Action OnDataReload;
        event Action OnDuplicate;
    }
}