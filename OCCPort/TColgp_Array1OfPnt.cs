using System.Collections.Generic;

namespace OCCPort
{
	internal class TColgp_Array1OfPnt
	{
		public List<gp_Pnt> list = new List<gp_Pnt>();
		public int Length()
		{
			return list.Count;
		}
	}
}