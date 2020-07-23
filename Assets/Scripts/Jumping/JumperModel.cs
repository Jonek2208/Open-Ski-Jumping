using UnityEngine;

namespace OpenSkiJumping.Jumping
{
    public class JumperModel : MonoBehaviour
    {
        public Animator animator;
        public CapsuleCollider bodyCollider;
        public SphereCollider distCollider1;
        public SphereCollider distCollider2;
        public GameObject modelObject;
        public GameObject skiLeft;
        public BoxCollider skiLeftCollider;
        public GameObject skiRight;
        public BoxCollider skiRightCollider;
    }
}