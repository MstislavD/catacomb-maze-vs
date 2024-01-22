using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rendering
{
    public enum EdgeThickness { THIN, THICK, VERYTHICK }

    public class EdgeSet<TEdge>
    {
        public TEdge[] Edges { get; set; }
        public Color Color { get; set; }
        public EdgeThickness Thickenss { get; set; }

        public EdgeSet(IEnumerable<TEdge> edges, Color color, EdgeThickness thickness)
        {
            Edges = edges.ToArray();
            Color = color;
            Thickenss = thickness;
        }
    }
}
