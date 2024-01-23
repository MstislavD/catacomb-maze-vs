using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grids;
using Rendering;
using CatacombHex;

namespace CatacombHexForm
{
    class HexGridAdapter : IGrid<HexCell, HexEdge, HexVertex>
    {
        HexGrid _grid;

        public HexGridAdapter(HexGrid grid)
        {
            _grid = grid;
        }

        float IGrid<HexCell, HexEdge, HexVertex>.XDim => (float)_grid.XDim;

        float IGrid<HexCell, HexEdge, HexVertex>.YDim => (float)_grid.YDim;

        float IGrid<HexCell, HexEdge, HexVertex>.ToroidalShift => (float)_grid.ToroidalShift;

        public IEnumerable<HexCell> Cells => _grid.Cells;

        public bool XWarped => _grid.XWarped;

        public bool YWarped => _grid.YWarped;

        public int CountCells => _grid.CountCells;

        public float VertexX(HexVertex vertex) => (float)vertex.X;

        public float VertexY(HexVertex vertex) => (float)vertex.Y;

        public IEnumerable<HexVertex> Vertices(HexCell cell) => cell.Vertices;

        public HexVertex[] Vertices(HexEdge edge) => new HexVertex[] { edge.Vertex1, edge.Vertex2 };

        public HexVertex GetCenter(HexCell cell) => cell.Center;
    }
}
