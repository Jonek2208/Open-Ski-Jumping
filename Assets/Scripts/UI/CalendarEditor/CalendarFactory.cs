using System.Collections.Generic;
using System.Linq;
using Competition.Persistent;
using UnityEngine;

namespace UI.CalendarEditor
{
    [CreateAssetMenu(menuName = "ScriptableObjects/CalendarFactory")]
    public class CalendarFactory : ScriptableObject
    {
        [SerializeField] private IEnumerable<Competitor> competitors;
        [SerializeField] private IEnumerable<ClassificationInfo> classifications;
        [SerializeField] private IEnumerable<EventInfo> events;

        public IEnumerable<EventInfo> Events
        {
            get => events;
            set => events = value;
        }

        public IEnumerable<ClassificationInfo> Classifications
        {
            get => classifications;
            set => classifications = value;
        }

        public IEnumerable<Competitor> Competitors
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