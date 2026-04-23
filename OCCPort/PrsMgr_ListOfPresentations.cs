using System.Collections.Generic;

namespace OCCPort
{
	public class PrsMgr_ListOfPresentations:List<Graphic3d_Structure>
	{
		public bool IsEmpty()
		{
			return Count == 0;
		}
	}
}