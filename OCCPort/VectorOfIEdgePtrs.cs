using OCCPort.Interfaces;
using System.Collections.Generic;

namespace OCCPort
{
    internal class VectorOfIEdgePtrs : List<IMeshData_Edge>
    {
        public VectorOfIEdgePtrs(int capacity, NCollection_IncAllocator theAllocator) : base(capacity)
        {
        }
    }
}