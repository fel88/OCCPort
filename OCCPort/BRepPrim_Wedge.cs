using System;
using System.Security.Cryptography;

namespace OCCPort.Tester
{
	internal class BRepPrim_Wedge : BRepPrim_GWedge

	{
		public BRepPrim_Wedge(gp_Ax2 Axes, double dx, double dy, double dz) 
			: base(new BRepPrim_Builder(), Axes, dx, dy, dz)
		{
		

		}


		
	}
}