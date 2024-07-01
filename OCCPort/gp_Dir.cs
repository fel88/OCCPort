using System;

namespace OCCPort
{
	public struct gp_Dir
	{
		private gp_XYZ coord;
		//! Creates a direction corresponding to X axis.

		internal double X()
		{
			return coord.X();
		}
		internal double Y()
		{
			return coord.Y();
		}
		internal double Z()
		{
			return coord.Z();
		}

		public static gp_Dir operator ^(gp_Dir v, gp_Dir theRight)
		{
			return v.Crossed(theRight);
		}

		public void SetCoord(double theXv,
							 double theYv,
							  double theZv)
		{
			double aD = Math.Sqrt(theXv * theXv + theYv * theYv + theZv * theZv);
			if (aD <= gp.Resolution())
				throw new Exception("gp_Dir::SetCoord() - input vector has zero norm");

			coord.SetX(theXv / aD);
			coord.SetY(theYv / aD);
			coord.SetZ(theZv / aD);
		}


		//! Computes the scalar product
		double Dot(gp_Dir theOther) { return coord.Dot(theOther.coord); }

		public static double operator *(gp_Dir v, gp_Dir theOther)
		{
			return v.Dot(theOther);
		}


		public static gp_Dir operator -(gp_Dir temp)
		{
			return temp.Reversed();
		}

		//! Normalizes the vector theV and creates a direction. Raises ConstructionError if theV.Magnitude() <= Resolution.
		public gp_Dir(gp_Vec theV)
		{
			gp_XYZ aXYZ = theV.XYZ();
			double aX = aXYZ.X();
			double aY = aXYZ.Y();
			double aZ = aXYZ.Z();
			double aD = Math.Sqrt(aX * aX + aY * aY + aZ * aZ);
			//Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir() - input vector has zero norm");
			coord = new gp_XYZ();
			coord.SetX(aX / aD);
			coord.SetY(aY / aD);
			coord.SetZ(aZ / aD);

		}
		//! Normalizes the vector theV and creates a direction. Raises ConstructionError if theV.Magnitude() <= Resolution.
		public gp_Dir(gp_Dir theV)
		{
			coord = new gp_XYZ(theV.coord);
		}
		//! Creates a direction from a triplet of coordinates. Raises ConstructionError if theCoord.Modulus() <= Resolution from gp.
		public gp_Dir(gp_XYZ theXYZ)
		{
			double aX = theXYZ.X();
			double aY = theXYZ.Y();
			double aZ = theXYZ.Z();
			double aD = Math.Sqrt(aX * aX + aY * aY + aZ * aZ);
			//Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir() - input vector has zero norm");
			coord = new gp_XYZ();
			coord.SetX(aX / aD);
			coord.SetY(aY / aD);
			coord.SetZ(aZ / aD);

		}

		//! Creates a direction with its 3 cartesian coordinates. Raises ConstructionError if Sqrt(theXv*theXv + theYv*theYv + theZv*theZv) <= Resolution
		//! Modification of the direction's coordinates
		//! If Sqrt (theXv*theXv + theYv*theYv + theZv*theZv) <= Resolution from gp where
		//! theXv, theYv ,theZv are the new coordinates it is not possible to
		//! construct the direction and the method raises the
		//! exception ConstructionError.
		public gp_Dir(double theXv, double theYv, double theZv)
		{
			double aD = Math.Sqrt(theXv * theXv + theYv * theYv + theZv * theZv);
			//Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir() - input vector has zero norm");
			coord = new gp_XYZ();
			coord.SetX(theXv / aD);
			coord.SetY(theYv / aD);
			coord.SetZ(theZv / aD);

		}

		//! for this unit vector, returns  its three coordinates as a number triplea.
		public gp_XYZ XYZ() { return coord; }
		//! Computes the angular value in radians between <me> and
		//! <theOther>. This value is always positive in 3D space.
		//! Returns the angle in the range [0, PI]
		double Angle(gp_Dir Other)
		{
			//    Commentaires :
			//    Au dessus de 45 degres l'arccos donne la meilleur precision pour le
			//    calcul de l'angle. Sinon il vaut mieux utiliser l'arcsin.
			//    Les erreurs commises sont loin d'etre negligeables lorsque l'on est
			//    proche de zero ou de 90 degres.
			//    En 3d les valeurs angulaires sont toujours positives et comprises entre
			//    0 et PI
			double Cosinus = coord.Dot(Other.coord);
			if (Cosinus > -0.70710678118655 && Cosinus < 0.70710678118655)
				return Math.Acos(Cosinus);
			else
			{
				double Sinus = (coord.Crossed(Other.coord)).Modulus();
				if (Cosinus < 0.0) return Math.PI - Math.Asin(Sinus);
				else return Math.Asin(Sinus);
			}
		}


		//! Returns True if the angle between the two directions is
		//! lower or equal to theAngularTolerance.
		public bool IsEqual(gp_Dir theOther, double theAngularTolerance)
		{
			return Angle(theOther) <= theAngularTolerance;
		}

		internal gp_Dir Crossed(gp_Dir theRight)
		{
			gp_Dir aV = this;
			aV.coord.Cross(theRight.coord);
			double aD = aV.coord.Modulus();
			//Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir::Crossed() - result vector has zero norm");

			aV.coord.Divide(aD);
			return aV;
		}
		void Reverse() { coord.Reverse(); }

		//! Reverses the orientation of a direction
		//! geometric transformations
		//! Performs the symmetrical transformation of a direction
		//! with respect to the direction V which is the center of
		//! the  symmetry.]
		public gp_Dir Reversed()
		{
			gp_Dir aV = this;
			aV.coord.Reverse();
			return aV;
		}

		internal void Transform(gp_Trsf T)
		{
			if (T.Form() == gp_TrsfForm.gp_Identity || T.Form() == gp_TrsfForm.gp_Translation) { }
			else if (T.Form() == gp_TrsfForm.gp_PntMirror) { coord.Reverse(); }
			else if (T.Form() == gp_TrsfForm.gp_Scale)
			{
				if (T.ScaleFactor() < 0.0) { coord.Reverse(); }
			}
			else
			{
				coord.Multiply(T.HVectorialPart());
				double D = coord.Modulus();
				coord.Divide(D);
				if (T.ScaleFactor() < 0.0) { coord.Reverse(); }
			}
		}

		public void CrossCross(gp_Dir theV1, gp_Dir theV2)
		{
			coord.CrossCross(theV1.coord, theV2.coord);
			var aD = coord.Modulus();
			//Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir::CrossCross() - result vector has zero norm");
			coord.Divide(aD);
		}

		public void Cross(gp_Dir theRight)
		{
			coord.Cross(theRight.coord);
			var aD = coord.Modulus();
			//Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir::Cross() - result vector has zero norm");
			coord.Divide(aD);
		}
	}
}