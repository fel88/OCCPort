using System.Collections.Generic;

namespace OCCPort
{
	internal class TColStd_HArray1OfReal
	{
		public List<double> list = new List<double>();
		public int Length()
		{
			return list.Count;
		}
	}
}