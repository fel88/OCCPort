using System;
using TKBRep;

namespace OCCPort
{
    internal class TopTools_ListIteratorOfListOfShape
    {
        TopTools_ListOfShape list;
        internal void Initialize(TopTools_ListOfShape l)
        {
            list = l;
        }

        int index = 0;
        internal bool More()
        {
            return index < list.Count;
        }

        internal void Next()
        {
            index++;
        }

        internal TopoDS_Shape Value()
        {
            return list[index];
        }
    }
}