using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeCalendar")]
    public class RuntimeCalendar : ScriptableObject
    {
        [SerializeField]
        private Calendar value;

        public Calendar Value { get => value; set => this.value = value; }
    }
}