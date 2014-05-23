using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFL_Head_Coach.Base_Class
{
    class Ability
    {
        public enum AbilityType
        {
            Speed = 0,
            Acceleration = 1,
            Strength = 2,
            Fatigue = 3
        }

        public double Acceleration { get; private set; }
        public double Fatigue { get; private set; }
        public double Speed { get; private set; }
        public double Strength { get; private set; }

        public void SetAbility(AbilityType type, double value)
        {
            switch (type)
            {
                case AbilityType.Acceleration : Acceleration = value; break;
                case AbilityType.Fatigue : Fatigue = value; break;
                case AbilityType.Speed : Speed = value; break;
                case AbilityType.Strength : Strength = value; break;
                default: break;
            }
        }
    }
}
