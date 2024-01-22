using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grids
{
    [Serializable]
    public class Edge<TCell, TVertex>
    {
        public TCell Cell1 { get; set; }

        public TCell Cell2 { get; set; }

        public TVertex Vertex1 { get; set; }

        public TVertex Vertex2 { get; set; }

        public bool IsBorder(Func<TCell,object> func) => Cell1 != null && Cell2 != null && !func(Cell1).Equals(func(Cell2));

    }
}
