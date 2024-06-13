namespace OCCPort
{
	public static class gp
	{

		public static double Resolution()
		{
			//2.2250738585072014e-308
			return double.Epsilon;
		}


		internal static gp_Dir DX()
		{
			gp_Dir gp_DX = new gp_Dir(1, 0, 0);
			return gp_DX;

		}

		internal static gp_Dir DY()
		{
			gp_Dir gp_DY = new gp_Dir(0, 1, 0);
			return gp_DY;

		}

		internal static gp_Dir DZ()
		{
			gp_Dir gp_DZ = new gp_Dir(0, 0, 1);
			return gp_DZ;

		}

		internal static gp_Pnt Origin()
		{
			gp_Pnt gp_Origin = new gp_Pnt(0, 0, 0);
			return gp_Origin;

		}
	}
}
