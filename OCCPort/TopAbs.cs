using System;

namespace OCCPort
{
	internal class TopAbs
	{

		public static TopAbs_Orientation Reverse(TopAbs_Orientation Ori)
		{
			TopAbs_Orientation[] TopAbs_Table_Reverse =
		   {
	TopAbs_Orientation.TopAbs_REVERSED, TopAbs_Orientation.TopAbs_FORWARD, TopAbs_Orientation.TopAbs_INTERNAL, TopAbs_Orientation.TopAbs_EXTERNAL
  };
			return TopAbs_Table_Reverse[(int)Ori];
		}

	}

}