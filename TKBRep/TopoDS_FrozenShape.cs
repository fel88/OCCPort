using System;

namespace OCCPort
{
    public class TopoDS_FrozenShape : Exception
    {
        public TopoDS_FrozenShape(string str) : base(str) { }
    }
}