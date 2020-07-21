using UnityEngine;

namespace OpenSkiJumping.Competition.Runtime
{
    public interface IJumpData
    {
        decimal Distance { get; set; }
        decimal[] JudgesMarks { get; set; }
        int GatesDiff { get; set; }
        decimal Wind { get; set; }
        decimal Speed { get; set; }
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeJumpData")]
    public class RuntimeJumpData : ScriptableObject, IJumpData
    {
        [SerializeField] private int gatesDiff;
        [SerializeField] private decimal speed;
        [SerializeField] private decimal distance;
        [SerializeField] private decimal wind;
        [SerializeField] private decimal[] judgesMarks;

        public decimal Distance
        {
            get => distance;
            set => distance = value;
        }

        public decimal[] JudgesMarks
        {
            get => judgesMarks;
            set => judgesMarks = value;
        }

        public int GatesDiff
        {
            get => gatesDiff;
            set => gatesDiff = value;
        }

        public decimal Wind
        {
            get => wind;
            set => wind = value;
        }

        public decimal Speed
        {
            get => speed;
            set => speed = value;
        }

        public void ResetValues()
        {
            gatesDiff = 0;
            speed = 0;
            distance = 0;
            wind = 0;
            for (int i = 0; i < judgesMarks.Length; i++)
            {
                judgesMarks[i] = 0;
            }
        }
    }
}