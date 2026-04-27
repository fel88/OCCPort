using System;

namespace OCCPort
{
    public class TopoDS_Edge : TopoDS_Shape
    {
        public TopoDS_Edge() : base() { }
        public TopoDS_Edge(TopoDS_Shape theOther) : base(theOther)
        {
        }
    }
}