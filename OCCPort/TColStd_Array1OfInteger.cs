using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class TColStd_Array1OfInteger
    {
        List<int> list = new List<int>();
        public int Length()
        {
            return list.Count;
        }
        internal int Value(int theRank)
        {
            throw new NotImplementedException();
        }
    }
}