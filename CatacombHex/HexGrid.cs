using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grids;

namespace CatacombHex
{
    [Serializable]
    public class HexGrid : HexGrid<HexCell, HexEdge, HexVertex>
    {
        public HexGrid(int width, int height, Topology topology, int toroidalShift) : base(width, height, topology, toroidalShift) { }
    }
}
