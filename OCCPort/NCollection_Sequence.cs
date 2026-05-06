using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class NCollection_Sequence<T> : List<T>
    {

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public void Append(T t)
        {
            Add(t);
        }
        public new T this[int key]
        {
            get => base[key - Lower()];
            set => base[key - Lower()] = value;
        }
        //! Method for consistency with other collections.
        //! @return Lower bound (inclusive) for iteration.
        public int Lower()
        {
            return 1;
        }

        //! Method for consisten
        internal T First()
        {
            return this[Lower()];
        }

        internal void Remove(int v)
        {
            RemoveAt(v - 1);
        }
    }
}