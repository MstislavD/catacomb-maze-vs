using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mathematics;

namespace Grids
{
    public interface ITopology
    {
        double XDim { get; }
        double YDim { get; }
        int IndexOfNeighbor0(int x, int y, int evenRow, int cellIndex);
        int IndexOfNeighbor1(int x, int cellIndex);
        int IndexOfNeighbor2(int x, int y, int evenRow, int cellIndex);
        bool XWarped { get; }
        bool YWarped { get; }
        double GetDistance(double x1, double y1, double x2, double y2);
        double GetDistance2(double x1, double y1, double x2, double y2);
        double[] GetClosest(double x1, double y1, double x2, double y2);
        double[] Centrify(double x, double y);
        Angle GetDirection(Vector vector);
        Angle GetDirection(double x1, double y1, double x2, double y2);
    }
}
