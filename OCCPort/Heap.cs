using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class Heap<T>
    {
        List<T> a = new List<T>();
        IComparer<T> comparer;

        public T[] Array()
        {
            return a.ToArray();
        }

        public Heap(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }
                
        public Heap(Comparison<T> comparison)
        {
            this.comparer = Comparer<T>.Create(comparison);
        }

        public Heap()
        {
            this.comparer = Comparer<T>.Default;
        }

        public void Add(T value)
        {
            a.Add(value);
            HeapifyUp(a.Count - 1);
        }

        public T PeekTop()
        {
            return a[0];
        }

        public T PopTop()
        {
            var result = PeekTop();
            Swap(0, a.Count - 1);
            a.RemoveAt(a.Count - 1);
            HeapifyDown(0);
            return result;
        }

        public void Remove(T value)
        {
            if (a[a.Count - 1].Equals(value))
            {
                a.RemoveAt(a.Count - 1);
                return;
            }

            var index = a.IndexOf(value);
            Swap(index, a.Count - 1);
            a.RemoveAt(a.Count - 1);

            int parentIndex = (index - 1) / 2;
            if (comparer.Compare(a[index], a[parentIndex]) > 0)
            {
                HeapifyUp(index);
            }
            else
            {
                HeapifyDown(index);
            }
        }

        public int Count => a.Count;

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (comparer.Compare(a[index], a[parentIndex]) > 0)
                {
                    Swap(index, parentIndex);
                    index = parentIndex;
                }
                else
                {
                    break;
                }
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                var leftChildIndex = index * 2 + 1;
                if (leftChildIndex >= a.Count)
                {
                    return;
                }

                var bestChildIndex = leftChildIndex;

                var rightChildIndex = index * 2 + 2;
                if (rightChildIndex < a.Count)
                {
                    if (comparer.Compare(a[rightChildIndex], a[leftChildIndex]) > 0)
                    {
                        bestChildIndex = rightChildIndex;
                    }
                }

                if (comparer.Compare(a[bestChildIndex], a[index]) > 0)
                {
                    Swap(bestChildIndex, index);
                    index = bestChildIndex;
                }
                else
                {
                    break;
                }
            }
        }

        private void Swap(int i, int j)
        {
            var t = a[i];
            a[i] = a[j];
            a[j] = t;
        }
    }

}