using System.Collections.Generic;

namespace OCCPort.OpenGL
{
    internal class NCollection_DataMap<T1, T2> : Dictionary<T1, T2>
    {
        public bool IsBound(T1 key)
        {
            return ContainsKey(key);
        }

        public T2 Find(T1 key)
        {
            return this[key];
        }

        public bool IsEmpty()
        {
            return this.Count == 0;
        }
    }
}