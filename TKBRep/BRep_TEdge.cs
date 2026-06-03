using OCCPort.Common;
using System;

namespace OCCPort
{
    internal class BRep_TEdge : TopoDS_TEdge
    {//=======================================================================
        public bool Degenerated()
        {
            return (myFlags & DegeneratedMask) != 0;
        }
        //=======================================================================
        public bool SameParameter()
        {
            return (myFlags & ParameterMask) != 0;
        }
        public void Degenerated(bool S)
        {
            if (S) myFlags |= DegeneratedMask;
            else myFlags &= ~DegeneratedMask;
        }
        public void Tolerance(double T)
        {
            myTolerance = T;
        }

        public bool SameRange()
        {
            return (myFlags & RangeMask) != 0;
        }

        public BRep_ListOfCurveRepresentation Curves()
        {
            return myCurves;
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

        public void SameParameter(bool S)
        {
            if (S) myFlags |= ParameterMask;
            else myFlags &= ~ParameterMask;
        }



        public void SameRange(bool S)
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

        public override TopoDS_TShape EmptyCopy()
        {

            BRep_TEdge TE =
              new BRep_TEdge();
            TE.Tolerance(myTolerance);
            // copy the curves representations
            BRep_ListOfCurveRepresentation l = TE.ChangeCurves();

            foreach (var item in myCurves)
            {
                // on ne recopie PAS les polygones
                if (item is BRep_GCurve ||
                     item is BRep_CurveOn2Surfaces)
                {
                    l.Append(item.Copy());
                }

            }

            TE.Degenerated(Degenerated());
            TE.SameParameter(SameParameter());
            TE.SameRange(SameRange());

            return TE;

        }
    }
}