using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grids
{
    public interface IGraph<TNode, TEdge>
    {
        IEnumerable<TNode> GetNodes();
        IEnumerable<TEdge> GetEdges();
        IEnumerable<TNode> GetNeighbors(TNode node);
        TNode GetNode1(TEdge edge);
        TNode GetNode2(TEdge edge);
        ITopology GetTopology();
        double[] GetCenter(TNode node);
        double GetSize(TNode node);
        int GetNodeCount();
        double GetEdgeLength(TEdge edge);
        IEnumerable<TEdge> GetEdges(TNode node);
    }
}
