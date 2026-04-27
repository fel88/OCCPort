using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class VectorOfPnt : NCollection_Vector<gp_Pnt>
    {
        public VectorOfPnt(int capacity) :base()
        {
        }

        internal int Size()
        {
            return Count;
        }
        
    }
}