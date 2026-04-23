using System.Collections.Generic;

namespace OCCPort
{
    public class NCollection_Vector<T>:List<T>
    {
        public void Append(T t)
        {
            Add(t);
        }
    }
}