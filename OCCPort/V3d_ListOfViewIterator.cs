using System;

namespace OCCPort
{
    public class V3d_ListOfViewIterator
    {
        V3d_ListOfView list;
        public V3d_ListOfViewIterator(V3d_ListOfView myActiveViews)
        {
            list= myActiveViews;
        }

        internal bool More()
        {
            throw new NotImplementedException();
        }
    }
}