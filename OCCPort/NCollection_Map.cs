using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class NCollection_Map
    {
        internal bool Add(TopoDS_Shape e)
        {
            if (shapes.Contains(e))
            {
                return false;
            }
            shapes.Add(e);
            return true;
        }
        public bool IsEmpty()
        {
            return shapes.Count == 0;

        }
        List<TopoDS_Shape> shapes = new List<TopoDS_Shape>();
        internal void Remove(TopoDS_Shape e)
        {
            shapes.Remove(e);            
        }
    }
}