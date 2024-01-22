using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mathematics;
using DataStructures;

namespace Grids
{
    public class TopologyToroidal : ITopology
    {
        int _width, _height;
        int _shift;
        double _xStep;
        double _toroidalShift;
        int _sizeMinusLastRow;
        int _size;
        double _xDim;
        double _yDim;

        public TopologyToroidal(double width, double height, double shift)
        {
            _xDim = width;
            _yDim = height;
            _toroidalShift = shift;
        }

        public TopologyToroidal(int width, int height, int shift, double xStep)
        {
            _width = width;
            _height = height;
            _xStep = xStep;
            _shift = shift;
            _sizeMinusLastRow = (_height - 1) * _width;
            _size = _height * _width;
            _toroidalShift = _xStep * _shift;
            _xDim = _width * _xStep;
            _yDim = _height * 3 * _xStep * 0.5 / Math.Sqrt(3);
        }

        public double XDim => _xDim;

        public double YDim => _yDim;

        public bool XWarped => true;

        public bool YWarped => true;

        public double GetDistance(double x1, double y1, double x2, double y2) => Math.Pow(GetDistance2(x1, y1, x2, y2), 0.5);

        public double GetDistance2(double x1, double y1, double x2, double y2)
        {
            double[] closest = GetClosest(x1, y1, x2, y2);
            return Math.Pow(x1 - closest[0], 2) + Math.Pow(y1 - closest[1], 2);
        }

        public int IndexOfNeighbor0(int x, int y, int evenRow, int cellIndex)
        {
            int result = -1;

            if (y > 0)
            {
                if (x < _width - 1 || evenRow == 0)
                {
                    result = cellIndex - _width + evenRow;
                }
                else
                {
                    result = cellIndex - _width - _width + evenRow;
                }
            }
            else
            {
                int delta = (cellIndex + _shift) % _width;
                result = _sizeMinusLastRow + delta;
            }

            return result;
        }

        public int IndexOfNeighbor1(int x, int cellIndex)
        {
            int mod = x < _width - 1 ? 0 : _width;
            int result = cellIndex + 1 - mod;
            return result;
        }

        public int IndexOfNeighbor2(int x, int y, int evenRow, int cellIndex)
        {
            int result = -1;

            if (y < _height - 1)
            {
                if (x < _width - 1 || evenRow == 0)
                {
                    result = cellIndex + _width + evenRow;
                }
                else
                {
                    result = cellIndex + 1;
                }
            }
            else
            {
                result = ((cellIndex + _width + 1) % _size - _shift + _width) % _width;
            }

            return result;
        }

        public double[] GetClosest_0(double x1, double y1, double x2, double y2)
        {
            double[] closest1 = _getClosest(x1, y1, x2, y2);
            double[] closest2 = _getClosest(x1, y1, x2 - _toroidalShift, y2 - YDim);
            double[] closest3 = _getClosest(x1, y1, x2 + _toroidalShift, y2 + YDim);

            double dist1 = Math.Pow(x1 - closest1[0], 2) + Math.Pow(y1 - closest1[1], 2);
            double dist2 = Math.Pow(x1 - closest2[0], 2) + Math.Pow(y1 - closest2[1], 2);
            double dist3 = Math.Pow(x1 - closest3[0], 2) + Math.Pow(y1 - closest3[1], 2);

            if (dist2 < dist1 && dist2 < dist3)
            {
                return closest2;
            }
            else if (dist3 < dist1)
            {
                return closest3;
            }
            else
            {
                return closest1;
            }
        }

        public double[] GetClosest(double x1, double y1, double x2, double y2)
        {
            List<double[]> candidates = new List<double[]>();
            candidates.Add(_getClosest(x1, y1, x2, y2));          

            double l = Math.Sqrt(XDim * XDim / 4 + YDim * YDim);

            double x = x2;
            double y = y2;

            while (y - y1 + YDim < l)
            {
                y += YDim;
                x += _toroidalShift;
                candidates.Add(_getClosest(x1, y1, x, y));
            }                     

            x = x2;
            y = y2;
            while (y1 - y + YDim < l)
            {
                y -= YDim;
                x -= _toroidalShift;
                candidates.Add(_getClosest(x1, y1, x, y));
            }

            return candidates.MinBy(c => _dist2(x1, y1, c[0], c[1]));
        }

        double[] _getClosest(double x1, double y1, double x2, double y2)
        {
            double halfWidth = XDim / 2;

            while (x1 - x2 > halfWidth)
            {
                x2 += XDim;
            }
            while (x2 - x1 > halfWidth)
            {
                x2 -= XDim;
            }

            return new double[] { x2, y2 };
        }

        double _dist2(double x1, double y1, double x2, double y2) => Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2);

        public double[] Centrify(double x, double y)
        {
            int step = (int)(y / YDim);
            if (y < 0)
            {
                step -= 1;
            }
            y = (YDim + y % YDim) % YDim;
            x = x - _toroidalShift * step;
            x = (XDim + x % XDim) % XDim;
            return new double[] { x, y };
        }

        public Angle GetDirection(Vector vector) => GetDirection(0, 0, vector.X, vector.Y);

        public Angle GetDirection(double x1, double y1, double x2, double y2)
        {
            double[] closestEndPoint = GetClosest(x1, y1, x2, y2);
            x2 = closestEndPoint[0];
            y2 = closestEndPoint[1];

            double dx = x2 - x1;
            double dy = y2 - y1;
            Angle angle = Angle.FromVectors(new Vector(dx, dy), GetZeroAngleVector());

            if (y2 < y1)
            {
                angle = -angle;
            }

            return angle;
        }

        public Vector GetZeroAngleVector()
        {
            return new Vector(1, 0);
        }
    }
}
