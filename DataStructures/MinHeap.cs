using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class MinHeap<T>
    {
        int _heapSize;
        List<T> _keys;
        List<double> _values;
        Dictionary<T, int> _idMap;

        public MinHeap()
        {
            _keys = new List<T>();
            _values = new List<double>();
            _idMap = new Dictionary<T, int>();
        }

        public MinHeap(IEnumerable<KeyValuePair<T, double>> items) : this()
        {
            foreach (KeyValuePair<T, double> pair in items)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
                _idMap[pair.Key] = _keys.Count - 1;
            }

            _heapSize = _keys.Count;

            for (int i = _keys.Count / 2; i > 0; i--)
            {
                _minHeapify(i - 1);
            }
        }

        public void SetValue(T item, double value)
        {
            if (!_idMap.ContainsKey(item))
            {
                Insert(item, value);
            }
            else
            {
                int id = _idMap[item];

                DecreaseKey(item, value); // check if needs to be higher in the heap
                _minHeapify(id); // check if item needs to be lower in the heap
            }
        }

        public T Minimum() => _keys[0];

        public T ExtractMin()
        {
            if (_heapSize < 1)
            {
                throw new Exception();
            }

            T min = _keys[0];

            _exchange(0, _heapSize - 1);

            _heapSize -= 1;
            _idMap.Remove(min);

            _keys.RemoveAt(_heapSize);
            _values.RemoveAt(_heapSize);

            _minHeapify(0);
            return min;
        }

        public T ExtractMin(out double value)
        {
            if (_heapSize < 1)
            {
                throw new Exception();
            }

            T min = _keys[0];
            value = _values[0];

            _exchange(0, _heapSize - 1);

            _heapSize -= 1;
            _idMap.Remove(min);

            _minHeapify(0);

            return min;
        }

        public void DecreaseKey(T item, double value)
        {
            int id = _idMap[item];

            //if (value > _values[id])
            //{
            //    throw new Exception();
            //}

            _values[id] = value;

            while (id > 0 && _values[_parentID(id)] > _values[id])
            {
                _exchange(id, _parentID(id));
                id = _parentID(id);
            }
        }

        public bool Contains(T item) => _idMap.ContainsKey(item);

        public void Insert(T item, double value)
        {
            _heapSize += 1;
            if (_heapSize > _keys.Count)
            {
                _keys.Add(item);
                _values.Add(double.MaxValue);
            }
            else
            {
                _keys[_heapSize - 1] = item;
                _values[_heapSize - 1] = double.MaxValue;
            }

            _idMap[item] = _heapSize - 1;

            DecreaseKey(item, value);
        }

        public int HeapSize => _heapSize;

        public IEnumerable<T> Heap => _keys.Take(_heapSize);

        public int ID(T item) => _idMap[item];

        public double Value(T item) => _values[_idMap[item]];

        public double MinValue => _values[0];

        void _minHeapify(int ID)
        {
            int leftID = _leftID(ID);
            int rightID = _rightID(ID);
            int smallestID;

            if (leftID < _heapSize && _values[leftID] < _values[ID])
            {
                smallestID = leftID;
            }
            else
            {
                smallestID = ID;
            }

            if (rightID < _heapSize && _values[rightID] < _values[smallestID])
            {
                smallestID = rightID;
            }

            if (smallestID != ID)
            {
                _exchange(ID, smallestID);
                _minHeapify(smallestID);
            }
        }

        int _parentID(int ID) => (ID + 1) / 2 - 1;

        int _leftID(int ID) => 2 * (ID + 1) - 1;

        int _rightID(int ID) => 2 * (ID + 1);

        void _exchange(int ID1, int ID2)
        {
            T tempKey = _keys[ID1];
            double tempValue = _values[ID1];

            _keys[ID1] = _keys[ID2];
            _values[ID1] = _values[ID2];

            _keys[ID2] = tempKey;
            _values[ID2] = tempValue;

            _idMap[_keys[ID1]] = ID1;
            _idMap[_keys[ID2]] = ID2;            
        }
    }
}
