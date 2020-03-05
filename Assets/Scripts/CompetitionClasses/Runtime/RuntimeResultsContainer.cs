using CompCal;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeResultsContainer")]
public class RuntimeResultsContainer : ScriptableObject
{
    [SerializeField]
    private CompCal.ResultsContainer value;

    public ResultsContainer Value { get => value; set => this.value = value; }
}