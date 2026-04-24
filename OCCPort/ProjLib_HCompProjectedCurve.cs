using System;
using System.Collections.Generic;
using static OpenTK.Audio.OpenAL.AL;

namespace OCCPort
{
    public class ProjLib_CompProjectedCurve : Adaptor2d_Curve2d
    {
        public ProjLib_CompProjectedCurve(Adaptor3d_Surface theSurface,
            Adaptor3d_Curve theCurve, double theTolU, double theTolV, double theMaxDist)
        {
            mySurface = (theSurface);
            myCurve = (theCurve);
            myNbCurves = (0);
            //mySequence(new ProjLib_HSequenceOfHSequenceOfPnt()),
            myTol3d = (1e-6);
            myContinuity = GeomAbs_Shape.GeomAbs_C2;
            myMaxDegree = (14);
            myMaxSeg = (16);
            myProj2d = (true);
            myProj3d = (false);
            myMaxDist = (theMaxDist);
            myTolU = theTolU;
            myTolV = theTolV;
            Init();

        }
        int myNbCurves;
        double myTol3d;

        Adaptor3d_Surface mySurface;
        Adaptor3d_Curve myCurve;

        GeomAbs_Shape myContinuity;
        int myMaxDegree;
        int myMaxSeg;
        bool myProj2d;
        bool myProj3d;
        double myMaxDist;
        double myTolU;
        double myTolV;
        TColStd_HArray1OfReal myTabInt;

        void Init()
        {
            myTabInt = null;
            List<double> aSplits = new List<double>();
            aSplits.Clear();

            /*
             a lot of code here
             */

        }
        public override gp_Lin2d Line()
        {
            throw new System.NotImplementedException();
        }

        public override GeomAbs_CurveType _GetType()
        {
            return GeomAbs_CurveType.GeomAbs_OtherCurve;


        }

        internal int NbCurves()
        {
            return myNbCurves;

        }
        ProjLib_SequenceOfHSequenceOfPnt mySequence;

        internal void Bounds(int Index, out double Udeb, out double Ufin)
        {
            if (Index < 1 || Index > myNbCurves) 
                throw new Standard_NoSuchObject();

            Udeb = mySequence.Value(Index).Value(1).X();
            Ufin = mySequence.Value(Index).Value(mySequence.Value(Index).Length()).X();

        }
    }
}