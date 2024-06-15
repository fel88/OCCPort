using System;

namespace OCCPort
{
    public class TopoDS_UnCompatibleShapes : Exception
    {
        public TopoDS_UnCompatibleShapes(string str) : base(str) { }
    }
}