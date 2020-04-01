using Competition;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeCalendar")]
public class RuntimeCalendar : ScriptableObject
{
    [SerializeField]
    private Competition.Calendar value;

    public Calendar Value { get => value; set => this.value = value; }
}