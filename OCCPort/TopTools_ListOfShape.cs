using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace OCCPort.Tester
{
    internal class TopTools_ListOfShape : List<TopoDS_Shape>
    {
        //typedef NCollection_List<TopoDS_Shape> TopTools_ListOfShape;

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