using System;
using System.Runtime.InteropServices;

namespace OCCPort
{
    public class Prs3d_Drawer : Graphic3d_PresentationAttributes
    {

        //! Changes highlight method to the given one.
        public virtual void SetMethod(Aspect_TypeOfHighlightMethod theMethod) { myHiMethod = theMethod; }

        public void SetAutoTriangulation(bool theIsEnabled)
        {
            myHasOwnIsAutoTriangulated = true;
            myIsAutoTriangulated = theIsEnabled;
        }
        Aspect_TypeOfHighlightMethod myHiMethod;            //!< box or color highlighting

        bool myFaceBoundaryDraw;
        bool myHasOwnFaceBoundaryDraw;
        public void SetupOwnDefaults()
        {
            myNbPoints = 30;
            myMaximalParameterValue = 500000.0;
            myChordialDeviation = 0.0001;
            myDeviationCoefficient = 0.001;
            myDeviationAngle = 20.0 * Math.PI / 180.0;
            //SetupOwnShadingAspect();
            /*SetupOwnPointAspect();
            SetOwnDatumAspects();
            SetOwnLineAspects();
            SetTextAspect(new Prs3d_TextAspect());
            SetDimensionAspect(new Prs3d_DimensionAspect());*/
        }
        //! Sets presentation Zlayer.
        public virtual void SetZLayer(Graphic3d_ZLayerId theLayer) { myZLayer = theLayer; }
        Graphic3d_ZLayerId myZLayer;              //!< Z-layer
                                                  //! Sets theDrawer as a link to which the current object references.
        public void Link(Prs3d_Drawer theDrawer)
        {
            SetLink(theDrawer);
        }
        //! Sets theDrawer as a link to which the current object references.
        public void SetLink(Prs3d_Drawer theDrawer) { myLink = theDrawer; }



        public bool SetupOwnShadingAspect(Prs3d_Drawer theDefaults = null)
        {
            if (theDefaults == null)
            {
                theDefaults = new Prs3d_Drawer();
            }
            if (myShadingAspect != null)
            {
                return false;
            }

            myShadingAspect = new Prs3d_ShadingAspect();
            Prs3d_Drawer aLink = (theDefaults != null && theDefaults != this) ? theDefaults : myLink;
            /*if (const Prs3d_ShadingAspect* aLinked = !aLink.IsNull() ? aLink->ShadingAspect().get() : NULL)
  {
                *myShadingAspect->Aspect() = *aLinked->Aspect();
            }*/
            return true;
        }

        //! Get the most edge continuity class; GeomAbs_CN by default (all edges).
        public GeomAbs_Shape FaceBoundaryUpperContinuity()
        {
            return HasOwnFaceBoundaryUpperContinuity()
                 ? (GeomAbs_Shape)myFaceBoundaryUpperContinuity
                 : (myLink != null
                   ? myLink.FaceBoundaryUpperContinuity()
                   : GeomAbs_Shape.GeomAbs_CN);
        }

        //! Returns true if the drawer has its own attribute for face boundaries upper edge continuity class that overrides the one in the link.
        public bool HasOwnFaceBoundaryUpperContinuity() { return myFaceBoundaryUpperContinuity != -1; }
        int myFaceBoundaryUpperContinuity; //!< the most edge continuity class (GeomAbs_Shape) to be included to face boundaries presentation, or -1 if undefined


        //! Checks whether the face boundary drawing is enabled or not.
        public bool FaceBoundaryDraw()
        {
            return myHasOwnFaceBoundaryDraw || myLink == null
                 ? myFaceBoundaryDraw
                 : myLink.FaceBoundaryDraw();
        }

        internal bool IsAutoTriangulation()
        {

            return myHasOwnIsAutoTriangulated || myLink == null
                 ? myIsAutoTriangulated
                 : myLink.IsAutoTriangulation();

        }// =======================================================================
         // function : ShadingAspect
         // purpose  :
         // =======================================================================

        public Prs3d_ShadingAspect ShadingAspect()
        {
            if (myShadingAspect == null
            && myLink != null)
            {
                return myLink.ShadingAspect();
            }
            return myShadingAspect;
        }

        Prs3d_ShadingAspect myShadingAspect;
        Prs3d_Drawer myLink;

        int myNbPoints;
        double myMaximalParameterValue;
        double myChordialDeviation;
        //Aspect_TypeOfDeflection myTypeOfDeflection;
        bool myHasOwnTypeOfDeflection;
        //Prs3d_TypeOfHLR myTypeOfHLR;
        double myDeviationCoefficient;
        double myPreviousDeviationCoefficient;
        double myDeviationAngle;
        double myPreviousDeviationAngle;
        bool myIsoOnPlane;
        bool myHasOwnIsoOnPlane;
        bool myIsoOnTriangulation;
        bool myHasOwnIsoOnTriangulation;
        bool myIsAutoTriangulated;
        bool myHasOwnIsAutoTriangulated;

        /*Handle(Prs3d_IsoAspect)       myUIsoAspect;
    Handle(Prs3d_IsoAspect)       myVIsoAspect;
    Handle(Prs3d_LineAspect)      myWireAspect;
    Standard_Boolean myWireDraw;
        Standard_Boolean myHasOwnWireDraw;
        Handle(Prs3d_PointAspect)     myPointAspect;
    Handle(Prs3d_LineAspect)      myLineAspect;
    Handle(Prs3d_TextAspect)      myTextAspect;
    Handle(Prs3d_ShadingAspect)   myShadingAspect;
    Handle(Prs3d_PlaneAspect)     myPlaneAspect;
    Handle(Prs3d_LineAspect)      mySeenLineAspect;
    Handle(Prs3d_ArrowAspect)     myArrowAspect;
    Standard_Boolean myLineArrowDraw;
        Standard_Boolean myHasOwnLineArrowDraw;
        Handle(Prs3d_LineAspect)      myHiddenLineAspect;
    Standard_Boolean myDrawHiddenLine;
        Standard_Boolean myHasOwnDrawHiddenLine;
        Handle(Prs3d_LineAspect)      myVectorAspect;
    Prs3d_VertexDrawMode myVertexDrawMode;
        Handle(Prs3d_DatumAspect)     myDatumAspect;
    Handle(Prs3d_LineAspect)      mySectionAspect;

    Handle(Prs3d_LineAspect)      myFreeBoundaryAspect;
    Standard_Boolean myFreeBoundaryDraw;
        Standard_Boolean myHasOwnFreeBoundaryDraw;
        Handle(Prs3d_LineAspect)      myUnFreeBoundaryAspect;
    Standard_Boolean myUnFreeBoundaryDraw;
        Standard_Boolean myHasOwnUnFreeBoundaryDraw;
        Handle(Prs3d_LineAspect)      myFaceBoundaryAspect;
    Standard_Integer myFaceBoundaryUpperContinuity; //!< the most edge continuity class (GeomAbs_Shape) to be included to face boundaries presentation, or -1 if undefined
        Standard_Boolean myFaceBoundaryDraw;
        Standard_Boolean myHasOwnFaceBoundaryDraw;

        Handle(Prs3d_DimensionAspect) myDimensionAspect;
    Prs3d_DimensionUnits myDimensionModelUnits;
        Standard_Boolean myHasOwnDimLengthModelUnits;
        Standard_Boolean myHasOwnDimAngleModelUnits;
        Prs3d_DimensionUnits myDimensionDisplayUnits;
        Standard_Boolean myHasOwnDimLengthDisplayUnits;
        Standard_Boolean myHasOwnDimAngleDisplayUnits;
        */
        internal bool IsoOnTriangulation()
        {
            throw new NotImplementedException();
        }
        Prs3d_LineAspect myFaceBoundaryAspect;
        Prs3d_LineAspect myFreeBoundaryAspect;

        public Prs3d_LineAspect FaceBoundaryAspect()
        {
            if (myFaceBoundaryAspect == null
            && myLink != null)
            {
                return myLink.FaceBoundaryAspect();
            }
            return myFaceBoundaryAspect;
        }
        internal Prs3d_LineAspect FreeBoundaryAspect()
        {
            if (myFreeBoundaryAspect == null && myLink != null)
            {
                return myLink.FreeBoundaryAspect();
            }
            return myFreeBoundaryAspect;
        }
    }
    //! Provides information about the continuity of a curve:
    //! -   C0: only geometric continuity.
    //! -   G1: for each point on the curve, the tangent vectors
    //! "on the right" and "on the left" are collinear with the same orientation.
    //! -   C1: continuity of the first derivative. The "C1" curve is
    //! also "G1" but, in addition, the tangent vectors " on the
    //! right" and "on the left" are equal.
    //! -   G2: for each point on the curve, the normalized
    //! normal vectors "on the right" and "on the left" are equal.
    //! -   C2: continuity of the second derivative.
    //! -   C3: continuity of the third derivative.
    //! -   CN: continuity of the N-th derivative, whatever is the
    //! value given for N (infinite order of continuity).
    //! Also provides information about the continuity of a surface:
    //! -   C0: only geometric continuity.
    //! -   C1: continuity of the first derivatives; any
    //! isoparametric (in U or V) of a surface "C1" is also "C1".
    //! -   G2: for BSpline curves only; "on the right" and "on the
    //! left" of a knot the computation of the "main curvature
    //! radii" and the "main directions" (when they exist) gives the same result.
    //! -   C2: continuity of the second derivative.
    //! -   C3: continuity of the third derivative.
    //! -   CN: continuity of any N-th derivative, whatever is the
    //! value given for N (infinite order of continuity).
    //! We may also say that a surface is "Ci" in u, and "Cj" in v
    //! to indicate the continuity of its derivatives up to the order
    //! i in the u parametric direction, and j in the v parametric direction.
    public enum GeomAbs_Shape
    {
        GeomAbs_C0,
        GeomAbs_G1,
        GeomAbs_C1,
        GeomAbs_G2,
        GeomAbs_C2,
        GeomAbs_C3,
        GeomAbs_CN
    }

}