using System;

namespace OCCPort.Tester
{
	internal class gp_Lin
	{
		internal gp_Dir Direction()
		{
			return pos.Direction();
		}
		gp_Ax1 pos;

		public gp_Lin(gp_Ax1 value)
		{
			pos = value;
		}
	}
}