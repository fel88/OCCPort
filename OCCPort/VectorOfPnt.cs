using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class VectorOfPnt : List<gp_Pnt>
    {
        internal int Size()
        {
            return Count;
        }
        public void Append(gp_Pnt f)
        {
            Add(f);
        }
    }
}