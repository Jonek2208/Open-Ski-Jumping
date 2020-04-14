using System.Collections.Generic;
using System.Linq;
using Competition.Persistent;
using UnityEngine;

namespace UI.CalendarEditor
{
    [CreateAssetMenu(menuName = "ScriptableObjects/CalendarFactory")]
    public class CalendarFactory : ScriptableObject
    {
        [SerializeField] private List<Competitor> competitors;
        [SerializeField] private List<ClassificationInfo> classifications;
        [SerializeField] private List<EventInfo> events;

        public List<EventInfo> Events
        {
            get => events;
            set => events = value;
        }

        public List<ClassificationInfo> Classifications
        {
            get => classifications;
            set => classifications = value;
        }

        public List<Competitor> Competitors
        {
            get => competitors;
            set => competitors = value;
        }

        public Calendar CreateCalendar()
        {
            return new Calendar
            {
                events = Events.ToList(), classifications = Classifications.ToList(),
                competitorsIds = competitors.Select(it => it.id).ToList()
            };
        }
    }
}