using System;

namespace OCCPort
{
	//! Representation of a curve by a 3D curve.

	internal class BRep_Curve3D : BRep_GCurve
	{
		public BRep_Curve3D(Geom_Curve c, TopLoc_Location l)
		{
		}

		public void SetRange(double First, double Last)
		{
			myFirst = First;
			myLast = Last;
			Update();
		}

	}
}