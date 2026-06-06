using OCCPort;
using System;
using System.Collections.Generic;
using System.Xml.Schema;
using TKernel;

namespace TKBRep
{
    public class TopTools_ListOfShape : List<TopoDS_Shape>
    {
        public NCollection_BaseAllocator Allocator()
        {
            return myAllocator;
        }
        NCollection_BaseAllocator myAllocator = new NCollection_BaseAllocator();

        public TopTools_ListOfShape() { }
        public TopTools_ListOfShape(NCollection_BaseAllocator nCollection_BaseAllocator)
        {
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }


        //typedef NCollection_List<TopoDS_Shape> TopTools_ListOfShape;

        //! Append another list at the end.
        //! After this operation, theOther list will be cleared.
        public void Append(TopTools_ListOfShape anc)
        {
            AddRange(anc);
            anc.Clear();
        }

        public void Append(TopoDS_Shape anc)
        {
            Add(anc);
        }

        public int Extent()
        {
            return Count; 
        }

        public void RemoveFirst()
        {
            RemoveAt(0);
        }

        public TopoDS_Shape First()
        {
            return this[0];
        }
    }
}