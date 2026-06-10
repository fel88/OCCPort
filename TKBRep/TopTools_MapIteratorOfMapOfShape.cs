using System;
using TKBRep;

namespace OCCPort
{
    public class TopTools_MapIteratorOfMapOfShape
    {
        public TopTools_MapIteratorOfMapOfShape(TopTools_MapOfShape vmap)
        {
            list = vmap;
        }

        TopTools_MapOfShape list = null;
        int index = 0;
        public void Initialize(TopTools_MapOfShape vmap)
        {
            list = vmap;
            index = 0;
        }

        public TopoDS_Shape Key()
        {
            return list[index];
        }

        public bool More()
        {
            return index < list.Count;
        }

        public void Next()
        {
            index++;
        }
    }
}