using System;
using TKBRep;

namespace OCCPort
{
    public class TopTools_ListIteratorOfListOfShape
    {
        TopTools_ListOfShape list;
        public void Initialize(TopTools_ListOfShape l)
        {
            list = l;
        }

        int index = 0;
        public bool More()
        {
            return index < list.Count;
        }

        public  void Next()
        {
            index++;
        }

        public TopoDS_Shape Value()
        {
            return list[index];
        }
    }
}