using System;
using System.Collections.Generic;

namespace OpenSkiJumping.Jumping
{
    public enum JumpMoveDirection
    {
        Down,
        Up
    }
    
    public enum LandingType
    {
        Both,
        Left,
        Right
    }

    [Serializable]
    public class FlightCommand
    {
        public JumpMoveDirection direction;
        public float value;
    }

    [Serializable]
    public class LandingCommand
    {
        public LandingType type;
        public uint time;
    }

    [Serializable]
    public class JumpData
    {
        public uint seed;
        public uint cnt;
        public uint start;
        public uint takeOff;
        public List<FlightCommand> flightCommands;
        public LandingCommand landingCommand;
    }
}