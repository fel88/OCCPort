using System;
using System.Collections.Generic;

namespace OCCPort
{
    public partial class IMeshData
	{
        public interface IEdgeHandle
        {
        }

        public class VectorOfIFaceHandles 
		{
			public int Size()
			{
				throw new NotImplementedException();
			}
		}

        internal class VectorOfIEdgeHandles
        {
            List<IEdgeHandle> items = new List<IEdgeHandle>();
            public int Size()
            {
                return items.Count; 
            }
        }
    }

}