using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class ItemSelectorRandom<T>: IItemSelector<T>
    {
        Random _random;
        Func<T, bool> _canBeRemoved;
        List<T> _list;
        int _lastItemID;

        public ItemSelectorRandom(IEnumerable<T> items, Random random)
        {
            _list = items.ToList();
            _random = random;
        }

        public ItemSelectorRandom(IEnumerable<T> items, Func<T, bool> canBeRemoved, Random random)
        {
            _list = items.ToList();
            _canBeRemoved = canBeRemoved;
            _random = random;
        }

        public void ProcessNextItem()
        {
            int id = _random.Next(_list.Count);

            T item = _list[id];

            if (_canBeRemoved(item))
            {
                _list[id] = _list[_list.Count - 1];
                _list.RemoveAt(_list.Count - 1);
            }
        }

        public T GetNextItem()
        {
            _lastItemID = _random.Next(_list.Count);
            return _list[_lastItemID];
        }

        public void RemovePreviousItem()
        {
            _list[_lastItemID] = _list[_list.Count - 1];
            _list.RemoveAt(_list.Count - 1);
        }

        public bool IsNotEmpty() => _list.Count > 0;

        public void AddItems(IEnumerable<T> items)
        {
            _list.AddRange(items);
        }

        public void AddItem(T item)
        {
            _list.Add(item);
        }
    }
}
