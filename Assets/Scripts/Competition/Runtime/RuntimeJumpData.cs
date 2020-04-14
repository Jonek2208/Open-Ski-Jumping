using UnityEngine;

namespace Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeJumpData")]
    public class RuntimeJumpData : ScriptableObject
    {
        [SerializeField]
        private int gate;
        [SerializeField]
        private float speed;
        [SerializeField]
        private float distance;
        [SerializeField]
        private float wind;
        [SerializeField]
        private decimal[] judgesMarks;

        public decimal Distance { get => (decimal)distance; set => distance = (float)value; }
        public decimal[] JudgesMarks { get => judgesMarks; set => judgesMarks = value; }
        public int Gate { get => gate; set => gate = value; }
        public decimal Wind { get => (decimal)wind; set => wind = (float)value; }
        public decimal Speed { get => (decimal)speed; set => speed = (float)value; }

        public void ResetValues()
        {
            gate = 0;
            speed = 0;
            distance = 0;
            wind = 0;
            for (int i = 0; i < judgesMarks.Length; i++) { judgesMarks[i] = 0; }
        }
    }
}