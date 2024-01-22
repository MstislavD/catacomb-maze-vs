using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public struct Interpolator
    {
        double[] _points;
        double[] _endValues;
        double[] _beginValues;

        public Interpolator(double[] points, double[] begin, double[] end)
        {
            _points = points;
            _beginValues = begin;
            _endValues = end;
        }

        public Interpolator(double[] points, double[] values)
        {
            _points = points;
            _beginValues = values;
            _endValues = values;
        }

        public double Sample(double arg)
        {
            for (int i = 1; i < _points.Length; i++)
            {
                if (arg < _points[i])
                {
                    double range = _points[i] - _points[i - 1];
                    double normalizedArg = (arg - _points[i - 1]) / range;
                    double valRange = _endValues[i] - _beginValues[i - 1];
                    double val = _beginValues[i - 1] + normalizedArg * valRange;
                    return val;
                }
            }

            return _points.Last();
        }
    }
}
