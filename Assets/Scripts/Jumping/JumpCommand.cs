using System;

namespace OpenSkiJumping.Jumping
{
    public enum JumpCommandType
    {
        Action,
        Move
    }

    [Serializable]
    public class JumpCommand
    {
        public JumpCommandType commandType;
        public float value;
    }

    [Serializable]
    public class JumpData
    {
        
    }
}