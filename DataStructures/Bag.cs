using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class Bag<T>
    {
        List<T> _list;
        Dictionary<T, int> _position;

        public Bag()
        {
            _list = new List<T>();
            _position = new Dictionary<T, int>();
        }

        public Bag(IEnumerable<T> set) : this()
        {
            foreach(T item in set)
            {
                    Add(item);                        
            }
        }

        public void Add(T item)
        {
            if (!_position.ContainsKey(item))
            {
                _position[item] = _list.Count;
                _list.Add(item);
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach(T item in items)
            {
                Add(item);
            }
        }

        public int Count => _list.Count;

        public T Extract(Random random)
        {
            int position = random.Next(_list.Count);
            T item = _list[position];
            _remove(item, position);
            return item;
        }

        public void Remove(T item)
        {
            if (_position.ContainsKey(item))
            {
                int position = _position[item];
                _remove(item, position);
            }
        }

        void _remove(T item, int position)
        {
            T last = _list.Last();
            _list[position] = last;
            _list.RemoveAt(_list.Count - 1);
            _position[last] = position;
            _position.Remove(item);
        }
    }
}
