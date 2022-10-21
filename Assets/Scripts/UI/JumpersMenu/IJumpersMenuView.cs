using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.UI.Base;

namespace OpenSkiJumping.UI.JumpersMenu
{
    public interface IJumpersMenuView : IViewBase
    {
        IEnumerable<Competitor> Jumpers { set; }
        
        event Action<Competitor> OnCurrentJumperChanged;
        event Action OnAdd;
        event Action<Competitor> OnRemove;
    }
}