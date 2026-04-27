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
        public override void Update()
        {
            double f = First();
            double l = Last();
            bool isneg = Precision.IsNegativeInfinite(f);
            bool ispos = Precision.IsPositiveInfinite(l);
            if (!isneg)
            {
                myPCurve.D0(f, ref myUV1);
            }
            if (!ispos)
            {
                myPCurve.D0(l,ref myUV2);
            }
        }

        Geom2d_Curve myPCurve;
        Geom_Surface mySurface;

        protected gp_Pnt2d myUV1;
        protected gp_Pnt2d myUV2;



        public override bool IsCurveOnSurface()
        {
            return true;
        }


    }
}