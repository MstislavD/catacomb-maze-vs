using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using System.Drawing;

namespace Rendering
{
    public struct ColorInterpolator
    {
        Interpolator<Color> _interpolator;

        public ColorInterpolator(double[] points, Color[] beginColors, Color[] endColors)
        {
            _interpolator = new Interpolator<Color>(points, beginColors, endColors, Extensions.Lerp); // _func
        }

        public Color Sample(double arg) => _interpolator.Sample(arg);

        Color _func(Color endColor, Color beginColor, double arg)
        {
            int red = (int)(arg * (endColor.R - beginColor.R) + beginColor.R);
            int green = (int)(arg * (endColor.G - beginColor.G) + beginColor.G);
            int blue = (int)(arg * (endColor.B - beginColor.B) + beginColor.B);

            return Color.FromArgb(red, green, blue);
        }
    }
}
