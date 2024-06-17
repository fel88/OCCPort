using System;

namespace OCCPort
{
    internal class BRep_CurveOnSurface : BRep_GCurve
    {
        public BRep_CurveOnSurface(Geom2d_Curve PC,
            Geom_Surface S, TopLoc_Location L)
            : base(L, PC.FirstParameter(), PC.LastParameter())
        {


            myPCurve = PC;
            mySurface = S;

        }

        Geom2d_Curve myPCurve;
        Geom_Surface mySurface;

        protected gp_Pnt2d myUV1;
        protected gp_Pnt2d myUV2;




        internal void SetRange(double aFCur, double aLCur)
        {
            throw new NotImplementedException();
        }
    }
}