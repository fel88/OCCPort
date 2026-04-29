using System;

namespace OCCPort
{
    internal class IDMapOfLink : NCollection_IndexedDataMap<BRepMesh_Edge, BRepMesh_PairOfIndex>
    {
        public IDMapOfLink(int v, NCollection_IncAllocator myAllocator)
        {
        }        
    }
}