using System;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects
{
    public class GameEventStringListener : MonoBehaviour
    {
        [Serializable] public class StringEvent : UnityEvent<string> { }
        public GameEventString Event;
        public StringEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(string value)
        {
            Response.Invoke(value);
        }
    }
}