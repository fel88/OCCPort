namespace OCCPort
{
	public class NCollection_Vec4
	{
		public NCollection_Vec4()
		{
			v = new double[4];
		}
		//! Per-component constructor.
		public NCollection_Vec4(double theX,
							double theY,
							double theZ,
							double theW)
		{
			v = new double[4];
			v[0] = theX;
			v[1] = theY;
			v[2] = theZ;
			v[3] = theW;
		}


		protected double[] v; //!< define the vector as array to avoid structure alignment issues
							  //! Alias to 1st component as X coordinate in XYZW.
		public double x() { return v[0]; }

		//! Alias to 1st component as RED channel in RGBA.
		public double r() { return v[0]; }

		//! Alias to 2nd component as Y coordinate in XYZW.
		public double y() { return v[1]; }

		//! Alias to 2nd component as GREEN channel in RGBA.
		public double g() { return v[1]; }

		//! Alias to 3rd component as Z coordinate in XYZW.
		public double z() { return v[2]; }

		//! Alias to 3rd component as BLUE channel in RGBA.
		public double b() { return v[2]; }

		//! Alias to 4th component as W coordinate in XYZW.
		public double w() { return v[3]; }

		//! Alias to 4th component as ALPHA channel in RGBA.
		public double a() { return v[3]; }


	}
}
