using OpenTK.Graphics.ES20;
using System.Security.Cryptography;

namespace OCCPort
{

    //! An interface between the services provided by a curve
    //! lying on a surface from the package Geom and those
    //! required of the curve by algorithms which use it. The
    //! curve is defined as a 2D curve from the Geom2d
    //! package, in the parametric space of the surface.
    public class Adaptor3d_CurveOnSurface : Adaptor3d_Curve
    {

        Adaptor3d_Surface mySurface;
        Adaptor2d_Curve2d myCurve;
        GeomAbs_CurveType myType;
        //gp_Circ myCirc;
        gp_Lin myLin;
        Adaptor3d_Surface myFirstSurf;
        Adaptor3d_Surface myLastSurf;
        //TColStd_HSequenceOfReal) myIntervals;
        GeomAbs_Shape myIntCont;
        public override GeomAbs_CurveType _GetType()
        {
            return myType;

        }

        public override int Degree()
        {
            // on a parametric surface should multiply
            // return TheCurve2dTool::Degree(myCurve);

            return myCurve.Degree();
        }

        public override int NbKnots()
        {
            if (mySurface._GetType() == GeomAbs_SurfaceType.GeomAbs_Plane)
                return myCurve.NbKnots();
            else
            {
                throw new Standard_NoSuchObject();
            }
        }

        public override Geom_BSplineCurve BSpline()
        {
            throw new System.NotImplementedException();
        }

        public override gp_Pnt Value(double d)
        {
            throw new System.NotImplementedException();
        }

        public override void D0(double d, ref gp_Pnt p)
        {
            throw new System.NotImplementedException();
        }
    }
}