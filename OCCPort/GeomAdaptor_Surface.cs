using OCCPort;
using System.Security.AccessControl;

namespace OCCPort
{
    //! An interface between the services provided by any
    //! surface from the package Geom and those required
    //! of the surface by algorithms which use it.
    //! Creation of the loaded surface the surface is C1 by piece
    //!
    //! Polynomial coefficients of BSpline surfaces used for their evaluation are
    //! cached for better performance. Therefore these evaluations are not
    //! thread-safe and parallel evaluations need to be prevented.
    public class GeomAdaptor_Surface : Adaptor3d_Surface
    {

        public void Load(Geom_Surface theSurf)
        {
            if (theSurf == null)
            {
                throw new Standard_NullObject("GeomAdaptor_Surface::Load");
            }

            double aU1 = 0, aU2 = 0, aV1 = 0, aV2 = 0;
            theSurf.Bounds(ref aU1, ref aU2, ref aV1, ref aV2);
            load(theSurf, aU1, aU2, aV1, aV2);
        }



        //! Returns the type of the surface : Plane, Cylinder,
        //! Cone,      Sphere,        Torus,    BezierSurface,
        //! BSplineSurface,               SurfaceOfRevolution,
        //! SurfaceOfExtrusion, OtherSurface
        public virtual new GeomAbs_SurfaceType GetType() { return mySurfaceType; }
        //! Standard_ConstructionError is raised if theUFirst>theULast or theVFirst>theVLast
        public void Load(Geom_Surface theSurf,
             double theUFirst, double theULast,
             double theVFirst, double theVLast,
             double theTolU = 0.0, double theTolV = 0.0)
        {
            if (theSurf == null)
            {
                throw new Standard_NullObject("GeomAdaptor_Surface::Load");
            }
            if (theUFirst > theULast || theVFirst > theVLast)
            {
                throw new Standard_ConstructionError("GeomAdaptor_Surface::Load");
            }

            load(theSurf, theUFirst, theULast, theVFirst, theVLast, theTolU, theTolV);
        }

        Geom_Surface mySurface;
        double myUFirst;
        double myULast;
        double myVFirst;
        double myVLast;
        double myTolU;
        double myTolV;

        //=======================================================================
        //function : Load
        //purpose  : 
        //=======================================================================

        public void load(Geom_Surface S,
                               double UFirst,
                                 double ULast,
                                double VFirst,
                                double VLast,
                                double TolU=0.0,
                                double TolV=0.0)
        {
            myTolU = TolU;
            myTolV = TolV;
            myUFirst = UFirst;
            myULast = ULast;
            myVFirst = VFirst;
            myVLast = VLast;
            //  mySurfaceCache.Nullify();

            if (mySurface != S)
            {
                mySurface = S;
                //  myNestedEvaluator.Nullify();
                //myBSplineSurface.Nullify();

                var TheType = S.DynamicType();
                if (TheType == typeof(Geom_RectangularTrimmedSurface))
                {
                    Load(((Geom_RectangularTrimmedSurface)S).BasisSurface(),
                         UFirst, ULast, VFirst, VLast);
                }
                else if (TheType == typeof(Geom_Plane))
                    mySurfaceType = GeomAbs_SurfaceType.GeomAbs_Plane;
                /*else if (TheType == typeof(Geom_CylindricalSurface))
                    mySurfaceType = GeomAbs_SurfaceType.GeomAbs_Cylinder;
                else if (TheType == typeof(Geom_ConicalSurface))
                    mySurfaceType = GeomAbs_SurfaceType.GeomAbs_Cone;
                else if (TheType == typeof(Geom_SphericalSurface))
                    mySurfaceType = GeomAbs_SurfaceType.GeomAbs_Sphere;
                else if (TheType == typeof(Geom_ToroidalSurface))
                    mySurfaceType = GeomAbs_SurfaceType.GeomAbs_Torus;
                else if (TheType == typeof(Geom_SurfaceOfRevolution))
                {
                    mySurfaceType = GeomAbs_SurfaceOfRevolution;
                    Handle(Geom_SurfaceOfRevolution) myRevSurf =
                        Handle(Geom_SurfaceOfRevolution)::DownCast(mySurface);
                    // Create nested adaptor for base curve
                    Handle(Geom_Curve) aBaseCurve = myRevSurf->BasisCurve();
                    Handle(Adaptor3d_Curve) aBaseAdaptor = new GeomAdaptor_Curve(aBaseCurve);
                    // Create corresponding evaluator
                    myNestedEvaluator =
                        new GeomEvaluator_SurfaceOfRevolution(aBaseAdaptor, myRevSurf->Direction(), myRevSurf->Location());
                }
                else if (TheType == STANDARD_TYPE(Geom_SurfaceOfLinearExtrusion))
                {
                    mySurfaceType = GeomAbs_SurfaceOfExtrusion;
                    Handle(Geom_SurfaceOfLinearExtrusion) myExtSurf =
                        Handle(Geom_SurfaceOfLinearExtrusion)::DownCast(mySurface);
                    // Create nested adaptor for base curve
                    Handle(Geom_Curve) aBaseCurve = myExtSurf->BasisCurve();
                    Handle(Adaptor3d_Curve) aBaseAdaptor = new GeomAdaptor_Curve(aBaseCurve);
                    // Create corresponding evaluator
                    myNestedEvaluator =
                      new GeomEvaluator_SurfaceOfExtrusion(aBaseAdaptor, myExtSurf->Direction());
                }
                else if (TheType == STANDARD_TYPE(Geom_BezierSurface))
                {
                    mySurfaceType = GeomAbs_BezierSurface;
                }
                else if (TheType == STANDARD_TYPE(Geom_BSplineSurface))
                {
                    mySurfaceType = GeomAbs_BSplineSurface;
                    myBSplineSurface = Handle(Geom_BSplineSurface)::DownCast(mySurface);
                }
                else if (TheType == STANDARD_TYPE(Geom_OffsetSurface))
                {
                    mySurfaceType = GeomAbs_OffsetSurface;
                    Handle(Geom_OffsetSurface) myOffSurf = Handle(Geom_OffsetSurface)::DownCast(mySurface);
                    // Create nested adaptor for base surface
                    Handle(Geom_Surface) aBaseSurf = myOffSurf->BasisSurface();
                    Handle(GeomAdaptor_Surface) aBaseAdaptor =
                        new GeomAdaptor_Surface(aBaseSurf, myUFirst, myULast, myVFirst, myVLast, myTolU, myTolV);
                    myNestedEvaluator = new GeomEvaluator_OffsetSurface(
                        aBaseAdaptor, myOffSurf->Offset(), myOffSurf->OsculatingSurface());
                }*/
                else
                    mySurfaceType = GeomAbs_SurfaceType.GeomAbs_OtherSurface;
            }
        }

        public GeomAdaptor_Surface()
        {
            /*myUFirst=(0.), myULast(0.),
	   myVFirst(0.), myVLast(0.),
	   myTolU(0.),   myTolV(0.),*/
            mySurfaceType = GeomAbs_SurfaceType.GeomAbs_OtherSurface;

        }
        GeomAbs_SurfaceType mySurfaceType;

    }
}