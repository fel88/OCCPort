using System.Collections.Generic;
using System.Linq;

namespace OCCPort
{
    public partial class IMeshData
	{
        internal class DMapOfShapeInteger
        {
			public List<DMapOfShapeIntegerItem> Items = new List<DMapOfShapeIntegerItem>();

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