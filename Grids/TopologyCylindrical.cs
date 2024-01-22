using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mathematics;

namespace Grids
{
    class TopologyCylindrical : ITopology
    {
        int _width, _height;
        double _xStep;

        public TopologyCylindrical(int width, int height, double xStep)
        {
            _width = width;
            _height = height;
            _xStep = xStep;
        }

        public double XDim => _width * _xStep;

        public double YDim => (_height * 3 + 1) * _xStep * 0.5 / Math.Sqrt(3);

        public bool XWarped => true;

        public bool YWarped => false;

        public double[] Centrify(double x, double y)
        {
            x = (XDim + x % XDim) % XDim;
            return new double[] { x, y };
        }

        public double[] GetClosest(double x1, double y1, double x2, double y2)
        {
            //if (Math.Abs(x2 - x1) > XDim / 2)
            //{
            //    x2 = x2 - XDim;
            //}

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

        public double GetDistance(double x1, double y1, double x2, double y2) => Math.Pow(GetDistance(x1, x2, y1, y2), 0.5);
        
        public double GetDistance2(double x1, double y1, double x2, double y2)
        {
            double dx = Math.Abs(x2 - x1);
            double dy = Math.Abs(y2 - y1);
            dx = dx % XDim;
            if (dx > XDim / 2)
            {
                dx = XDim - dx;
            }
            
            return Math.Pow(dx, 2) + Math.Pow(dy, 2);
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

            return result;
        }

        public Vector GetZeroAngleVector()
        {
            return new Vector(1, 0);
        }
    }
}
