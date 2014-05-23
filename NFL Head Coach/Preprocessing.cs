using System;
//using System.Math;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace NFL_Head_Coach
{
    class Preprocessing
    {
        // Average Filtering
        public static void AvgSmoothing(StrokeCollection myStrokeCollection)
        {
            for (int cnt = 0; cnt < myStrokeCollection.Count; cnt++)
            {
                for (int index = 1; index < (myStrokeCollection[cnt].StylusPoints.Count - 1); index++)
                {
                    double x, y;

                    x = (myStrokeCollection[cnt].StylusPoints[index - 1].X + myStrokeCollection[cnt].StylusPoints[index].X + myStrokeCollection[cnt].StylusPoints[index + 1].X) / 3;
                    y = (myStrokeCollection[cnt].StylusPoints[index - 1].Y + myStrokeCollection[cnt].StylusPoints[index].Y + myStrokeCollection[cnt].StylusPoints[index + 1].Y) / 3;

                    myStrokeCollection[cnt].StylusPoints[index] = new StylusPoint(x, y);
                }
            }
        }
        public static void AvgSmoothing(Stroke myStroke)
        {
            for (int index = 1; index < (myStroke.StylusPoints.Count - 1); index++)
            {
                double x, y;

                x = (myStroke.StylusPoints[index - 1].X + myStroke.StylusPoints[index].X + myStroke.StylusPoints[index + 1].X) / 3;
                y = (myStroke.StylusPoints[index - 1].Y + myStroke.StylusPoints[index].Y + myStroke.StylusPoints[index + 1].Y) / 3;

                myStroke.StylusPoints[index] = new StylusPoint(x, y);
            }
        }

        // Not Finished
        public static void GaussianSmoothing(StrokeCollection myStrokeCollection, float delta)
        {
            const double e = Math.E;
            StrokeCollection myCloneStrokeCollection = myStrokeCollection.Clone();
            //
        }

        // Resampling all the strokes with the same distance intervals
        /*
        public static void Resampling(Stroke myStroke, double scaleFactor, Rect myBounds)
        {
            double diagonalLength = GeometryRelation.Distance(myBounds.BottomLeft, myBounds.TopRight);
            double S = diagonalLength / scaleFactor;
            //MessageBox.Show("S = "+S);
            //
            double D = 0;
            StylusPointCollection resampledStylusPointsCollection = new StylusPointCollection();


            // Add the first point to resampled stylus points
            resampledStylusPointsCollection.Add(myStroke.StylusPoints[0]);
            for (int i = 1; i < myStroke.StylusPoints.Count; i++)
            {
                StylusPoint p1, p2;
                p1 = myStroke.StylusPoints[i - 1];
                p2 = myStroke.StylusPoints[i];
                double d = GeometryRelation.Distance(p1, p2);
                if ((D + d) >= S)
                {
                    StylusPoint newPoint = new StylusPoint(p1.X + ((S - D) / d) * (p2.X - p1.X), p1.Y + ((S - D) / d) * (p2.Y - p1.Y));
                    resampledStylusPointsCollection.Add(newPoint);
                    myStroke.StylusPoints[i] = newPoint;
                    D = 0;
                }
                else
                {
                    D = D + d;
                }
            }
            myStroke.StylusPoints = resampledStylusPointsCollection;
        }
        */ 
        public static void Resampling(Stroke myStroke, int n)
        {
            double S =  Length(myStroke) / (n-1);
            double D = 0;
            StylusPointCollection resampledStylusPointsCollection = new StylusPointCollection();


            // Add the first point to resampled stylus points
            resampledStylusPointsCollection.Add(myStroke.StylusPoints[0]);
            for (int i = 1; i < myStroke.StylusPoints.Count; i++)
            {
                StylusPoint p1, p2;
                p1 = myStroke.StylusPoints[i - 1];
                p2 = myStroke.StylusPoints[i];
                double d = Distance(p1, p2);
                if ((D + d) >= S)
                {
                    StylusPoint newPoint = new StylusPoint(p1.X + ((S - D) / d) * (p2.X - p1.X), p1.Y + ((S - D) / d) * (p2.Y - p1.Y));
                    resampledStylusPointsCollection.Add(newPoint);
                    myStroke.StylusPoints.Insert(i, newPoint);
                    //myStroke.StylusPoints[i] = newPoint;
                    D = 0;
                }
                else
                {
                    D = D + d;
                }
            }
            myStroke.StylusPoints = resampledStylusPointsCollection;
            if (myStroke.StylusPoints.Count == n - 1)
            {
                myStroke.StylusPoints.Add(new StylusPoint(myStroke.StylusPoints[myStroke.StylusPoints.Count - 1].X, myStroke.StylusPoints[myStroke.StylusPoints.Count - 1].Y));
            }
        }

        // Calculate the length of one stroke
        private static double Length(Stroke myStroke)
        {
            double length = 0;
            for (int i = 0; i < myStroke.StylusPoints.Count - 1; i++)
            {
                length += Distance(myStroke.StylusPoints[i], myStroke.StylusPoints[i + 1]);
            }
            return length;
        }

        private static double Distance(StylusPoint p1, StylusPoint p2)
        {
            return (Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)));
        }
    }
}
