using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class TopoDS_ListIteratorOfListOfShape
    {
        protected List<TopoDS_Shape> list = new List<TopoDS_Shape>();
        public TopoDS_Shape this[int i]
        {
            get { return list[i]; }
            set { list[i] = value; }
        }
        internal bool More()
        {
            throw new NotImplementedException();
        }

        internal void Next()
        {
            throw new NotImplementedException();
        }

        internal TopoDS_Shape Value()
        {
            throw new NotImplementedException();
        }
    }
}