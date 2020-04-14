using Competition.Persistent;
using UnityEngine;

namespace Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeResultsDatabase")]
    public class RuntimeResultsDatabase : ScriptableObject
    {
        [SerializeField] private ResultsDatabase value;
        public ResultsDatabase Value { get => value; set => this.value = value; }
    }
}