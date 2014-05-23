using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFL_Head_Coach
{
    class PlayerBehavior : Ability
    {
        public enum MotionType 
        {
            Null = 0,
            Move = 1,
            Stop = 2,
            Throwing = 3
        }

        public void Run()
        {
        }

        public void Stop()
        { 
        }

        public void Defense()
        { 
        }
    }
}
