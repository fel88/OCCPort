using System;

namespace OCCPort
{
	internal class BRep_TEdge : TopoDS_TEdge
	{//=======================================================================
		public bool Degenerated()
		{
			return (myFlags & DegeneratedMask) != 0;
		}
		public BRep_TEdge()


		{
			myCurves = new BRep_ListOfCurveRepresentation();
			myTolerance = Standard_Real.RealEpsilon();
			myFlags = (0);
			SameParameter(true);
			SameRange(true);
		}
		//=======================================================================
		//function : Tolerance
		//purpose  : 
		//=======================================================================

		public double Tolerance()
		{
			return myTolerance;
		}

		const int ParameterMask = 1;
		const int RangeMask = 2;
		const int DegeneratedMask = 4;

		void SameParameter(bool S)
		{
			if (S) myFlags |= ParameterMask;
			else myFlags &= ~ParameterMask;
		}



		void SameRange(bool S)
		{
			if (S) myFlags |= RangeMask;
			else myFlags &= ~RangeMask;
		}

		public BRep_ListOfCurveRepresentation ChangeCurves()
		{
			return myCurves;
		}


		double myTolerance;
		int myFlags;
		BRep_ListOfCurveRepresentation myCurves;

		internal void UpdateTolerance(double T)
		{
			if (T > myTolerance) myTolerance = T;
		}
	}
}