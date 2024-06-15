using System;

namespace OCCPort
{
    public class NCollection_Vec2
	{
		//! Per-component constructor.
		public NCollection_Vec2(double theX,

							  double theY)
		{

			v[0] = theX;
			v[1] = theY;
		}

		public NCollection_Vec2()
		{ 

		}
		//! Alias to 1st component as X coordinate in XY.
		public double x() { return v[0]; }

		//! Alias to 2nd component as Y coordinate in XY.
		public double y() { return v[1]; }

		double[] v = new double[2];
	}
}