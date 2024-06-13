namespace OCCPort
{
	internal class Graphic3d_Vec4d : NCollection_Vec4
	{


		public Graphic3d_Vec4d(double v1, double v2, double v3, double v4)
		{
			this.v[0] = v1;
			this.v[1] = v2;
			this.v[2] = v3;
			this.v[3] = v4;
		}

		internal double w()
		{
			return v[3];
		}

		internal double x()
		{
			return v[0];

		}

		internal double y()
		{
			return v[1];

		}

		internal double z()
		{
			return v[2];

		}
	}
}
