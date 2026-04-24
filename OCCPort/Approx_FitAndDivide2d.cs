using System;

namespace OCCPort
{
    public class Approx_FitAndDivide2d
    {

        //AppParCurves_SequenceOfMultiCurve myMultiCurves;
        //TColStd_SequenceOfReal myfirstparam;
        //TColStd_SequenceOfReal mylastparam;
        //AppParCurves_MultiCurve TheMultiCurve;
        bool alldone;
        bool tolreached;
        //TColStd_SequenceOfReal Tolers3d;
        //TColStd_SequenceOfReal Tolers2d;
        int mydegremin;
        int mydegremax;
        double mytol3d;
        double mytol2d;
        double currenttol3d;
        double currenttol2d;
        bool mycut;
        AppParCurves_Constraint myfirstC;
        AppParCurves_Constraint mylastC;
        int myMaxSegments;
        bool myInvOrder;
        bool myHangChecking;

        //! The MultiLine <Line> will be approximated until tolerances
        //! will be reached.
        //! The approximation will be done from degreemin to degreemax
        //! with a cutting if the corresponding boolean is True.
        public Approx_FitAndDivide2d(AppCont_Function Line, int degreemin = 3, int degreemax = 8,

            double Tolerance3d = 1.0e-5, double Tolerance2d = 1.0e-5, bool cutting = false,
            AppParCurves_Constraint FirstC = AppParCurves_Constraint.AppParCurves_TangencyPoint,
            AppParCurves_Constraint LastC = AppParCurves_Constraint.AppParCurves_TangencyPoint)
        {

        }

        //! Initializes the fields of the algorithm.
        public Approx_FitAndDivide2d(int degreemin = 3, int degreemax = 8, double Tolerance3d = 1.0e-05,
            double Tolerance2d = 1.0e-05, bool cutting = false, AppParCurves_Constraint FirstC = AppParCurves_Constraint.AppParCurves_TangencyPoint,
            AppParCurves_Constraint LastC = AppParCurves_Constraint.AppParCurves_TangencyPoint)
        {

        }


        double myTolerance;
        Geom2d_BSplineCurve myBSpline;
        Geom2d_BezierCurve myBezier;
        int myDegMin;
        int myDegMax;
        
        AppParCurves_Constraint myBndPnt;

        //! Changes the max number of segments, which is allowed for cutting.
        public void SetMaxSegments(int theMaxSegments)
        {
            myMaxSegments = theMaxSegments;

        }

        internal void SetHangChecking(bool theHangChecking)
        {
            myHangChecking = theHangChecking;

        }

        internal void Perform(ProjLib_Function f)
        {
            throw new NotImplementedException();
        }
    }
}