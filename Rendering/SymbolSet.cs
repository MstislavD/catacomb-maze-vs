using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mathematics;

namespace Rendering
{
    public enum Symbol { Circle, Key, Hex, Character, Direction }

    public class SymbolSet<TVertex>
    {
        public SymbolSet(IEnumerable<TVertex> vertices, Symbol symbol)
        {
            Vertices = vertices.ToArray();
            Symbol = symbol;
            PrimalColor = Color.White;
            SecondaryColor = Color.Black;
            Scale = 1;
            AngleByVertex = new Dictionary<TVertex, Angle>();
        }

        public SymbolSet(IEnumerable<TVertex> vertices, Symbol symbol, Color primalColor) : this(vertices, symbol)
        {
            PrimalColor = primalColor;
        }

        public SymbolSet(IEnumerable<TVertex> vertices, Symbol symbol, Color primalColor, float scale) : this(vertices, symbol)
        {
            PrimalColor = primalColor;
            Scale = scale;
        }

        public SymbolSet(TVertex vertex, Symbol symbol, Color primalColor) : this(new TVertex[] { vertex }, symbol, primalColor) { }

        public TVertex[] Vertices { get; set; }
        public Symbol Symbol { get; set; }
        public Color PrimalColor { get; set; }
        public Color SecondaryColor { get; set; }
        public float Scale { get; set; }
        public Dictionary<TVertex, Angle> AngleByVertex { get; set; }
        public Dictionary<TVertex, float> LengthByVertex { get; set; }
    }
}
