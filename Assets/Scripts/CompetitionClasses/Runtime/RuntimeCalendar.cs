using CompCal;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeCalendar")]
public class RuntimeCalendar : ScriptableObject
{
    [SerializeField]
    private CompCal.Calendar value;

    public Calendar Value { get => value; set => this.value = value; }
}