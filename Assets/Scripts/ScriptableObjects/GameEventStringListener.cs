using UnityEngine;
using UnityEngine.Events;

public class GameEventStringListener : MonoBehaviour
{
    [System.Serializable] public class StringEvent : UnityEvent<string> { }
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