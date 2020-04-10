using System.Collections.Generic;
using Competition;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeResultsDatabase")]
public class RuntimeResultsDatabase : ScriptableObject
{
    [SerializeField] private ResultsDatabase value;
    public ResultsDatabase Value { get => value; set => this.value = value; }
}