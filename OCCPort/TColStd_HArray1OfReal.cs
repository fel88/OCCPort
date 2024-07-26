using System.Collections.Generic;

namespace OCCPort
{
	internal class TColStd_HArray1OfReal: NCollection_Array1
	{
		
		public int Length()
		{
			return list.Count;
		}
		
		
	}
}