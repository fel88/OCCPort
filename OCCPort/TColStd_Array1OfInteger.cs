using System;

namespace OCCPort
{
	public class TColStd_Array1OfInteger : NCollection_Array1<int>
	{
		public int Length()
		{
			return list.Count;
		}
	}
}