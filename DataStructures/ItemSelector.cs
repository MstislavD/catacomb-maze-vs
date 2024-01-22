using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public interface IItemSelector<T>
    {
        T GetNextItem();
        void RemovePreviousItem();
        void AddItems(IEnumerable<T> items);
        void AddItem(T item);
        bool IsNotEmpty();
        void ProcessNextItem();
    }
}
