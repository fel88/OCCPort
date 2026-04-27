using System;

namespace OCCPort
{

    //! The TVertex from  BRep inherits  from  the TVertex
    //! from TopoDS. It contains the geometric data.
    //!
    //! The  TVertex contains a 3d point, location and a tolerance.

    internal class BRep_TVertex : TopoDS_TVertex
    {
        public BRep_TVertex()
        {
            myTolerance = Standard_Real.RealEpsilon();

        }
        public gp_Pnt Pnt()
        {
            return myPnt;

        }

        public double Tolerance()
        {
            return myTolerance;
        }

        public void Pnt(gp_Pnt P)
        {
            myPnt = P;
        }


        gp_Pnt myPnt;
        double myTolerance;
        BRep_ListOfPointRepresentation myPoints;
        public BRep_ListOfPointRepresentation Points()
        {
            return myPoints;
        }
        public BRep_ListOfPointRepresentation ChangePoints()
        {
            return myPoints;
        }


        internal void UpdateTolerance(double T)
        {
            if (T > myTolerance) myTolerance = T;
        }

        public override TopoDS_TShape EmptyCopy()
        {
            throw new NotImplementedException();
        }
    }
}