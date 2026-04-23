using System;
using System.Collections.Generic;
using System.Linq;

namespace OCCPort
{
    public interface IMeshData
    {
        //! Default size for memory block allocated by IncAllocator. 
        /**
        * The idea here is that blocks of the given size are returned to the system
        * rather than retained in the malloc heap, at least on WIN32 and WIN64 platforms.
        */
        //# ifdef _WIN64
        public const int MEMORY_BLOCK_SIZE_HUGE = 1024 * 1024;
        //#else
        //      public const size_t MEMORY_BLOCK_SIZE_HUGE = 512 * 1024;
        //#endif
        public class DMapOfShapeInteger
        {
            public List<DMapOfShapeIntegerItem> Items = new List<DMapOfShapeIntegerItem>();

            internal void Bind(TopoDS_Edge theEdge, int v)
            {
                Items.Add(new DMapOfShapeIntegerItem() { Int = v, Shape = theEdge });
            }

            internal int Find(TopoDS_Edge aEdge)
            {
                throw new NotImplementedException();
            }

            internal bool IsBound(TopoDS_Edge theEdge)
            {
                return Items.Any(z => z.Shape == theEdge);
            }

            public class DMapOfShapeIntegerItem
            {
                public TopoDS_Shape Shape;
                public int Int;

            }
        }
    }

}