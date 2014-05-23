using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFL_Head_Coach.Base_Class
{
    class PlayerBehavior : Ability
    {
        enum MotionType 
        {
            Run =0,
            Stop = 1,
            Defense = 2
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
