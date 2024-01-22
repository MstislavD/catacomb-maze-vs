using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathematics
{
    public static class Utility
    {
        public static Vector EllipsePoint(double ra, double rb, Angle a)
        {
            double tg = Math.Tan(a.Rad);
            double x = ra * rb / Math.Sqrt(rb * rb + ra * ra * tg * tg);
            x = Math.Sign(Math.Cos(a.Rad)) * x;
            double y = x * tg;
            return new Vector() { X = x, Y = y };
        }
    }
}
