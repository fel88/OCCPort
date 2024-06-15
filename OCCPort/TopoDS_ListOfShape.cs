using System;
using System.Collections.Generic;
using System.Linq;

namespace OCCPort
{
    internal class TopoDS_ListOfShape
    {
        List<TopoDS_Shape> list = new List<TopoDS_Shape>();
        internal void Append(TopoDS_Shape aComponent)
        {
            list.Add(aComponent);
        }

        internal TopoDS_Shape Last()
        {
            return list.Last();
        }
    }
}