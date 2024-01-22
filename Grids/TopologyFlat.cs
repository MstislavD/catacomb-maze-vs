using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mathematics;

namespace Grids
{
    [Serializable]
    internal class TopologyFlat : ITopology
    {
        int _width, _height;
        double _xStep;            

        public TopologyFlat(int width, int height, double xStep)
        {
            _width = width;
            _height = height;
            _xStep = xStep;
        }
       
        public int IndexOfNeighbor0(int x, int y, int evenRow, int cellIndex)
        {
            return y > 0 && (x < _width - 1 || evenRow == 0) ? cellIndex - _width + evenRow : -1;
        }

        public int IndexOfNeighbor1(int x, int cellIndex)
        {
            return x < _width - 1 ? cellIndex + 1 : -1;
        }

        public int IndexOfNeighbor2(int x, int y, int evenRow, int cellIndex)
        {
            return y < _height - 1 && (x < _width - 1 || evenRow == 0) ? cellIndex + _width + evenRow : -1;
        }

        public double GetDistance(double x1, double y1, double x2, double y2) => Math.Pow(GetDistance2(x1, x2, y1, y2), 0.5);
    
        public double GetDistance2(double x1, double y1, double x2, double y2) => Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2);

        public double[] GetClosest(double x1, double y1, double x2, double y2) => new double[] { x2, y2 };

        public double[] Centrify(double x, double y) => new double[] { x, y };

        public Angle GetDirection(Vector vector) => GetDirection(0, 0, vector.X, vector.Y);

        public Angle GetDirection(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            Angle angle = Angle.FromVectors(new Vector(dx, dy), GetZeroAngleVector());

            if (y2 < y1)
            {
                angle = -angle;
            }

            return angle;
        }

        public virtual bool XWarped => false;

        public bool YWarped => false;

        public double XDim => (_width + 0.5) * _xStep;

        public double YDim => (_height * 3 + 1) * _xStep * 0.5 / Math.Sqrt(3);

        public Vector GetZeroAngleVector()
        {
            return new Vector(1, 0);
        }
    }
}
