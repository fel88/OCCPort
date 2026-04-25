using OCCPort.Interfaces;
using System.Collections.Generic;

namespace OCCPort
{
    internal class VectorOfOrientation : List<TopAbs_Orientation>
    {
        public VectorOfOrientation(int capacity, NCollection_IncAllocator theAllocator) : base(capacity)
        {
        }
    }
}