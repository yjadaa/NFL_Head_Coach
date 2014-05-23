using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace NFL_Head_Coach
{
    class Path
    {
        public enum Status
        {
            Null = 0,
            Start = 1,
            End = 2
        }
        
        public Stroke Stroke;
        public int Index,Length;
        public Status CurrentStatus;

        public Path(Stroke s)
        {
            Stroke = s;
            Length = s.StylusPoints.Count;
            Index = 0;
            CurrentStatus = Status.Start;
        }
        public Path()
        {
            CurrentStatus = Status.Null;
        }

        public void Start()
        {
            CurrentStatus = Status.Start;
        }

        public Point GetPoint()
        {
            StylusPoint p = new StylusPoint();
            if (CurrentStatus == Status.Start)
            {
                if (Index < Length)
                {
                    p = Stroke.StylusPoints[Index];
                    Index++;
                }
                else
                {
                    p = Stroke.StylusPoints[Length - 1];
                }
            }
            return StylusPoint2Point(p);
        }

        private Point StylusPoint2Point(StylusPoint p)
        {
            return (new Point(p.X,p.Y));
        }
    }
}
