using System;
using System.Collections.Generic;

namespace OCCPort.Tester
{
	public class TColgp_SequenceOfPnt
	{
		internal int Size()
		{
			throw new NotImplementedException();
		}

		List<gp_Pnt> list = new List<gp_Pnt>();
		public gp_Pnt Value(int index)
		{
			return list[index];
		}

	}
}