using System;
using System.Collections.Generic;
using System.Linq;

namespace Grids
{
    [Serializable]
    public class HexGrid<TCell, TEdge, TVertex> : IGraph<TCell, TEdge>, IHexGrid<TCell>
        where TCell : HexCell<TVertex, TCell, TEdge>, new()
        where TEdge : Edge<TCell, TVertex>, new()
        where TVertex : Vertex, new()
    {
        static protected double _xStep = 1;
        static protected double _ySmallStep = _xStep * 0.5 / Math.Sqrt(3);
        static double _sensitivity = 0.3;

        List<TCell> _cells;
        List<TEdge> _edges;
        protected int _width, _height, _toroidalShift;
        protected double _xDim, _yDim;
        ITopology _topology;

        public HexGrid(int width, int height, Topology topology, int toroidalShift)
        {
            _cells = new List<TCell>();
            _edges = new List<TEdge>();

            _width = width;
            _height = height;
            _toroidalShift = toroidalShift;

            if (topology == Topology.Flat)
            {
                _topology = new TopologyFlat(_width, _height, _xStep);
            }
            else if (topology == Topology.Cylindrical)
            {
                _topology = new TopologyCylindrical(_width, _height, _xStep);
            }
            else
            {
                _topology = new TopologyToroidal(_width, _height, toroidalShift, _xStep);
            }

            _xDim = _topology.XDim;
            _yDim = _topology.YDim;

            _runForEachCell(_createCell);
            _runForEachCell(_designateNeighbors);
            _runForEachCell(_addVertices);
            _runForEachCell(_createEdges);
        }

        public HexGrid(int width, int height, Topology topology) : this(width, height, topology, 0) { }

        // ABSTRACT BEGIN

        TCell _newCell(int x, int y) => new TCell() { X = x, Y = y };

        TVertex _newVertex(double x, double y) => new TVertex() { X = x, Y = y };

        TEdge _newEdge(TCell cell1, TCell cell2, int dir, TVertex vertex1, TVertex vertex2) => new TEdge() { Cell1 = cell1, Cell2 = cell2, Vertex1 = vertex1, Vertex2 = vertex2 };

        void _setCellCenter(TCell cell, TVertex center) => cell.SetCenter(center);

        void _setCoordinates(TVertex vertex, double x, double y)
        {
            vertex.X = x;
            vertex.Y = y;
        }

        void _addNeighbor(TCell cell, TCell neighbor, int direction) => cell.AddNeighbor(direction, neighbor);

        void _addVertexToCell(TCell cell, int direction, TVertex vertex) => cell.AddVertex(direction, vertex);

        void _addEdge(TCell cell, int direction, TEdge edge) => cell?.AddEdge(direction, edge);

        TCell _getNeighbor(TCell cell, int direction) => cell.GetNeighbor(direction);

        TVertex _getVertex(TCell cell, int direction) => cell.GetVertex(direction);

        TCell _getCell1(TEdge edge) => edge.Cell1;

        TCell _getCell2(TEdge edge) => edge.Cell2;

        IEnumerable<TCell> _getNeighbors(TCell cell) => cell.GetNeighbors();

        TVertex _getCenter(TCell cell) => cell.Center;

        double _getX(TVertex vertex) => vertex.X;

        double _getY(TVertex vertex) => vertex.Y;

        IEnumerable<TEdge> _getEdges(TCell cell) => cell.GetEdges();

        // ABSTRACT END

        void _createCell(int x, int y)
        {
            TCell cell = _newCell(x, y);
            _cells.Add(cell);

            int evenRow = y % 2;
            double centerX = (x + 0.5 + 0.5 * evenRow) * _xStep;
            double centerY = (y * 3 + 2) * _ySmallStep;
            _setCellCenter(cell, _newVertex(centerX, centerY));
        }

        void _designateNeighbors(int x, int y)
        {
            int evenRow = y % 2 == 0 ? 0 : 1;
            int cellIndex = x + y * _width;

            int cellIndex0 = cellIndex - _width + evenRow;
            int cellIndex1 = cellIndex + 1;
            int cellIndex2 = cellIndex + _width + evenRow;

            _markAsNeighbors(cellIndex, _topology.IndexOfNeighbor0(x, y, evenRow, cellIndex), 0);
            _markAsNeighbors(cellIndex, _topology.IndexOfNeighbor1(x, cellIndex), 1);
            _markAsNeighbors(cellIndex, _topology.IndexOfNeighbor2(x, y, evenRow, cellIndex), 2);
        }

        void _markAsNeighbors(int cellIndex1, int cellIndex2, int direction)
        {
            if (cellIndex2 > -1)
            {
                _addNeighbor(_cells[cellIndex1], _cells[cellIndex2], direction);
                _addNeighbor(_cells[cellIndex2], _cells[cellIndex1], direction + 3);
            }
        }

        void _addVertices(int x, int y)
        {
            TCell cell = _cells[x + y * _width];
            int evenRow = y % 2 == 0 ? 0 : 1;

            TVertex up = _addVertexToCell(cell, 0, cell.Center.X, cell.Center.Y - 2 * _ySmallStep);
            TVertex down = _addVertexToCell(cell, 3, cell.Center.X, cell.Center.Y + 2 * _ySmallStep);

            if (y == 0 || (x == _width - 1 && evenRow == 1))
            {
                _addVertexToCell(cell, 1, cell.Center.X + 0.5 * _xStep, cell.Center.Y - _ySmallStep);
            }
            else
            {
                cell.GetNeighbor(0).AddVertex(4, up);
            }

            if (y == 0 || (x == 0 && evenRow == 0))
            {
                _addVertexToCell(cell, 5, cell.Center.X - 0.5 * _xStep, cell.Center.Y - _ySmallStep);
            }
            else
            {
                cell.GetNeighbor(5).AddVertex(2, up);
            }

            if (y == _height - 1 || (x == _width - 1 && evenRow == 1))
            {
                _addVertexToCell(cell, 2, cell.Center.X + 0.5 * _xStep, cell.Center.Y + _ySmallStep);
            }
            else
            {
                cell.GetNeighbor(2).AddVertex(5, down);
            }

            if (y == _height - 1 || (x == 0 && evenRow == 0))
            {
                _addVertexToCell(cell, 4, cell.Center.X - 0.5 * _xStep, cell.Center.Y + _ySmallStep);
            }
            else
            {
                cell.GetNeighbor(3).AddVertex(1, down);
            }
        }

        TVertex _addVertexToCell(TCell cell, int direction, double x, double y)
        {
            TVertex vertex = _newVertex(x, y);
            _addVertexToCell(cell, direction, vertex);
            return vertex;
        }

        void _createEdges(int x, int y)
        {
            int cellIndex = x + y * _width;

            TCell cell = _cells[cellIndex];

            _createEdge(cell, 0);
            _createEdge(cell, 1);
            _createEdge(cell, 2);

            for (int i = 3; i < 6; i++)
            {
                if (!_cellHasNeighbor(cell, i))
                {
                    _createEdge(cell, i);
                }
            }
        }

        bool _cellHasNeighbor(TCell cell, int direction) => _getNeighbor(cell, direction) != null;

        void _createEdge(TCell cell, int dir)
        {
            TEdge edge = _newEdge(cell, _getNeighbor(cell, dir), dir, _getVertex(cell, dir), _getVertex(cell, (dir + 1) % 6));
            _addEdge(cell, dir, edge);
            _addEdge(_getCell2(edge), dir + 3, edge);         
            _edges.Add(edge);
        }

        public IEnumerable<TCell> Cells => _cells;

        public IEnumerable<TEdge> Edges => _edges;

        public double XDim => _xDim;

        public double YDim => _yDim;

        public double XStep => _xStep;

        public double YStep => _ySmallStep * 3;

        public bool XWarped => _topology.XWarped;

        public bool YWarped => _topology.YWarped;

        public int CountCells => _cells.Count;

        public double ToroidalShift => (_toroidalShift + (_height % 2 == 0 ? 0 : -0.5)) * _xStep;

        public IEnumerable<TCell> GetSublattice(int latticeParameter)
        {
            int a = latticeParameter;
            int b = latticeParameter * 2;
            int c = (latticeParameter + 1) / 2;

            List<TCell> sublattice = new List<TCell>();

            for (int j = 0; j < _height; j++)
            {
                if (j % b == 0)
                {
                    for (int i = 0; i < _width; i++)
                    {
                        if (i % a == 0)
                        {
                            sublattice.Add(_getCell(i, j));
                        }
                    }
                }
                else if (j % a == 0)
                {
                    for (int i = 0; i < _width; i++)
                    {
                        if ((i + c) % a == 0)
                        {
                            sublattice.Add(_getCell(i, j));
                        }
                    }
                }
            }

            return sublattice;
        }

        public void SmoothCellsFromProperty<T>(Func<TCell, T> property)
        {
            _runForEachCell((x, y) => _adjustCellVertices(x, y, property));
        }

        public IEnumerable<TCell> GetNeighborhood(TCell center, int radius)
        {
            HashSet<TCell> neighborhood = new HashSet<TCell>();
            _addNeighbors(center, radius, 0, neighborhood);
            return neighborhood;
        }

        public IEnumerable<TCell> GetNeighborhood16(TCell center)
        {
            HashSet<TCell> neighborhood = new HashSet<TCell>();
            Action<TCell> add = cell => { if (cell != null) { neighborhood.Add(cell); } };
            Action<TCell, int> addN = (cell, dir) => { if (cell != null && _getNeighbor(cell, dir) != null) { neighborhood.Add(_getNeighbor(cell, dir)); } };

            add(center);

            TCell n4 = _getNeighbor(center, 0);
            add(n4);
            addN(n4, 5);
            addN(n4, 0);
            addN(n4, 1);

            add(_getNeighbor(center, 1));

            TCell n12 = _getNeighbor(center, 2);
            add(n12);
            addN(n12, 1);
            addN(n12, 2);
            addN(n12, 3);

            TCell n11 = _getNeighbor(center, 3);
            add(n11);
            addN(n11, 2);
            addN(n11, 4);

            TCell n7 = _getNeighbor(center, 4);
            add(n7);
            addN(n7, 4);

            TCell n3 = _getNeighbor(center, 5);
            add(n3);
            addN(n3, 0);
            addN(n3, 4);

            return neighborhood;
        }

        TCell _getCell(int i, int j)
        {
            int index = _width * j + i;
            return _cells[index];
        }

        void _addNeighbors(TCell cell, int maxStep, int step, HashSet<TCell> used)
        {
            used.Add(cell);
            step += 1;
            if (step <= maxStep)
            {
                foreach (TCell neighbor in _getNeighbors(cell))
                {
                    _addNeighbors(neighbor, maxStep, step, used);
                }
            }
        }

        void _adjustCellVertices<T>(int x, int y, Func<TCell, T> property)
        {
            int evenRow = y % 2 == 0 ? 0 : 1;
            int cellIndex = x + y * _width;
            TCell cell = _cells[cellIndex];

            _adjustVertex(cell, 0, property);
            _adjustVertex(cell, 3, property);

            if ((x == _width - 1 && evenRow == 1) || y == 0)
            {
                _adjustVertex(cell, 1, property);
            }

            if ((x == _width - 1 && evenRow == 1) || y == _height - 1)
            {
                _adjustVertex(cell, 2, property);
            }

            if ((x == 0 && evenRow == 0) || y == _height - 1)
            {
                _adjustVertex(cell, 4, property);
            }

            if ((x == 0 && evenRow == 0) || y == 0)
            {
                _adjustVertex(cell, 5, property);
            }            
        }

        void _adjustVertex<T>(TCell cell, int vertexIndex, Func<TCell, T> property)
        {
            int direction0 = vertexIndex;
            int direction1 = (vertexIndex + 5) % 6;
            TCell neighbor0 = _getNeighbor(cell, direction0);
            TCell neighbor1 = _getNeighbor(cell, direction1);

            TVertex vertex = _getVertex(cell, vertexIndex);
            double distance = 0.25;

            if (neighbor0 != null && neighbor1 != null)
            {
                if (property(cell).Equals(property(neighbor0)) && !property(cell).Equals(property(neighbor1)))
                {
                    _moveVertex(vertex, _virtualNeighborCenter(cell, direction1), distance);
                }
                else if (property(cell).Equals(property(neighbor1)) && !property(cell).Equals(property(neighbor0)))
                {
                    _moveVertex(vertex, _virtualNeighborCenter(cell, direction0), distance);
                }
                else if (property(neighbor0).Equals(property(neighbor1)) && !property(cell).Equals(property(neighbor0)))
                {
                    _moveVertex(vertex, _getCenter(cell), distance);
                }
            }
        }
        
        TVertex _virtualNeighborCenter(TCell cell, int direction)
        {
            double x = _getX(_getCenter(cell));
            double y = _getY(_getCenter(cell));

            if (direction == 0)
            {
                x = x + _xStep / 2;
                y = y - _ySmallStep * 3;
            }
            else if (direction == 1)
            {
                x = x + _xStep;
            }
            else if (direction == 2)
            {
                x = x + _xStep / 2;
                y = y + _ySmallStep * 3;
            }
            else if (direction == 3)
            {
                x = x - _xStep / 2;
                y = y + _ySmallStep * 3;
            }
            else if (direction == 4)
            {
                x = x - _xStep;
            }
            else if (direction == 5)
            {
                x = x - _xStep / 2;
                y = y - _ySmallStep * 3;
            }

            return _newVertex(x, y);
        }

        void _moveVertex(TVertex vertex0, TVertex vertex1, double distance)
        {
            double x0 = _getX(vertex0);
            double x1 = _getX(vertex1);                       
            x0 = (x1 - x0) * distance + x0;

            double y0 = vertex0.Y;
            double y1 = vertex1.Y;
            y0 = (y1 - y0) * distance + y0;

            _setCoordinates(vertex0, x0, y0);
        }

        void _runForEachCell(Action<int, int> method)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    method(x, y);
                }
            }
        }

        public IEnumerable<TCell> GetNodes() => _cells;

        public IEnumerable<TEdge> GetEdges() => _edges;

        public IEnumerable<TCell> GetNeighbors(TCell node) => _getNeighbors(node);

        public TCell GetNode1(TEdge edge) => _getCell1(edge);

        public TCell GetNode2(TEdge edge) => _getCell2(edge);

        public double[] GetCenter(TCell node) => new double[] { _getX(_getCenter(node)), _getY(_getCenter(node)) };

        public double GetSize(TCell node) => 1;

        public int GetNodeCount() => _cells.Count;

        public ITopology GetTopology() => _topology;

        public double GetEdgeLength(TEdge edge) => 1;

        public IEnumerable<TEdge> GetEdges(TCell node) => _getEdges(node);

        public TCell GetHex(double x, double y)
        {
            int bm = XWarped ? 0 : 1;
            int cm = YWarped ? 0 : 1;

            int bandCount = 2 * _width + bm;
            double bandWidth = 1.0 / bandCount;
            int band = (int)(x / bandWidth);

            int cellCount = _height * 3 + cm;
            double cellHeight = 1.0 / cellCount;
            int cell = (int)(y / cellHeight);

            int rem1 = cell % 6;
            int rem2 = band % 2;

            int cellX = 0;
            int cellY = 0;

            if (rem1 == 0)
            {
                double xx = rem2 == 1 ? x - band * bandWidth : (band + 1) * bandWidth - x;
                double yy = y - cell * cellHeight;

                if (xx > yy)
                {
                    cellX = (band - 1) / 2;
                    cellY = cell / 3 - 1;
                }
                else
                {
                    cellX = band / 2;
                    cellY = cell / 3;
                }
            }
            else if (rem1 < 3)
            {
                cellX = band / 2;
                cellY = cell / 3;
            }
            else if (rem1 == 3)
            {
                double xx = rem2 == 1 ? (band + 1) * bandWidth - x : x - band * bandWidth;
                double yy = y - cell * cellHeight;

                if (xx > yy)
                {
                    cellX = band / 2;
                    cellY = cell / 3 - 1;
                }
                else
                {
                    cellX = (band + 1) / 2 - 1;
                    cellY = cell / 3;
                }
            }
            else
            {
                cellX = (band + 1) / 2 - 1;
                cellY = cell / 3;
            }

            return _checkCoords(cellX, cellY) ? _getCell(cellX, cellY) : null;
        }

        public TEdge GetEdge(double x, double y)
        {
            int bm = XWarped ? 0 : 1;
            int cm = YWarped ? 0 : 1;

            int bandCount = 2 * _width + bm;
            double bandWidth = 1.0 / bandCount;
            int band = (int)(x / bandWidth);

            int cellCount = _height * 3 + cm;
            double cellHeight = 1.0 / cellCount;
            int cell = (int)(y / cellHeight);

            int rem1 = cell % 6;
            int rem2 = band % 2;

            int direction;
            double dist = 0;

            int cellX = 0;
            int cellY = 0;

            if (rem1 == 0)
            {
                double xx = rem2 == 1 ? x - band * bandWidth : (band + 1) * bandWidth - x;
                double yy = y - cell * cellHeight;

                dist = Math.Abs(xx - yy) * Math.Cos(30);

                if (xx > yy)
                {
                    cellX = (band - 1) / 2;
                    cellY = cell / 3 - 1;
                    direction = 2 + rem2;
                }
                else
                {
                    cellX = band / 2;
                    cellY = cell / 3;
                    direction = 5 - rem2 * 5;
                }
            }
            else if (rem1 < 3)
            {
                cellX = band / 2;
                cellY = cell / 3;
                direction = 4 - rem2 * 3;
                dist = rem2 == 0 ? x - band * bandWidth : (band + 1) * bandWidth - x;
            }

            else if (rem1 == 3)
            {
                double xx = rem2 == 1 ? (band + 1) * bandWidth - x : x - band * bandWidth;
                double yy = y - cell * cellHeight;

                dist = Math.Abs(xx - yy) * Math.Cos(30);

                if (xx > yy)
                {
                    cellX = band / 2;
                    cellY = cell / 3 - 1;
                    direction = 3 - rem2;
                }
                else
                {
                    cellX = (band + 1) / 2 - 1;
                    cellY = cell / 3;
                    direction = rem2 * 5;
                }
            }
            else
            {
                cellX = (band + 1) / 2 - 1;
                cellY = cell / 3;
                direction = 1 + rem2 * 3;
                dist = rem2 == 1 ? x - band * bandWidth : (band + 1) * bandWidth - x;
            }

            dist = dist / bandWidth;

            return (_checkCoords(cellX, cellY) && dist < _sensitivity) ? _getCell(cellX, cellY).GetEdge(direction) : null;
        }

        bool _checkCoords(int x, int y) => x > -1 && x < _width && y > -1 && y < _height;

        public int Columns => _width;

        public int Rows => _height;

    }
}
