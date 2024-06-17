using OCCPort;
using System;

namespace OCCPort
{
	internal struct gp_Ax2
	{


		public gp_Ax2(gp_Pnt P, gp_Dir N, gp_Dir Vx)
		{
			axis = new gp_Ax1(P, N);
			vydir = (N);
			vxdir = (N);
			vxdir.CrossCross(Vx, N);
			vydir.Cross(vxdir);
		}



		//! Returns the main direction of <me>.
		public gp_Dir Direction() { return axis.Direction(); }

		//! Returns the "Location" point (origin) of <me>.
		public gp_Pnt Location() { return axis.Location(); }

		//! Returns the "XDirection" of <me>.


		gp_Ax1 axis;
		gp_Dir vydir;
		gp_Dir vxdir;

		internal gp_Dir XDirection()
		{
			return vxdir;
		}

		internal gp_Dir YDirection()
		{
			return vydir;
		}
	}
}