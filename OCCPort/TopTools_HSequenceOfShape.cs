using System.Collections.Generic;

namespace OCCPort
{
    internal class TopTools_HSequenceOfShape : List<TopoDS_Shape>
    {
        public void Prepend(TopoDS_Shape shape)
        {
            Insert(0, shape);
        }
        public void Append(TopoDS_Shape shape)
        {
            Add(shape);
        }
        public TopoDS_Shape Value(int i)
        {
            return this[i - 1];
        }
        public int Length()
        {
            return Count;
        }
    }
}