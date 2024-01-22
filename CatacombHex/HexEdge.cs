using Grids;

namespace CatacombHex
{
    [System.Serializable]
    public class HexEdge : Edge<HexCell, HexVertex>
    {
        public bool _walled;

        public bool Walled
        {
            get => _walled;

            set
            {

                bool passable1 = Cell1 != null && Cell1.Passable;
                bool passable2 = Cell2 != null && Cell2.Passable;

                 _walled = value;

                if (!passable1 && !passable2)
                {
                    _walled = false; ;
                }
                if (passable1 != passable2)
                {
                    _walled = true; ;
                }
            }
        }
    }
}
