using System;

namespace OCCPort
{

	//! The TVertex from  BRep inherits  from  the TVertex
	//! from TopoDS. It contains the geometric data.
	//!
	//! The  TVertex contains a 3d point, location and a tolerance.

	internal class BRep_TVertex : TopoDS_TVertex
	{
		public gp_Pnt Pnt( )
		{
			return myPnt;

		}

		public void Pnt(gp_Pnt P)
		{
			myPnt = P;
		}


		gp_Pnt myPnt;
		double myTolerance;
		BRep_ListOfPointRepresentation myPoints;

		internal void UpdateTolerance(double T)
		{
			if (T > myTolerance) myTolerance = T;
		}
	}
}