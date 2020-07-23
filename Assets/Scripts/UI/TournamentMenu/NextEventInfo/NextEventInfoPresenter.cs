namespace OpenSkiJumping.UI.TournamentMenu.NextEventInfo
{
    public class NextEventInfoPresenter
    {
        private readonly TournamentMenuData model;
        private readonly INextEventInfoView view;

        public NextEventInfoPresenter(INextEventInfoView view, TournamentMenuData model)
        {
            this.model = model;
            this.view = view;
            InitEvents();
            SetInitValues();
        }

        private void InitEvents()
        {
            view.OnDataReload += SetInitValues;
        }

        private void SetInitValues()
        {
            var currentEvent = model.GetCurrentEvent();
            if (currentEvent == null) return;
            var currentEventId = model.GetCurrentEventId();
            view.Hill = $"{currentEventId + 1}. {currentEvent.hillId}";
            view.EventType = currentEvent.eventType;
        }
    }
}