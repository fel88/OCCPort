using System.Collections.Generic;

namespace OCCPort
{
    public class NCollection_Sequence<T>:List<T>
    {

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public void Append(T t)
        {
            Add(t);
        }
    }
}