using UnityEngine;

namespace Jumping
{
    public class MouseScript : MonoBehaviour
    {
        bool cursorActive;
        void Start()
        {
            UnlockCursor();
            SetCursor();
        }
        void Update()
        {
            SetCursor();
        }

        void SetCursor()
        {
            if(!cursorActive) 
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
            Cursor.visible = cursorActive;
        }

        public void LockCursor()
        {
            cursorActive = false;
        }

        public void UnlockCursor()
        {
            cursorActive = true;
        }
    }
}
