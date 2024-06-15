using System;

namespace OCCPort
{
	internal class gp_Ax2
	{


		public gp_Ax2(gp_Pnt P, gp_Dir N, gp_Dir Vx)
		{
			axis = new gp_Ax1(P, N);
			vydir = (N);
			vxdir = (N);
			vxdir.CrossCross(Vx, N);
			vydir.Cross(vxdir);
		}


		internal gp_Dir Direction()
		{
			throw new NotImplementedException();
		}

		internal gp_Pnt Location()
		{
			throw new NotImplementedException();
		}

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