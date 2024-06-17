using System;

namespace OCCPort
{
    //! Root   class    for    the    geometric     curves
    //! representation. Contains a range.
    //! Contains a first and a last parameter.

    internal class BRep_GCurve : BRep_CurveRepresentation
    {
        internal bool IsCurve3D()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {

        }

        protected double myFirst;
        protected double myLast;

        public BRep_GCurve(TopLoc_Location l, double v1, double v2)
        {
        }

        internal void Range(double f, double l)
        {
            throw new NotImplementedException();
        }

        internal bool IsCurveOnSurface(Geom_Surface s, TopLoc_Location l)
        {
            throw new NotImplementedException();
        }
    }//! Root class for the curve representations. Contains
}
