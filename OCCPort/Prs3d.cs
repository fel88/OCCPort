using OCCPort;
using System;

namespace OCCPort
{
	internal class Prs3d
	{
		internal static void AddFreeEdges(TColgp_SequenceOfPnt theSegments, Poly_Triangulation aPolyTri, TopLoc_Location aLocation)
		{
			throw new NotImplementedException();
		}
		//! Computes the absolute deflection value based on relative deflection Prs3d_Drawer::DeviationCoefficient().
		//! @param theBndBox [in] bounding box
		//! @param theDeviationCoefficient [in] relative deflection coefficient from Prs3d_Drawer::DeviationCoefficient()
		//! @param theMaximalChordialDeviation [in] absolute deflection coefficient from Prs3d_Drawer::MaximalChordialDeviation()
		//! @return absolute deflection coefficient based on bounding box dimensions or theMaximalChordialDeviation if bounding box is Void or Infinite
		public static double GetDeflection(Bnd_Box theBndBox,
									  double theDeviationCoefficient,
									  double theMaximalChordialDeviation)
		{
			if (theBndBox.IsVoid())
			{
				return theMaximalChordialDeviation;
			}

			Bnd_Box aBndBox = theBndBox;
			if (theBndBox.IsOpen())
			{
				if (!theBndBox.HasFinitePart())
				{
					return theMaximalChordialDeviation;
				}
				aBndBox = theBndBox.FinitePart();
			}

			Graphic3d_Vec3d aVecMin = new Graphic3d_Vec3d(), aVecMax = new Graphic3d_Vec3d();
			double x1, x2, y1, y2, z1, z2;
			aBndBox.Get(out x1, out y1, out z1, out x2, out y2, out z2);
			aVecMin.SetValues(x1, y1, z1);
			aVecMax.SetValues(x2, y2, z2);

			return GetDeflection(aVecMin, aVecMax, theDeviationCoefficient);
		}
		//! Computes the absolute deflection value based on relative deflection Prs3d_Drawer::DeviationCoefficient().
		//! @param theBndMin [in] bounding box min corner
		//! @param theBndMax [in] bounding box max corner
		//! @param theDeviationCoefficient [in] relative deflection coefficient from Prs3d_Drawer::DeviationCoefficient()
		//! @return absolute deflection coefficient based on bounding box dimensions
		public static double GetDeflection(Graphic3d_Vec3d theBndMin,
									  Graphic3d_Vec3d theBndMax,
									  double theDeviationCoefficient)
		{

			var aDiag = theBndMax - theBndMin;
			return Math.Max(aDiag.maxComp() * theDeviationCoefficient * 4.0, Precision.Confusion());
		}
	}
}