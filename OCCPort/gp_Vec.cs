using System;

namespace OCCPort
{
	//! Defines a non-persistent vector in 3D space.

	public struct gp_Vec
	{
		gp_XYZ coord;
		//! Multiplies a vector by a scalar
		public gp_Vec Multiplied(double theScalar)
		{
			gp_Vec aV = this;
			aV.coord.Multiply(theScalar);
			return aV;
		}

		//! computes the scalar product
		public double Dot(gp_Vec theOther) { return coord.Dot(theOther.coord); }

		//! computes the scalar product
		public double Dot(gp_XYZ theOther) { return coord.Dot(theOther); }



		public gp_XYZ XYZ()
		{
			return coord;
		}

		public gp_Vec Added(gp_Vec theOther)
		{
			gp_Vec aV = this;
			aV.coord.Add(theOther.coord);
			return aV;
		}

		public static gp_Vec operator *(gp_Vec v, double theScalar)
		{
			return v.Multiplied(theScalar);
		}
		public static gp_Vec operator *(double theScalar, gp_Vec v)
		{
			return v.Multiplied(theScalar);
		}

		public static gp_Vec operator +(gp_Vec v, gp_Vec theOther)
		{
			return v.Added(theOther);
		}


		//! Creates a vector with a triplet of coordinates.
		public gp_Vec(gp_XYZ theCoord)
		{
			coord = (theCoord);
		}


		//! Computes the magnitude of this vector.
		public double Magnitude() { return coord.Modulus(); }

		//! normalizes a vector
		//! Raises an exception if the magnitude of the vector is
		//! lower or equal to Resolution from gp.
		public void Normalize()
		{
			double aD = coord.Modulus();

			if (aD <= gp.Resolution())
				throw new Exception("gp_Vec::Normalize() - vector has zero norm");

			coord.Divide(aD);
		}

		public gp_Vec(gp_Dir theV)
		{
			coord = theV.XYZ();
		}

	}
}