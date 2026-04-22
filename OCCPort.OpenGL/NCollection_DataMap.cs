using System.Collections.Generic;

namespace OCCPort.OpenGL
{
    internal class NCollection_DataMap<T1, T2>:Dictionary<T1,T2>
    {
        public bool IsEmpty()
        {
            return this.Count == 0;
        }
    }
}