using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;

namespace Grids
{
    public enum Selector { Order, Random, Distance }

    public class RegionGraph<TCell, TRegion, TEdge, TRegionEdge> : IGraph <TRegion, TRegionEdge>
    {
        List<TRegion> _regions;
        List<TRegionEdge> _regionEdges;
        Dictionary<TRegion, IItemSelector<TCell>> _neighborsByRegion;
        Dictionary<TRegion, RegionData> _dataByRegion;
        Dictionary<TRegionEdge, RegionEdgeData> _dataByRegionEdge;
        Dictionary<TCell, TRegion> _regionByCell;
        Dictionary<TEdge, TRegionEdge> _regionEdgeByCellEdge;

        IFactory<TRegion, TRegionEdge> _factory;
        IGraph<TCell, TEdge> _graph;
        ITopology _topology;

        class RegionData
        {
            public List<TCell> Cells;
            public List<TRegion> Neighbors;
            public List<TRegionEdge> RegionEdges;
            public double Size;
            public double CenterX;
            public double CenterY;
        }

        class RegionEdgeData
        {
            public TRegion Region1;
            public TRegion Region2;
            public List<TEdge> Edges;
            public double Length;
        }

        public RegionGraph(IFactory<TRegion, TRegionEdge> factory, IGraph<TCell, TEdge> graph, IEnumerable<TCell> centers, Random random, Selector regionSelector, Selector cellSelector)
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            _factory = factory;
            _graph = graph;
            _topology = _graph.GetTopology();

            _regions = new List<TRegion>();
            _regionEdges = new List<TRegionEdge>();
            _neighborsByRegion = new Dictionary<TRegion, IItemSelector<TCell>>();
            _dataByRegion = new Dictionary<TRegion, RegionData>();
            _dataByRegionEdge = new Dictionary<TRegionEdge, RegionEdgeData>();
            _regionByCell = new Dictionary<TCell, TRegion>();
            _regionEdgeByCellEdge = new Dictionary<TEdge, TRegionEdge>();           

            foreach (TCell cell in centers)
            {
                TRegion region = factory.CreateRegion();
                _regions.Add(region);
                _dataByRegion[region] = new RegionData();
                _dataByRegion[region].Cells = new List<TCell>();
                _dataByRegion[region].Neighbors = new List<TRegion>();
                _dataByRegion[region].RegionEdges = new List<TRegionEdge>();
                _addCellToRegion(cell, region);
                if (cellSelector == Selector.Random)
                {
                    _neighborsByRegion[region] = new ItemSelectorRandom<TCell>(_graph.GetNeighbors(cell), random);
                }
                else if (cellSelector == Selector.Distance)
                {
                    TCell regionCenter = cell;
                    _neighborsByRegion[region] = new ItemSelectorPriority<TCell>(_graph.GetNeighbors(cell), c => _getDistance2(regionCenter, c));
                }
            }

            IItemSelector<TRegion> growableRegions;

            if (regionSelector == Selector.Random)
            {
                growableRegions = new ItemSelectorRandom<TRegion>(_regions, random);
            }
            else if (cellSelector == Selector.Distance && regionSelector == Selector.Distance)
            {
                growableRegions = new ItemSelectorPriority<TRegion>(_regions, _regionPriority);
            }
            else
            {
                regionSelector = Selector.Order;
                growableRegions = new ItemSelectorOrder<TRegion>(_regions);
            }

            while (growableRegions.IsNotEmpty())
            {
                TRegion region = growableRegions.GetNextItem();
                IItemSelector<TCell> neighbors = _neighborsByRegion[region];

                if (regionSelector != Selector.Distance)
                {
                    bool cellAdded = false;
                    while (neighbors.IsNotEmpty() && !cellAdded)
                    {
                        TCell neighborCell = neighbors.GetNextItem();
                        neighbors.RemovePreviousItem();

                        if (_cellNotInRegion(neighborCell))
                        {
                            _addCellToRegion(neighborCell, region);
                            cellAdded = true;
                            neighbors.AddItems(_graph.GetNeighbors(neighborCell).Where(_cellNotInRegion));
                        }                        
                    }

                    if (!cellAdded)
                    {
                        growableRegions.RemovePreviousItem();
                    }
                }

                else
                {
                    TCell neighborCell = neighbors.GetNextItem();
                    neighbors.RemovePreviousItem();

                    if (_cellNotInRegion(neighborCell))
                    {
                        _addCellToRegion(neighborCell, region);
                        neighbors.AddItems(_graph.GetNeighbors(neighborCell).Where(_cellNotInRegion));
                    }                  

                    growableRegions.RemovePreviousItem();

                    if (neighbors.IsNotEmpty())
                    {
                        growableRegions.AddItem(region);
                    }                   
                }               
            }

            foreach (TEdge edge in _graph.GetEdges())
            {
                TCell cell1 = _graph.GetNode1(edge);
                TCell cell2 = _graph.GetNode2(edge);                
  
                if (cell2 != null)
                {
                    TRegion region1 = _regionByCell[cell1];
                    TRegion region2 = _regionByCell[cell2];

                    if (!region1.Equals(region2))
                    {
                        _makeNeighbors(region1, region2, edge);
                    }
                }               
            }

            foreach (TRegion region in _regions)
            {
                _findRegionCenter(region);
            }

            Console.WriteLine($"Region graph generated in {sw.ElapsedMilliseconds} ms");

        }

        string _cellCenter(TCell cell) => $"({_graph.GetCenter(cell)[0]}, {_graph.GetCenter(cell)[1]})";

        void _makeNeighbors(TRegion region1, TRegion region2, TEdge edge)
        {
            TRegionEdge regionEdge;

            if (!_dataByRegion[region1].Neighbors.Contains(region2))
            {
                _dataByRegion[region1].Neighbors.Add(region2);
                _dataByRegion[region2].Neighbors.Add(region1);
                regionEdge = _factory.CreateRegionEdge();
                _regionEdges.Add(regionEdge);
                _dataByRegionEdge[regionEdge] = new RegionEdgeData();
                _dataByRegionEdge[regionEdge].Region1 = region1;
                _dataByRegionEdge[regionEdge].Region2 = region2;
                _dataByRegionEdge[regionEdge].Edges = new List<TEdge>();
                _dataByRegion[region1].RegionEdges.Add(regionEdge);
                _dataByRegion[region2].RegionEdges.Add(regionEdge);
            }
            else
            {
                regionEdge = _dataByRegion[region1].RegionEdges.First(e => _dataByRegionEdge[e].Region1.Equals(region2) || _dataByRegionEdge[e].Region2.Equals(region2));
            }

            _dataByRegionEdge[regionEdge].Edges.Add(edge);
            _dataByRegionEdge[regionEdge].Length += _graph.GetEdgeLength(edge);
            _regionEdgeByCellEdge[edge] = regionEdge;

        }

        double _regionPriority(TRegion region)
        {
            IItemSelector<TCell> selector = _neighborsByRegion[region];
            TCell nextCell = selector.GetNextItem();
            double priority = ((ItemSelectorPriority<TCell>)selector).GetNextValue();
            return priority;

        }

        double _getDistance2(TCell cell1, TCell cell2)
        {
            double[] center1 = _graph.GetCenter(cell1);
            double[] center2 = _graph.GetCenter(cell2);
            return _topology.GetDistance2(center1[0], center1[1], center2[0], center2[1]);
        }

        bool _cellNotInRegion(TCell cell) => !_regionByCell.ContainsKey(cell);

        void _addCellToRegion(TCell cell, TRegion region)
        {
            _regionByCell[cell] = region;
            _dataByRegion[region].Cells.Add(cell);
        }

        void _findRegionCenter(TRegion region)
        {
            double centerX = 0;
            double centerY = 0;
            double size = 0;

            foreach (TCell cell in _dataByRegion[region].Cells)
            {
                double cellSize = _graph.GetSize(cell);
                double[] center = _graph.GetCenter(cell);
                double[] closestCenter = _topology.GetClosest(centerX, centerY, center[0], center[1]);
                centerX = (centerX * size + closestCenter[0] * cellSize) / (size + cellSize);
                centerY = (centerY * size + closestCenter[1] * cellSize) / (size + cellSize);
                size += cellSize;
            }

            double[] centrified = _topology.Centrify(centerX, centerY);           

            _dataByRegion[region].Size = size;
            _dataByRegion[region].CenterX = centrified[0];
            _dataByRegion[region].CenterY = centrified[1];
        }

        public TRegion GetRegion(TCell cell) => _regionByCell[cell];

        public IEnumerable<TRegion> GetNodes() => _regions;

        public IEnumerable<TRegionEdge> GetEdges() => _regionEdges;

        public IEnumerable<TRegion> GetNeighbors(TRegion region) => _dataByRegion[region].Neighbors;

        public TRegion GetNode1(TRegionEdge regionEdge) => _dataByRegionEdge[regionEdge].Region1;

        public TRegion GetNode2(TRegionEdge regionEdge) => _dataByRegionEdge[regionEdge].Region2;

        public ITopology GetTopology() => _graph.GetTopology();

        public double[] GetCenter(TRegion node) => new double[] { _dataByRegion[node].CenterX, _dataByRegion[node].CenterY };

        public double GetSize(TRegion node) => _dataByRegion[node].Size;

        public int GetNodeCount() => _regions.Count;

        public TRegionEdge GetRegionEdge(TEdge edge) => _regionEdgeByCellEdge.ContainsKey(edge) ? _regionEdgeByCellEdge[edge] : default(TRegionEdge);

        public IEnumerable<TCell> GetCells() => _graph.GetNodes();

        public IEnumerable<TEdge> GetCellEdges() => _graph.GetEdges();

        public IGraph<TCell, TEdge> GetCellGraph() => _graph;

        public double GetEdgeLength(TRegionEdge edge) => _dataByRegionEdge[edge].Length;

        public IEnumerable<TRegionEdge> GetEdges(TRegion node) => _dataByRegion[node].RegionEdges;
    }
}
