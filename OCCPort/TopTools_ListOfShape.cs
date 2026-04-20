using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace OCCPort.Tester
{
    internal class TopTools_ListOfShape : List<TopoDS_Shape>
    {
        internal NCollection_BaseAllocator Allocator()
        {
            return myAllocator;
        }
        NCollection_BaseAllocator myAllocator = new NCollection_BaseAllocator();

        public TopTools_ListOfShape() { }
        public TopTools_ListOfShape(NCollection_BaseAllocator nCollection_BaseAllocator)
        {
        }


        //typedef NCollection_List<TopoDS_Shape> TopTools_ListOfShape;

        //! Append another list at the end.
        //! After this operation, theOther list will be cleared.
        internal void Append(TopTools_ListOfShape anc)
        {
            AddRange(anc);
            anc.Clear();
        }
        internal void Append(TopoDS_Shape anc)
        {
            Add(anc);
        }

        internal int Extent()
        {
            return Count; 
        }
    }
}