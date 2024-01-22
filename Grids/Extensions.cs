using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grids
{
    public static class Extensions
    {
        public static bool NodeIsBridge<TCell, TEdge, TQuality>(this IGraph<TCell, TEdge> graph, TCell node, Func<TCell, TQuality> quality)
        {
            TQuality nodeQuality = quality(node);
            HashSet<TCell> cells = new HashSet<TCell>(graph.GetNeighbors(node).Where(c => quality(c).Equals(nodeQuality)));
            if (cells.Count == 0)
            {
                return false;
            }

            TCell firstCell = cells.First();
            HashSet<TCell> visited = new HashSet<TCell>();
            visited.Add(firstCell);
            Queue<TCell> queue = new Queue<TCell>();
            queue.Enqueue(firstCell);

            while (queue.Count > 0)
            {
                TCell nextCell = queue.Dequeue();
                foreach (TCell neighbor in graph.GetNeighbors(nextCell).Where(cells.Contains).Where(c => !visited.Contains(c)))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            return cells.Count != visited.Count;

        }

        public static IEnumerable<TCell> GetNodesOfEdge<TCell, TEdge>(this IGraph<TCell, TEdge> graph, TEdge edge) => new TCell[] { graph.GetNode1(edge), graph.GetNode2(edge) };

        public static TEdge GetEdgeBetweenTwoNodes<TCell, TEdge>(this IGraph<TCell, TEdge> graph, TCell node1, TCell node2)
        {
            return graph.GetEdges(node1).First(e => graph.GetNode1(e).Equals(node2) || graph.GetNode2(e).Equals(node2));
        }

        public static double GetCellPerimeter<TCell, TEdge>(this IGraph<TCell, TEdge> graph, TCell node) => graph.GetEdges(node).Sum(e => graph.GetEdgeLength(e));
    }
}
