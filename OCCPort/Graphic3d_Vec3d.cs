namespace OCCPort
{
	internal class Graphic3d_Vec3d : NCollection_Vec3
	{


		public Graphic3d_Vec3d(double v1, double v2, double v3) : base(v1, v2, v3)
		{

		}

		internal void SetValues(double v1, double v2, double v3)
		{
			v[0] = v1;
			v[1] = v2;
			v[2] = v3;
		}
	}
}
