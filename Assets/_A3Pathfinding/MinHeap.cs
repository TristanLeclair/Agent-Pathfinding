using System;

namespace _A3Pathfinding
{
    public class MinHeap<T> where T : IHeapItem<T>
    {
        private readonly T[] _items;
        private int _currentItemCount;

        public MinHeap(int maxHeapSize)
        {
            _items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = _currentItemCount;
            _items[_currentItemCount] = item;
            SortUp(item);
            _currentItemCount++;
        }

        public T RemoveFirst()
        {
            T firstItem = _items[0];
            _currentItemCount--;
            _items[0] = _items[_currentItemCount];
            _items[0].HeapIndex = 0;
            SortDown(_items[0]);
            return firstItem;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                var parent = _items[parentIndex];
                if (item.CompareTo(parent) > 0)
                {
                    Swap(item, parent);
                }
                else break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        public int Count => _currentItemCount;

        public bool Contains(T item)
        {
            return Equals(_items[item.HeapIndex], item);
        }

        private void SortDown(T item)
        {
            while (true)
            {
                var childIndexLeft = item.HeapIndex * 2 + 1;
                var childIndexRight = item.HeapIndex * 2 + 2;

                if (childIndexLeft < _currentItemCount)
                {
                    var swapIndex = childIndexLeft;

                    if (childIndexRight < _currentItemCount)
                    {
                        if (_items[childIndexLeft]
                                .CompareTo(_items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(_items[swapIndex]) < 0)
                    {
                        Swap(item, _items[swapIndex]);
                    }
                    else return;
                }
                else return;
            }
        }

        private void Swap(T item, T heapItem)
        {
            _items[item.HeapIndex] = heapItem;
            _items[heapItem.HeapIndex] = item;
            (item.HeapIndex, heapItem.HeapIndex) =
                (heapItem.HeapIndex, item.HeapIndex);
        }
    }

    public interface IHeapItem<in T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}