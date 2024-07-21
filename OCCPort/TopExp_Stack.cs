using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class TopExp_Stack: TopoDS_Iterator
    {
      //  typedef TopoDS_Iterator* TopExp_Stack;
        public List<TopoDS_Iterator> list = new List<TopoDS_Iterator>();
        public TopoDS_Iterator this[int i]
        {
            get { return list[i]; }
            set { list[i] = value; }
        }

        internal void Add(TopoDS_Iterator topoDS_Iterator)
        {
            list.Add(topoDS_Iterator);
        }
    }
}