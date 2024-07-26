using System;
using System.Collections.Generic;

namespace OCCPort
{
    //! Root   class    for    the    geometric     curves
    //! representation. Contains a range.
    //! Contains a first and a last parameter.

    internal class BRep_GCurve : BRep_CurveRepresentation
    {
        public override bool IsCurve3D()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {

        }
        public void SetRange(double First,

                   double Last)
        {
            myFirst = First;
            myLast = Last;
            Update();
        }

        protected double myFirst;
        protected double myLast;

        public BRep_GCurve(TopLoc_Location l, double v1, double v2)
        {
        }

        //=======================================================================

        public void Range(double First,
                           double Last)
        {
            First = myFirst;
            Last = myLast;
        }


    }//! Root class for the curve repre ntations. Contains
}
