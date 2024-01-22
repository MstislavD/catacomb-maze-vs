using System.Collections.Generic;

namespace Rendering
{
    public interface IGrid<TCell, TEdge, TVertex>
    {
        IEnumerable<TCell> Cells { get; }
        float XDim { get; }
        float YDim { get; }
        IEnumerable<TVertex> Vertices(TCell cell);
        float VertexX(TVertex vertex);
        float VertexY(TVertex vertex);
        TVertex[] Vertices(TEdge edge);
        bool XWarped { get; }
        bool YWarped { get; }
        float ToroidalShift { get; }
        int CountCells { get; }
        TVertex GetCenter(TCell cell);
    }
}
