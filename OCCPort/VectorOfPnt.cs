using System;
using System.Collections.Generic;
using TKMath;

namespace OCCPort
{
    public class VectorOfPnt : NCollection_Vector<gp_Pnt>
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