using System;
using System.Collections.Generic;
using System.Xml.Schema;
using TKernel;

namespace TKBRep
{
    public class TopTools_ListOfShape : NCollection_List<TopoDS_Shape>
    {
        public TopTools_ListOfShape()
        {
            allocator = new NCollection_BaseAllocator();
        }
        public TopTools_ListOfShape(NCollection_BaseAllocator nCollection_BaseAllocator)
        {
            allocator = nCollection_BaseAllocator;
        }
        NCollection_BaseAllocator allocator;
        public NCollection_BaseAllocator Allocator()
        {
            return allocator;
        }
    }
}