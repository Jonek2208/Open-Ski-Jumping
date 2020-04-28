using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.UI.CalendarEditor
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

        public void MoveEvent(EventInfo item, int val)
        {
            int index = Events.IndexOf(item);
            if (index < 0 || index + val < 0 || Events.Count <= index + val) return;
            var buf = Events[index + val];
            Events[index + val] = Events[index];
            Events[index] = buf;
            Events[index].name = $"{index + 1} {Events[index].hillId}";
            Events[index + val].name = $"{index + val + 1} {Events[index].hillId}";
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