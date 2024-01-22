using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public struct Interpolator<T>
    {
        double[] _points;
        T[] _endValues;
        T[] _beginValues;

        Func<T, T, double, T> _func;

        public Interpolator(double[] points, T[] begin, T[] end, Func<T, T, double, T> func)
        {
            _points = points;
            _beginValues = begin;
            _endValues = end;
            _func = func;
        }

        public Interpolator(double[] points, T[] values, Func<T, T, double, T> funcEndBeginArg) : this(points, values, values, funcEndBeginArg) { }

        public T Sample(double arg)
        {
            for (int i = 1; i < _points.Length; i++)
            {
                if (arg <= _points[i])
                {
                    double range = _points[i] - _points[i - 1];
                    double normalizedArg = (arg - _points[i - 1]) / range;
                    T val = _func(_endValues[i], _beginValues[i - 1], normalizedArg);
                    return val;
                }
            }

            return default(T);
        }
    }
}
