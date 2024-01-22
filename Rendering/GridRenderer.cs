using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections.Generic;
using Mathematics;

namespace Rendering
{
    public struct RendererData<TCell, TEdge, TVertex>
    {
        internal double Scale;
        public Bitmap Image { get; internal set; }
        internal GridRenderer<TCell, TEdge, TVertex> GridRenderer;
    }

    public class GridRenderer<TCell, TEdge, TVertex>
    {
        IGrid<TCell, TEdge, TVertex> _grid;
        float _yOverlap;
        float _scale;

        static public RendererData<TCell, TEdge, TVertex> GetRenderData(IGrid<TCell, TEdge, TVertex> grid, int imageWidth, int imageHeight, Dictionary<TCell, Color> colorByCell, IEnumerable<EdgeSet<TEdge>> eSets, IEnumerable<SymbolSet<TVertex>> sSets, TCell centerOfVisibility, float visibility)
        {
            GridRenderer<TCell, TEdge, TVertex> renderer = new GridRenderer<TCell, TEdge, TVertex>(grid);

            return renderer._rData(imageWidth, imageHeight, colorByCell, eSets, sSets, 0, false, centerOfVisibility, visibility);
        }

        static public Bitmap GetImage(IGrid<TCell, TEdge, TVertex> grid, int imageWidth, int imageHeight, Dictionary<TCell, Color> colorByCell, IEnumerable<EdgeSet<TEdge>> eSets, IEnumerable<SymbolSet<TVertex>> sSets)
        {
            GridRenderer<TCell, TEdge, TVertex> renderer = new GridRenderer<TCell, TEdge, TVertex>(grid);

            return renderer._rData(imageWidth, imageHeight, colorByCell, eSets, sSets, 0, false, default(TCell), 0).Image;
        }

        static public Bitmap GetImage(IGrid<TCell, TEdge, TVertex> grid, int imageWidth, int imageHeight, Dictionary<TCell, Color> colorByCell, IEnumerable<EdgeSet<TEdge>> eSets, IEnumerable<SymbolSet<TVertex>> sSets, float yOverlap, bool shiftLine)
        {
            GridRenderer<TCell, TEdge, TVertex> renderer = new GridRenderer<TCell, TEdge, TVertex>(grid);

            return renderer._rData(imageWidth, imageHeight, colorByCell, eSets, sSets, yOverlap, shiftLine, default(TCell), 0).Image;
        }

        static public RendererData<TCell, TEdge, TVertex> GetRenderData(IGrid<TCell, TEdge, TVertex> grid, int imageWidth, int imageHeight, Dictionary<TCell, Color> colorByCell, IEnumerable<EdgeSet<TEdge>> eSets, IEnumerable<SymbolSet<TVertex>> sSets, float yOverlap, bool shiftLine)
        {
            GridRenderer<TCell, TEdge, TVertex> renderer = new GridRenderer<TCell, TEdge, TVertex>(grid);

            return renderer._rData(imageWidth, imageHeight, colorByCell, eSets, sSets, yOverlap, shiftLine, default(TCell), 0);
        }

        static public void ModifyImage(RendererData<TCell, TEdge, TVertex> data, TCell cell, Color color, IEnumerable<EdgeSet<TEdge>> eSets)
        {
            data.GridRenderer._modifyImage(data, cell, color, eSets);
        }

        GridRenderer(IGrid<TCell, TEdge, TVertex> grid)
        {
            _grid = grid;
        }

        RendererData<TCell, TEdge, TVertex> _rData(
            int imageWidth,
            int imageHeight,
            Dictionary<TCell, Color> colorByCell,
            IEnumerable<EdgeSet<TEdge>> eSets,
            IEnumerable<SymbolSet<TVertex>> sSets,
            float yOverlap,
            bool shiftLine,
            TCell centerOfVisibility,
            float visibility)
        {
            imageWidth -= 1;
            imageHeight -= 1;

            _yOverlap = yOverlap;
            _scale = Math.Min(imageWidth / _grid.XDim, imageHeight / (_grid.YDim + _yOverlap));

            Bitmap image = new Bitmap((int)(_grid.XDim * _scale + 1), (int)((_grid.YDim + _yOverlap) * _scale + 1));

            Graphics g = Graphics.FromImage(image);

            foreach (TCell cell in _grid.Cells)
            {
                _drawCell(g, colorByCell[cell], cell);
            }

            foreach (EdgeSet<TEdge> edgeSet in eSets)
            {
                _drawEdgeSet(g, edgeSet);
            }

            foreach (SymbolSet<TVertex> symbolSet in sSets)
            {
                _drawSymbolSet(g, symbolSet);
            }

            if (shiftLine && _grid.YWarped)
            {
                _drawShiftLine(g);
            }

            _drawFogOfWar(centerOfVisibility, visibility, g, image.Width, image.Height);

            return new RendererData<TCell, TEdge, TVertex> { Scale = _scale, GridRenderer = this, Image = image }; ;
        }

        private void _drawFogOfWar(TCell centerOfVisibility, float visibility, Graphics g, float width, float height)
        {
            if (centerOfVisibility != null)
            {
                PointF[] cellPoints = _grid.Vertices(centerOfVisibility).Select(PointFromVertex).ToArray();

                float yStep = (cellPoints[3].Y - cellPoints[0].Y);
                float xStep = (cellPoints[1].X - cellPoints[5].X);

                float x1 = cellPoints[0].X;
                float x2 = cellPoints[5].X - xStep * visibility;
                float x3 = cellPoints[1].X + xStep * visibility;

                float y1 = cellPoints[1].Y - yStep * visibility * 0.5f;
                float y2 = cellPoints[0].Y - yStep * visibility;
                float y3 = cellPoints[2].Y + yStep * visibility * 0.5f;
                float y4 = cellPoints[3].Y + yStep * visibility;

                PointF[] p =
                {
                    new PointF(x2, y1),
                    new PointF(x1, y2),
                    new PointF(x3, y1),
                    new PointF(x3, y3),
                    new PointF(x1, y4),
                    new PointF(x2, y3),
                    new PointF(x2, y1),
                    new PointF(0, y1),
                    new PointF(0, height),
                    new PointF(width, height),
                    new PointF(width, 0),
                    new PointF(0, 0),
                    new PointF(0, y1)
                };

                g.FillPolygon(Brushes.Black, p);

            }
        }

        void _modifyImage(RendererData<TCell,TEdge,TVertex> data, TCell cell, Color color, IEnumerable<EdgeSet<TEdge>> eSets)
        {
            Graphics g = Graphics.FromImage(data.Image);

            _drawCell(g, color, cell);

            foreach (EdgeSet<TEdge> edgeSet in eSets)
            {
                _drawEdgeSet(g, edgeSet);
            }
        }

        private void _drawSymbolSet(Graphics g, SymbolSet<TVertex> symbolSet)
        {
            double cellWidth = Math.Sqrt(_grid.XDim * _grid.YDim / _grid.CountCells);
            float symbolSize = (float)cellWidth * _scale * symbolSet.Scale;
            Pen pen = new Pen(symbolSet.SecondaryColor);
            Brush brush = new SolidBrush(symbolSet.PrimalColor);
            if (symbolSet.Symbol == Symbol.Circle)
            {
                foreach (TVertex vertex in symbolSet.Vertices)
                {
                    _drawCircle(g, pen, brush, vertex, symbolSize);
                }
            }
            else if (symbolSet.Symbol == Symbol.Key)
            {
                foreach (TVertex vertex in symbolSet.Vertices)
                {
                    _drawKey(g, vertex, symbolSize);
                }
            }
            else if (symbolSet.Symbol == Symbol.Hex)
            {
                foreach (TVertex vertex in symbolSet.Vertices)
                {
                    _drawHex(g, vertex, symbolSet.PrimalColor, symbolSize);
                }
            }
            else if (symbolSet.Symbol == Symbol.Character)
            {
                foreach (TVertex vertex in symbolSet.Vertices)
                {
                    _drawCharacter(g, vertex, symbolSet.PrimalColor, symbolSize);
                }
            }
            else if (symbolSet.Symbol == Symbol.Direction)
            {
                foreach(TVertex vertex in symbolSet.Vertices)
                {
                    _drawDirectionSymbol(g, vertex, symbolSet.AngleByVertex[vertex], symbolSet.LengthByVertex[vertex], symbolSize);
                }
            }
        }

        private void _drawEdgeSet(Graphics g, EdgeSet<TEdge> edgeSet)
        {
            double cellWidth = Math.Sqrt(_grid.XDim * _grid.YDim /_grid.CountCells);

            double penThicknessFactor = 0;
            if (edgeSet.Thickenss == EdgeThickness.THICK) penThicknessFactor = 0.4f;
            if (edgeSet.Thickenss == EdgeThickness.VERYTHICK) penThicknessFactor = 0.8f;
            float penWidth = (float)(edgeSet.Thickenss == EdgeThickness.THIN ? 1.0 : Math.Pow(cellWidth * _scale, penThicknessFactor));

            Pen pen = new Pen(edgeSet.Color, penWidth);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            foreach (TEdge edge in edgeSet.Edges)
            {
                _drawEdge(g, pen, edge);
            }
        }

        private void _drawCircle(Graphics g, Pen pen, Brush brush, TVertex vertex, float symbolSize)
        {
            g.FillEllipse(brush, _grid.VertexX(vertex) * _scale - symbolSize / 2, _grid.VertexY(vertex) * _scale - symbolSize / 2, symbolSize, symbolSize);
            g.DrawEllipse(pen, _grid.VertexX(vertex) * _scale - symbolSize / 2, _grid.VertexY(vertex) * _scale - symbolSize / 2, symbolSize, symbolSize);
        }

        private void _drawDirectionSymbol(Graphics g, TVertex vertex, Angle angle, float length, float symbolSize)
        {
            g.FillEllipse(Brushes.Black, _grid.VertexX(vertex) * _scale - symbolSize / 2, _grid.VertexY(vertex) * _scale - symbolSize / 2, symbolSize, symbolSize);
            g.DrawEllipse(Pens.Black, _grid.VertexX(vertex) * _scale - symbolSize / 2, _grid.VertexY(vertex) * _scale - symbolSize / 2, symbolSize, symbolSize);

            Vector vector = Vector.FromPolar(length, angle);

            float x0 = _grid.VertexX(vertex) * _scale;
            float y0 = _grid.VertexY(vertex) * _scale;
            g.DrawLine(new Pen(Color.Black), x0, y0, x0 + (float)vector.X, y0 + (float)vector.Y);
        }

        private void _drawEdge(Graphics g, Pen pen, TEdge edge)
        {
            TVertex[] vertices = _grid.Vertices(edge);
            Action<float, float> drawCellAction = (xShift, yShift) => g.DrawLine(pen, PointFromVertex(vertices[0], xShift, yShift), PointFromVertex(vertices[1], xShift, yShift));
            _draw(g, drawCellAction, vertices);

        }

        private void _drawCell(Graphics g, Color color, TCell cell)
        {
            SolidBrush brush = new SolidBrush(color);
            IEnumerable<TVertex> _cellVertices = _grid.Vertices(cell);
            Action<float, float> drawCellAction = (xShift, yShift) => g.FillPolygon(brush, _cellVertices.Select(v => PointFromVertex(v, xShift, yShift)).ToArray());
            _draw(g, drawCellAction, _cellVertices);
        }

        private void _draw(Graphics g, Action<float,float> drawAction, IEnumerable<TVertex> vertices)
        {
            _draw(g, drawAction, vertices, 0, 0);
            if (_grid.YWarped && vertices.Any(v => _grid.VertexY(v) > _grid.YDim))
            {
                _draw(g, drawAction, vertices, -_grid.ToroidalShift, -_grid.YDim);
            }
            if (_grid.YWarped && vertices.Any(v => _grid.VertexY(v) < _yOverlap))
            {
                _draw(g, drawAction, vertices, _grid.ToroidalShift, _grid.YDim);
            }
        }

        private void _draw(Graphics g, Action<float, float> drawAction, IEnumerable<TVertex> vertices, float xShift0, float yShift0)
        {
            drawAction(xShift0, yShift0);
            if (_grid.XWarped && vertices.Any(v => _grid.VertexX(v) + xShift0 < 0))
            {
                drawAction(xShift0 + _grid.XDim, yShift0);
            }
            if (_grid.XWarped && vertices.Any(v => _grid.VertexX(v) + xShift0 > _grid.XDim))
            {
                drawAction(xShift0 - _grid.XDim, yShift0);
            }

        }

        private void _drawShiftLine(Graphics g)
        {
            List<Pen> pens = new List<Pen>();
            pens.Add(Pens.Black);
            pens.Add(Pens.White);

            for (int i = 0; i < pens.Count; i++)
            {
                g.DrawLine(pens[i], 0 + i, 0, _grid.ToroidalShift * _scale + i, _grid.YDim * _scale);
            }
        }

        PointF PointFromVertex(TVertex vertex) => new PointF(_grid.VertexX(vertex) * _scale, _grid.VertexY(vertex) * _scale);

        PointF PointFromVertex(TVertex vertex, float xShift, float yShift) => new PointF((_grid.VertexX(vertex) + xShift) * _scale, (_grid.VertexY(vertex) + yShift) * _scale);

        public PointF Lerp (PointF p1, PointF p2, float value)
        {
            PointF p = new PointF(0, 0);
            p.X = (p2.X - p1.X) * value + p1.X;
            p.Y = (p2.Y - p1.Y) * value + p1.Y;
            return p;
        }

        void _drawKey(Graphics g, TVertex vertex, float symbolSize)
        {
            float unit = symbolSize / 12;
            Pen pen = new Pen(Color.Black, 3);
            PointF center = PointFromVertex(vertex);
            g.DrawEllipse(pen, center.X - 2 * unit, center.Y - 4 * unit, unit * 4, unit * 4);
            g.DrawLine(pen, center.X, center.Y, center.X, center.Y + 4 * unit);
            g.DrawLine(pen, center.X, center.Y + 2 * unit, center.X + 2 * unit, center.Y + 2 * unit);
            g.DrawLine(pen, center.X, center.Y + 4 * unit, center.X + 2 * unit, center.Y + 4 * unit);
            g.FillRectangle(Brushes.Black, center.X, center.Y + 2 * unit, 1 * unit, 2 * unit);
        }

        void _drawHex(Graphics g, TVertex vertex, Color color, float symbolSize)
        {
            float unit = symbolSize / 12;
            Pen pen = new Pen(Color.Black, 3);
            PointF center = PointFromVertex(vertex);
            PointF[] p = new PointF[6];
            p[0] = new PointF(center.X, center.Y - 4 * unit);
            p[1] = new PointF(center.X + 3 * unit, center.Y - 2 * unit);
            p[2] = new PointF(center.X + 3 * unit, center.Y + 2 * unit);
            p[3] = new PointF(center.X, center.Y + 4 * unit);
            p[4] = new PointF(center.X - 3 * unit, center.Y + 2 * unit);
            p[5] = new PointF(center.X - 3 * unit, center.Y - 2 * unit);

            SolidBrush brush = new SolidBrush(color);
            g.FillPolygon(brush, p);
        }

        void _drawCharacter(Graphics g, TVertex vertex, Color color, float symbolSize)
        {
            float unit = symbolSize / 12;
            Pen pen = new Pen(Color.Black, 3);
            
            PointF center = PointFromVertex(vertex);
            PointF[] p = new PointF[3];
            p[0] = new PointF(center.X, center.Y - 4 * unit);
            p[1] = new PointF(center.X + 2 * unit, center.Y + 4 * unit);
            p[2] = new PointF(center.X - 2 * unit, center.Y + 4 * unit);

            SolidBrush brush = new SolidBrush(color);
            g.DrawPolygon(pen, p);
            g.DrawEllipse(pen, center.X - 2 * unit, center.Y - 4 * unit, 4 * unit, 4 * unit);
            g.FillPolygon(brush, p);
            g.FillEllipse(brush, center.X - 2 * unit, center.Y - 4 * unit, 4 * unit, 4 * unit);
        }
    }
}
