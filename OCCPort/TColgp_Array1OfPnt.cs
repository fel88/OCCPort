using System.Collections.Generic;

namespace OCCPort
{
	public class TColgp_Array1OfPnt : List<gp_Pnt>
	{
		//public List<gp_Pnt> list = new List<gp_Pnt>();
		public int Length()
		{
			return Count;
		}
	}
}