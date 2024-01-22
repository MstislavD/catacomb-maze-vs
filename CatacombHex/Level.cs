using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grids;
using DataStructures;
using System.Runtime.Serialization;

namespace CatacombHex
{
    public enum LevelMode { Edit, Play }

    public enum GameObject { Empty, Clear, Entrance, Exit, Key, LightOrb }

    [Serializable]
    public class Level
    {
        HexGrid _grid;
        LevelMode _mode;
        GameObject _object;
        Dictionary<GameObject, HexCell> _cellByObject;
        HexCell _character;
        Random _random;
        int _moves;
        bool _hasKey;
        int _orbs;

        public const double VisibilityIncrement = 0.5;

        const double _redundancy = 0;
        const int _mazeSize = 100;
        const double _orbDensity = 0.1;

        GameObject[] _essentialObjects = { GameObject.Entrance, GameObject.Exit, GameObject.Key };

        public Level()
        {
            int gridWidth = 24;
            int gridHeight = 15;

            _random = new Random();

            _grid = new HexGrid(gridWidth, gridHeight, Topology.Flat, 0);

            _cellByObject = new Dictionary<GameObject, HexCell>
            {
                [GameObject.Entrance] = null,
                [GameObject.Exit] = null,
                [GameObject.Key] = null
            };
        }

        public HexGrid Grid => _grid;

        public int Moves => _moves;

        public bool HasKey => _hasKey;

        public event Action<string> MessageSent;

        public event Action LevelFinished;

        public void ProcessClick(HexCell cell, HexEdge edge)
        {
            if (cell != null)
            {
                if (_mode == LevelMode.Edit)
                {
                    _processEditClick(cell, edge);
                }
                else
                {
                    _processPlayClick(cell, edge);
                }
            }
        }

        private void _processEditClick(HexCell cell, HexEdge edge)
        {
            if (edge == null)
            {
                if (_object == GameObject.Empty)
                {
                    cell.Passable = !cell.Passable;
                    if (!cell.Passable)
                    {
                        _resetCell(cell);
                    }
                }
                else if (_object == GameObject.LightOrb)
                {
                    _resetCell(cell);
                    cell.SetGameObject(GameObject.LightOrb);
                }
                else if (_object == GameObject.Clear)
                {
                    _resetCell(cell);
                    cell.RemoveGameObject(cell.Object);
                }
                else
                {
                    if (!cell.HasGameObject(_object) && cell.Passable)
                    {
                        _cellByObject[_object]?.RemoveGameObject(_object);
                        cell.SetGameObject(_object);
                        _resetCell(cell);
                        _cellByObject[_object] = cell;
                    }
                }
            }
            else
            {
                edge.Walled = !edge.Walled;
            }
        }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            MessageSent = null;
            LevelFinished = null;
        }

        void _processPlayClick(HexCell cell, HexEdge edge)
        {
            int direction = _character.GetDirection(cell);
            if (!cell.Passable || direction == -1 || _character.GetEdge(direction).Walled)
            {
                return;
            }

            _character = cell;
            _moves++;

            if (cell.HasGameObject(GameObject.Key))
            {
                _hasKey = true;
                cell.RemoveGameObject(GameObject.Key);
                _cellByObject[GameObject.Key] = null;
                MessageSent?.Invoke("You found the key!");
            }

            if (cell.HasGameObject(GameObject.LightOrb))
            {
                cell.RemoveGameObject(GameObject.LightOrb);
                _orbs += 1;
            }

            if (cell == _cellByObject[GameObject.Exit])
            {
                if (_hasKey)
                {
                    MessageSent?.Invoke($"You found the exit and opened the door with the key. Level finished! Moves made: {_moves}");
                    LevelFinished?.Invoke();
                }
                else
                {
                    MessageSent?.Invoke("You found the exit but you need the key to open the door.");
                }
            }
        }

        public int Orbs => _orbs;

        public void SetMode(LevelMode mode)
        {
            _mode = mode;
            if (mode == LevelMode.Play)
            {
                List<HexCell> cells = _grid.Cells.Where(c => (c.Passable && c.HasGameObject(GameObject.Empty))).ToList();

                foreach (GameObject obj in _essentialObjects)
                {
                    if (_cellByObject[obj] == null || !_cellByObject[obj].Passable)
                    {
                        _cellByObject[obj] = cells.Extract(_random);
                        _cellByObject[obj].SetGameObject(obj);
                    }
                }

                _character = _cellByObject[GameObject.Entrance];
                _moves = 0;
                _hasKey = false;
                _orbs = 0;

                MessageSent?.Invoke("The exit portal can only be opened by a magical key!");
            }
        }

        public void SetGameObject(GameObject gameObject) => _object = gameObject;

        public bool IsPlayable()
        {
            int emptyCellsRequired = 3;
            foreach (GameObject obj in _essentialObjects)
            {
                emptyCellsRequired -= _cellByObject[obj] == null ? 0 : 1;
            }

            return _grid.Cells.Where(c => c.Passable && c.HasGameObject(GameObject.Empty)).Count() >= emptyCellsRequired;
        }

        public HexCell Character => _character;

        public void Generate()
        {
            Reset();

            HexCell firstCell = _grid.GetHex(0.5, 0.5);

            HashSet<HexCell> includedCells = new HashSet<HexCell>();
            Bag<HexCell> possibleCells = new Bag<HexCell>();
            possibleCells.Add(firstCell);

            Func<HexCell, bool> candidate = cell => !includedCells.Contains(cell) && cell.X > 0 && cell.Y > 0 && cell.X < _grid.Columns - 1 && cell.Y < Grid.Rows - 1;

            while (includedCells.Count < _mazeSize)
            {
                HexCell nextCell = possibleCells.Extract(_random);
                nextCell.Passable = true;
                includedCells.Add(nextCell);
                possibleCells.AddRange(nextCell.GetNeighbors().Where(candidate));
                foreach(HexEdge edge in nextCell.GetEdges())
                {
                    edge.Walled = true;
                }
            }

            HashSet<HexCell> connected = new HashSet<HexCell>() { firstCell };
            possibleCells = new Bag<HexCell>();
            possibleCells.AddRange(firstCell.GetNeighbors().Where(c => c.Passable));

            while (connected.Count < includedCells.Count)
            {
                HexCell nextCell = possibleCells.Extract(_random);
                connected.Add(nextCell);
                possibleCells.AddRange(nextCell.GetNeighbors().Where(c => c.Passable && !connected.Contains(c)));
                List<HexEdge> edges = nextCell.GetNeighbors().Where(connected.Contains).Select(nextCell.GetEdge).ToList();
                edges.Extract(_random).Walled = false;
            }

            List<HexEdge> walls = Grid.Edges.Where(e => e.Cell1.Passable && e.Cell2.Passable && e.Walled).ToList();
            int excessWalls = (int)(walls.Count * _redundancy);

            for (int i = 0; i < excessWalls; i++)
            {
                walls.Extract(_random).Walled = false;
            }

            List<HexCell> cells = _grid.Cells.Where(c => c.Passable).ToList();

            foreach (GameObject obj in _essentialObjects)
            {
                _cellByObject[obj] = cells.Extract(_random);
                _cellByObject[obj].SetGameObject(obj);
            }

            int orbCount = (int)(_mazeSize * _orbDensity);

            for (int i = 0; i < orbCount; i++)
            {
                cells.Extract(_random).SetGameObject(GameObject.LightOrb);
            }

        }

        void _resetCell(HexCell cell)
        {
            foreach (GameObject obj in _essentialObjects)
            {
                if (_cellByObject[obj] == cell)
                {
                    _cellByObject[obj] = null;
                }
            }
        }

        public void Reset()
        {
            foreach (HexCell cell in _grid.Cells)
            {
                cell.Passable = false;
                _resetCell(cell);
            }
        }


        
    }
}
