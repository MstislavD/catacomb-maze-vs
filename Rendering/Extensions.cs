using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Rendering
{
    public static class Extensions
    {
        public static Color NextColor (this Random random)
        {
            int red = random.Next(256);
            int green = random.Next(256);
            int blue = random.Next(256);

            return Color.FromArgb(red, green, blue);
        }

        public static Color Lerp(Color endColor, Color beginColor, double arg)
        {
            int red = (int)(arg * (endColor.R - beginColor.R) + beginColor.R);
            int green = (int)(arg * (endColor.G - beginColor.G) + beginColor.G);
            int blue = (int)(arg * (endColor.B - beginColor.B) + beginColor.B);

            return Color.FromArgb(red, green, blue);
        }
    }
}
