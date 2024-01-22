using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class ItemSelectorOrder<T> : IItemSelector<T>
    {
        Func<T, bool> _canBeRemoved;
        List<T> _list;
        int _currentID;

        public ItemSelectorOrder(IEnumerable<T> item)
        {
            _list = item.ToList();
            _currentID = -1;
        }

        public ItemSelectorOrder(IEnumerable<T> item, Func<T, bool> canBeRemoved)
        {
            _list = item.ToList();
            _canBeRemoved = canBeRemoved;
        }

        public void ProcessNextItem()
        {
            T item = _list[_currentID];

            if (_canBeRemoved(item))
            {
                _list[_currentID] = _list[_list.Count - 1];
                _list.RemoveAt(_list.Count - 1);
            }
            else
            {
                _currentID += 1;
            }

            if (_currentID >= _list.Count)
            {
                _currentID = 0;
            }
        }

        public bool IsNotEmpty() => _list.Count > 0;

        public T GetNextItem()
        {
            _currentID = _currentID + 1 >= _list.Count ? 0 : _currentID + 1;
            return _list[_currentID];
        }

        public void RemovePreviousItem()
        {
            _list[_currentID] = _list[_list.Count - 1];
            _list.RemoveAt(_list.Count - 1);
            _currentID -= 1;
        }

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
