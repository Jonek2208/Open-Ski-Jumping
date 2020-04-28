using UnityEngine;

namespace OpenSkiJumping.Jumping
{
    public class ReplayController : MonoBehaviour
    {
        public Transform[] bodyParts;
        public Replay replay;
        public bool isRecording;

        void Update()
        {
            if (isRecording)
            {
                replay.AddFrame(bodyParts);
            }
        }
    }
}