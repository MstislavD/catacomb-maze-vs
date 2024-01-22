using System;

namespace Mathematics
{
    public struct Angle
    {
        public static Angle FromRadian(double radian) => new Angle(radian);

        public static Angle FromDegree(double degree) => new Angle(degree / 180 * Math.PI);

        public static Angle FromVectors(Vector vector1, Vector vector2) => new Angle(vector1, vector2);

        public static Angle RightAngle => FromDegree(90);

        public static Angle StraightAngle => FromDegree(180);

        public static Angle operator -(Angle angle) => new Angle(-angle.Rad);

        public static Angle operator -(Angle angle1, Angle angle2) => new Angle(angle1.Rad - angle2.Rad);

        public static Angle operator +(Angle angle1, Angle angle2) => new Angle(angle1.Rad + angle2.Rad);

        Angle(double radian)
        {
            Rad = radian;
        }

        Angle(Vector vector1, Vector vector2)
        {
            double dot = vector1.DotProduct(vector2);
            double cos = dot / vector1.Length / vector2.Length;
            Rad = Math.Acos(cos);
        }

        public double Rad;

        public double Degree => Rad / Math.PI * 180;

        public double Cos => Math.Cos(Rad);

        public override string ToString()
        {
            return $"{Degree}\u00B0C";
        }
    }
}
