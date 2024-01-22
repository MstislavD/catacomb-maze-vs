using System;
using Grids;

namespace CatacombHex
{
    [Serializable]
    public class HexCell : HexCell<HexVertex, HexCell, HexEdge>
    {
        bool _passable;
        GameObject _object;

        public bool Passable
        {
            get => _passable;

            set
            {
                _passable = value;

                if (!_passable)
                {
                    _object = GameObject.Empty;
                    for (int i = 0; i < 6; i++)
                    {                        
                        HexCell neighbor = GetNeighbor(i);
                        if (neighbor != null && !neighbor.Passable)
                        {
                            GetEdge(i).Walled = false;
                        }
                        else
                        {
                            GetEdge(i).Walled = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        HexCell neighbor = GetNeighbor(i);
                        if (neighbor == null || !neighbor.Passable)
                        {
                            GetEdge(i).Walled = true;
                        }
                        else
                        {
                            GetEdge(i).Walled = false;
                        }
                    }
                }
            }
        }

        public bool HasGameObject(GameObject gameObject)
        {
            return _object == gameObject;
        }

        public void SetGameObject(GameObject gameObject)
        {
            if (_passable)
            {
                _object = gameObject;
            }
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            if (_object == gameObject)
            {
                _object = GameObject.Empty;
            }
            return;
        }

        public GameObject Object => _object;
    }
}
