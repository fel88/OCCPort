using System;

namespace OCCPort
{
	public class TColStd_Array1OfInteger : NCollection_Array1
	{
		public int Length()
		{
			return list.Count;
		}
	}
}