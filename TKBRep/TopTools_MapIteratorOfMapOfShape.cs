using OpenTK.Graphics.ES11;
using System;

namespace OCCPort
{
    internal class TopTools_MapIteratorOfMapOfShape
    {
        public TopTools_MapIteratorOfMapOfShape(TopTools_MapOfShape vmap)
        {
            list = vmap;
        }

        TopTools_MapOfShape list = null;
        int index = 0;
        internal void Initialize(TopTools_MapOfShape vmap)
        {
            list = vmap;
            index = 0;
        }

        internal TopoDS_Shape Key()
        {
            return list[index];
        }

        internal bool More()
        {
            return index < list.Count;
        }

        internal void Next()
        {
            index++;
        }
    }
}