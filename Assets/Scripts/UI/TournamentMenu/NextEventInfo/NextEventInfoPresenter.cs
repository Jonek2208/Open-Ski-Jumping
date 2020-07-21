namespace OpenSkiJumping.UI.TournamentMenu.NextEventInfo
{
    public class NextEventInfoPresenter
    {
        private readonly INextEventInfoView view;
        private readonly TournamentMenuData model;

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
            var currentEventId = model.GetCurrentEventId();
            view.Hill = $"{currentEventId + 1}. {currentEvent.hillId}";
            view.EventType = currentEvent.eventType;
        }
    }
}