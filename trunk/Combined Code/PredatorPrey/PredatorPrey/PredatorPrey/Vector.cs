using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PredatorPrey
{
    class Vector
    {
        public double X;
        public double Y;

        public static Vector UnitY = new Vector(0, 1);

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static double Dot(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static double Distance(Vector a, Vector b)
        {
            return Magnitude(Subtract(a, b));
        }

        public static Vector Subtract(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static double Magnitude(Vector a)
        {
            return Math.Sqrt(Math.Pow(a.X, 2) + Math.Pow(a.Y, 2));
        }

        public static Vector Normalize(Vector a)
        {
            double magnitude = Magnitude(a);

            if (magnitude == 0)
                return a;

            return new Vector(a.X / magnitude, a.Y / magnitude);
        }
    }
}
