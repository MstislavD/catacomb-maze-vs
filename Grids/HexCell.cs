using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grids
{
    [Serializable]
    public class HexCell<TVertex, TCell, TEdge>
    {
        TVertex[] _vertices;
        TVertex _center;
        TCell[] _neighbors;
        TEdge[] _edges;

        public HexCell()
        {
            _vertices = new TVertex[6];
            _neighbors = new TCell[6];
            _edges = new TEdge[6];
        }

        public void AddVertex(int direction, TVertex vertex)
        {
            _vertices[direction] = vertex;
        }

        public void AddNeighbor(int direction, TCell neighbor)
        {
            _neighbors[direction] = neighbor;
        }

        public void SetCenter(TVertex center)
        {
            _center = center;
        }

        public void AddEdge(int direction, TEdge edge) => _edges[direction] = edge;

        public TVertex Center => _center;

        public IEnumerable<TVertex> Vertices => _vertices;

        public TCell GetNeighbor(int direction) => _neighbors[direction];

        public TVertex GetVertex(int direction) => _vertices[direction];

        public TEdge GetEdge(int direction) => _edges[direction];

        public IEnumerable<TCell> GetNeighbors() => _neighbors.Where(c => c != null);

        public IEnumerable<TEdge> GetEdges() => _edges.Where(e => e != null);

        public TEdge GetEdge(TCell neighbor)
        {
            for (int i = 0; i < 6; i++)
            {
                if (_neighbors[i] != null && _neighbors[i].Equals(neighbor))
                {
                    return _edges[i];
                }
            }
            return default(TEdge);
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int GetDirection(TEdge edge)
        {
            for (int i = 0; i < 6; i++)
            {
                if (_edges[i].Equals(edge))
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetDirection(TCell cell)
        {
            for (int i = 0; i < 6; i++)
            {
                if (_neighbors[i].Equals(cell))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
