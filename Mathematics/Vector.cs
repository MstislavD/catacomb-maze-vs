using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathematics
{
    public struct Vector : Point2D
    {
        public static Vector FromPolar(double radius, Angle angle)
        {
            double x = radius * Math.Cos(angle.Rad);
            double y = radius * Math.Sin(angle.Rad);

            return new Vector(x, y);
        }

        public double DotProduct(Vector vector) => X * vector.X + Y * vector.Y;
        public double Length => Math.Sqrt(X * X + Y * Y);
        public double X { get; set; }
        public double Y { get; set; }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector(double[] coords)
        {
            X = coords[0];
            Y = coords[1];
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }

        public static Vector operator +(Vector vector1, Vector vector2)
        {
            double x = vector1.X + vector2.X;
            double y = vector1.Y + vector2.Y;

            return new Vector(x, y);
        }

        public static Vector operator -(Vector vector1, Vector vector2)
        {
            double x = vector1.X - vector2.X;
            double y = vector1.Y - vector2.Y;

            return new Vector(x, y);
        }

        public static Vector operator /(Vector vector, double d)
        {
            double x = vector.X / d;
            double y = vector.Y / d;

            return new Vector(x, y);
        }
    }
}
