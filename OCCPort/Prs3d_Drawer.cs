using OCCPort;
using System;
using System.Runtime.InteropServices;

namespace OCCPort
{
    public class Prs3d_Drawer : Graphic3d_PresentationAttributes
    {
        public Prs3d_Drawer()
        {
            myNbPoints = (-1);
            /*myMaximalParameterValue(-1.0),
            myChordialDeviation(-1.0),
            myTypeOfDeflection(Aspect_TOD_RELATIVE),
            myHasOwnTypeOfDeflection(Standard_False),
            myTypeOfHLR(Prs3d_TOH_NotSet),
            myDeviationCoefficient(-1.0),
            myDeviationAngle(-1.0),
            myIsoOnPlane(Standard_False),
            myHasOwnIsoOnPlane(Standard_False),
            myIsoOnTriangulation(Standard_False),
            myHasOwnIsoOnTriangulation(Standard_False),
            myIsAutoTriangulated(Standard_True),
            myHasOwnIsAutoTriangulated(Standard_False),

            myWireDraw(Standard_True),
            myHasOwnWireDraw(Standard_False),
            myLineArrowDraw(Standard_False),
            myHasOwnLineArrowDraw(Standard_False),
            myDrawHiddenLine(Standard_False),
            myHasOwnDrawHiddenLine(Standard_False),
            myVertexDrawMode(Prs3d_VDM_Inherited),*/

            myFreeBoundaryDraw = (true);
            myHasOwnFreeBoundaryDraw = (false);
            myUnFreeBoundaryDraw = (true);
            myHasOwnUnFreeBoundaryDraw = (false);/*
  myFaceBoundaryUpperContinuity(-1),
  myFaceBoundaryDraw(Standard_False),
  myHasOwnFaceBoundaryDraw(Standard_False),

  myHasOwnDimLengthModelUnits(Standard_False),
  myHasOwnDimAngleModelUnits(Standard_False),
  myHasOwnDimLengthDisplayUnits(Standard_False),
  myHasOwnDimAngleDisplayUnits(Standard_False)*/
        }

        Prs3d_LineAspect myFreeBoundaryAspect;
        bool myFreeBoundaryDraw;
        bool myHasOwnFreeBoundaryDraw;
        Prs3d_LineAspect myUnFreeBoundaryAspect;
        bool myUnFreeBoundaryDraw;
        bool myHasOwnUnFreeBoundaryDraw;
        Prs3d_LineAspect myFaceBoundaryAspect;
        int myFaceBoundaryUpperContinuity; //!< the most edge continuity class (GeomAbs_Shape) to be included to face boundaries presentation, or -1 if undefined
        bool myFaceBoundaryDraw;
        bool myHasOwnFaceBoundaryDraw;
        //! Changes highlight method to the given one.
        public virtual void SetMethod(Aspect_TypeOfHighlightMethod theMethod) { myHiMethod = theMethod; }

        public void SetAutoTriangulation(bool theIsEnabled)
        {
            myHasOwnIsAutoTriangulated = true;
            myIsAutoTriangulated = theIsEnabled;
        }
        Aspect_TypeOfHighlightMethod myHiMethod;            //!< box or color highlighting


        public void SetupOwnDefaults()
        {
            myNbPoints = 30;
            myMaximalParameterValue = 500000.0;
            myChordialDeviation = 0.0001;
            myDeviationCoefficient = 0.001;
            myDeviationAngle = 20.0 * Math.PI / 180.0;
            SetupOwnShadingAspect();
            /*SetupOwnPointAspect();
            SetOwnDatumAspects();*/
            SetOwnLineAspects();
            /*
            SetTextAspect(new Prs3d_TextAspect());
            SetDimensionAspect(new Prs3d_DimensionAspect());*/
        }

        static Quantity_NameOfColor THE_DEF_COLOR_FaceBoundary = Quantity_NameOfColor.Quantity_NOC_BLACK;
        static Quantity_NameOfColor THE_DEF_COLOR_FreeBoundary = Quantity_NameOfColor.Quantity_NOC_GREEN;
        // =======================================================================
        // function : SetOwnLineAspects
        // purpose  :
        // =======================================================================
        public bool SetOwnLineAspects(Prs3d_Drawer theDefaults = null)
        {
            if (theDefaults == null)
            {
                theDefaults = new Prs3d_Drawer();
            }
            bool isUpdateNeeded = false;
            /*  const Handle(Prs3d_Drawer)&aLink = (!theDefaults.IsNull() && theDefaults != this) ? theDefaults : myLink;
              if (myUIsoAspect.IsNull())
              {
                  isUpdateNeeded = true;
                  myUIsoAspect = new Prs3d_IsoAspect(Quantity_NOC_GRAY75, Aspect_TOL_SOLID, 1.0, 1);
                  if (const Prs3d_IsoAspect* aLinked = !aLink.IsNull() ? aLink->UIsoAspect().get() : NULL)
      {
                      *myUIsoAspect->Aspect() = *aLinked->Aspect();
                      myUIsoAspect->SetNumber(aLinked->Number());
                  }
              }
              if (myVIsoAspect.IsNull())
              {
                  isUpdateNeeded = true;
                  myVIsoAspect = new Prs3d_IsoAspect(Quantity_NOC_GRAY75, Aspect_TOL_SOLID, 1.0, 1);
                  if (const Prs3d_IsoAspect* aLinked = !aLink.IsNull() ? aLink->VIsoAspect().get() : NULL)
      {
                      *myVIsoAspect->Aspect() = *aLinked->Aspect();
                      myVIsoAspect->SetNumber(aLinked->Number());
                  }
              }
              if (myWireAspect.IsNull())
              {
                  isUpdateNeeded = true;
                  myWireAspect = new Prs3d_LineAspect(THE_DEF_COLOR_Wire, Aspect_TOL_SOLID, 1.0);
                  if (const Prs3d_LineAspect* aLinked = !aLink.IsNull() ? aLink->WireAspect().get() : NULL)
      {
                      *myWireAspect->Aspect() = *aLinked->Aspect();
                  }
              }
              if (myLineAspect.IsNull())
              {
                  isUpdateNeeded = true;
                  myLineAspect = new Prs3d_LineAspect(THE_DEF_COLOR_Line, Aspect_TOL_SOLID, 1.0);
                  if (const Prs3d_LineAspect* aLinked = !aLink.IsNull() ? aLink->LineAspect().get() : NULL)
      {
                      *myLineAspect->Aspect() = *aLinked->Aspect();
                  }
              }
              if (mySeenLineAspect.IsNull())
              {
                  isUpdateNeeded = true;
                  mySeenLineAspect = new Prs3d_LineAspect(THE_DEF_COLOR_SeenLine, Aspect_TOL_SOLID, 1.0);
                  if (const Prs3d_LineAspect* aLinked = !aLink.IsNull() ? aLink->SeenLineAspect().get() : NULL)
      {
                      *mySeenLineAspect->Aspect() = *aLinked->Aspect();
                  }
              }
              if (myHiddenLineAspect.IsNull())
              {
                  isUpdateNeeded = true;
                  myHiddenLineAspect = new Prs3d_LineAspect(THE_DEF_COLOR_HiddenLine, Aspect_TOL_DASH, 1.0);
                  if (const Prs3d_LineAspect* aLinked = !aLink.IsNull() ? aLink->HiddenLineAspect().get() : NULL)
      {
                      *myHiddenLineAspect->Aspect() = *aLinked->Aspect();
                  }
              }*/
            if (myFreeBoundaryAspect == null)
            {
                isUpdateNeeded = true;
                myFreeBoundaryAspect = new Prs3d_LineAspect(new Quantity_Color(THE_DEF_COLOR_FreeBoundary), Aspect_TypeOfLine.Aspect_TOL_SOLID, 1.0);
                /* if (const Prs3d_LineAspect* aLinked = !aLink.IsNull() ? aLink->FreeBoundaryAspect().get() : NULL)
     {
                     *myFreeBoundaryAspect->Aspect() = *aLinked->Aspect();
                 }*/
            }
            /*  if (myUnFreeBoundaryAspect.IsNull())
              {
                  isUpdateNeeded = true;
                  myUnFreeBoundaryAspect = new Prs3d_LineAspect(THE_DEF_COLOR_UnFreeBoundary, Aspect_TOL_SOLID, 1.0);
                  if (const Prs3d_LineAspect* aLinked = !aLink.IsNull() ? aLink->UnFreeBoundaryAspect().get() : NULL)
      {
                      *myUnFreeBoundaryAspect->Aspect() = *aLinked->Aspect();
                  }
              }*/
            isUpdateNeeded = SetupOwnFaceBoundaryAspect(theDefaults) || isUpdateNeeded;
            return isUpdateNeeded;
        }
        public bool SetupOwnFaceBoundaryAspect(Prs3d_Drawer theDefaults = null)
        {
            if (theDefaults == null)
            {
                theDefaults = new Prs3d_Drawer();
            }
            if (myFaceBoundaryAspect != null)
            {
                return false;
            }

            myFaceBoundaryAspect = new Prs3d_LineAspect(new Quantity_Color(THE_DEF_COLOR_FaceBoundary), Aspect_TypeOfLine.Aspect_TOL_SOLID, 1.0);

            /* const Handle(Prs3d_Drawer)&aLink = (!theDefaults.IsNull() && theDefaults != this) ? theDefaults : myLink;
             if (const Prs3d_LineAspect* aLinked = !aLink.IsNull() ? aLink->FaceBoundaryAspect().get() : NULL)
   {
                 *myFaceBoundaryAspect->Aspect() = *aLinked->Aspect();
             }*/
            return true;
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

        }

        //! Sets the parameter theAspect for display attributes of shading.
        public void SetShadingAspect(Prs3d_ShadingAspect theAspect)
        {
            myShadingAspect = theAspect;
        }

        //! Returns true if the drawer has its own attribute for
        //! shading aspect that overrides the one in the link.
        public bool HasOwnShadingAspect() { return myShadingAspect != null; }

        // =======================================================================
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

        Prs3d_ShadingAspect myShadingAspect = new Prs3d_ShadingAspect();
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