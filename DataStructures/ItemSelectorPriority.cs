using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class ItemSelectorPriority<T> : IItemSelector<T>
    {
        Func<T, double> _priority;
        Func<T, bool> _canBeRemoved;
        MinHeap<T> _priorityQueue;

        public ItemSelectorPriority(IEnumerable<T> items, Func<T, double> priority)
        {
            _priority = priority;
            _priorityQueue = new MinHeap<T>();

            AddItems(items);
        }

        public ItemSelectorPriority(IEnumerable<T> regions, Func<T, bool> canBeRemoved, Func<T,double> priority)
        {
            _priority = priority;
            _priorityQueue = new MinHeap<T>();
            _canBeRemoved = canBeRemoved;

            foreach(T item in regions)
            {
                _priorityQueue.SetValue(item, priority(item));
            }
        }

        public void ProcessNextItem()
        {
            T item = _priorityQueue.ExtractMin();

            if (!_canBeRemoved(item))
            {
                _priorityQueue.SetValue(item, _priority(item));
            }
        }

        public bool IsNotEmpty() => _priorityQueue.HeapSize > 0;

        public T GetNextItem()
        {
            return _priorityQueue.Minimum();
        }

        public void RemovePreviousItem()
        {
            _priorityQueue.ExtractMin();
        }

        public void AddItems(IEnumerable<T> items)
        {
            foreach(T item in items)
            {
                AddItem(item);
            }            
        }

        public double GetNextValue() => _priorityQueue.MinValue;

        public void AddItem(T item)
        {
            _priorityQueue.Insert(item, _priority(item));
        }
    }
}
