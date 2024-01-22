using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grids
{
    public interface IFactory<TNode, TEdge>
    {
        TNode CreateRegion();
        TEdge CreateRegionEdge();
    }
}
