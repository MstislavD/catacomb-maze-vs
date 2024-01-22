using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grids
{
    public interface IHexGrid<TCell>
    {
        IEnumerable<TCell> GetSublattice(int latticeParameter);
        IEnumerable<TCell> GetNeighborhood(TCell center, int radius);
        IEnumerable<TCell> GetNeighborhood16(TCell center);
        void SmoothCellsFromProperty<T>(Func<TCell, T> func);
    }
}
