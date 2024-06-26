﻿using System;

namespace OCCPort
{
    internal class BRep_TEdge : TopoDS_TEdge
    {//=======================================================================

        public BRep_TEdge()


        {
            myCurves = new BRep_ListOfCurveRepresentation();
            myTolerance = Standard_Real.RealEpsilon();
            myFlags = (0);
            SameParameter(true);
            SameRange(true);
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

        internal BRep_ListOfCurveRepresentation ChangeCurves()
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