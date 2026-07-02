using TKernel;
using TKMath;
using TKService;

namespace TKV3d
{
    //! A graphic attribute manager which governs how
    //! objects such as color, width, line thickness and deflection are displayed.
    //! A drawer includes an instance of the Aspect classes with particular default values.
    public class Prs3d_Drawer : Graphic3d_PresentationAttributes
    {
        public Prs3d_Drawer()
        {
            myNbPoints = (-1);
            /*myMaximalParameterValue(-1.0),*/
            myChordialDeviation = (-1.0);
            myTypeOfDeflection = Aspect_TypeOfDeflection.Aspect_TOD_RELATIVE;
            /*
            myHasOwnTypeOfDeflection(Standard_False),
            myTypeOfHLR(Prs3d_TOH_NotSet),
            myDeviationCoefficient(-1.0),*/
            myDeviationAngle = (-1.0);
            myIsoOnPlane = (false);
            myHasOwnIsoOnPlane = (false);
            myIsoOnTriangulation = (false);
            myHasOwnIsoOnTriangulation = (false);
            myIsAutoTriangulated = (true);
            myHasOwnIsAutoTriangulated = (false);
            myWireDraw = true;
            /*
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

        //! Sets the parameter theAspect for display of wires.
        public void SetWireAspect(Prs3d_LineAspect theAspect)
        {
            myWireAspect = theAspect;
        }
        public void SetFaceBoundaryDraw(bool theIsEnabled)
        {
            myHasOwnFaceBoundaryDraw = true;
            myFaceBoundaryDraw = theIsEnabled;
        }

        public void EnableDrawHiddenLine()
        {
            myHasOwnDrawHiddenLine = true;
            myDrawHiddenLine = true;
        }

        //! Returns True if the drawing of isos on planes is enabled.
        public bool IsoOnPlane()
        {
            return myHasOwnIsoOnPlane || myLink == null
                 ? myIsoOnPlane
                 : myLink.IsoOnPlane();
        }


        //! Sets the maximum value allowed for the first and last parameters of an infinite curve.
        //! By default, this value is 500000.
        public double MaximalParameterValue()
        {
            return myMaximalParameterValue > 0.0
                 ? myMaximalParameterValue
                 : (myLink != null
                   ? myLink.MaximalParameterValue()
                   : 500000.0);
        }


        //! Returns True if the drawing of the wire is enabled.
        public bool WireDraw()
        {
            return myHasOwnWireDraw || myLink == null
                 ? myWireDraw
                 : myLink.WireDraw();
        }

        //! Defines own attributes for drawing an U isoparametric curve of a face,
        //! settings from linked Drawer or NULL if neither was set.
        //!
        //! These attributes are used by the following algorithms:
        //!   Prs3d_WFDeflectionSurface
        //!   Prs3d_WFDeflectionRestrictedFace
        public Prs3d_IsoAspect UIsoAspect()
        {
            if (myUIsoAspect == null && myLink != null)
            {
                return myLink.UIsoAspect();
            }
            return myUIsoAspect;
        }

        public Prs3d_IsoAspect VIsoAspect()
        {
            if (myVIsoAspect == null
            && myLink != null)
            {
                return myLink.VIsoAspect();
            }
            return myVIsoAspect;
        }

        //! Returns the value for deviation angle in radians, 20 * M_PI / 180 by default.
        public double DeviationAngle()
        {
            return myDeviationAngle > 0.0
                 ? myDeviationAngle
                 : (myLink != null
                   ? myLink.DeviationAngle()
                   : 20.0 * Math.PI / 180.0);
        }

        public Prs3d_LineAspect WireAspect()
        {
            if (myWireAspect == null
            && myLink != null)
            {
                return myLink.WireAspect();
            }
            return myWireAspect;
        }

        //! Defines the maximal chordial deviation when drawing any curve.
        //! Even if the type of deviation is set to TOD_Relative, this value is used by: 
        //!   Prs3d_DeflectionCurve
        //!   Prs3d_WFDeflectionSurface
        //!   Prs3d_WFDeflectionRestrictedFace
        public void SetMaximalChordialDeviation(double theChordialDeviation)
        {

            myChordialDeviation = theChordialDeviation;
        }

        //! Returns the deviation coefficient.
        //! Drawings of curves or patches are made with respect
        //! to a maximal chordal deviation. A Deviation coefficient
        //! is used in the shading display mode. The shape is
        //! seen decomposed into triangles. These are used to
        //! calculate reflection of light from the surface of the
        //! object. The triangles are formed from chords of the
        //! curves in the shape. The deviation coefficient gives
        //! the highest value of the angle with which a chord can
        //! deviate from a tangent to a   curve. If this limit is
        //! reached, a new triangle is begun.
        //! This deviation is absolute and is set through the
        //! method: SetMaximalChordialDeviation. The default value is 0.001.
        //! In drawing shapes, however, you are allowed to ask
        //! for a relative deviation. This deviation will be:
        //! SizeOfObject * DeviationCoefficient.
        public double DeviationCoefficient()
        {
            return myDeviationCoefficient > 0.0
                 ? myDeviationCoefficient
                 : (myLink != null
                   ? myLink.DeviationCoefficient()
                   : 0.001);
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
        //! Returns the maximal chordal deviation. The default value is 0.0001.
        //! Drawings of curves or patches are made with respect to an absolute maximal chordal deviation.
        public double MaximalChordialDeviation()
        {
            return myChordialDeviation > 0.0
                 ? myChordialDeviation
                 : (myLink != null
                   ? myLink.MaximalChordialDeviation()
                   : 0.0001);
        }

        //! Returns the type of chordal deflection.
        //! This indicates whether the deflection value is absolute or relative to the size of the object.
        public Aspect_TypeOfDeflection TypeOfDeflection()
        {
            return myHasOwnTypeOfDeflection || myLink == null
                 ? myTypeOfDeflection
                 : myLink.TypeOfDeflection();
        }


        public void SetAutoTriangulation(bool theIsEnabled)
        {
            myHasOwnIsAutoTriangulated = true;
            myIsAutoTriangulated = theIsEnabled;
        }
        Aspect_TypeOfHighlightMethod myHiMethod;            //!< box or color highlighting

        public Prs3d_VertexDrawMode VertexDrawMode()
        {
            if (HasOwnVertexDrawMode())
            {
                return myVertexDrawMode;
            }
            else if (myLink != null)
            {
                return myLink.VertexDrawMode();
            }
            return Prs3d_VertexDrawMode.Prs3d_VDM_Isolated;
        }

        //! Returns true if the vertex draw mode is not equal to <b>Prs3d_VDM_Inherited</b>. 
        //! This means that individual vertex draw mode value (i.e. not inherited from the global 
        //! drawer) is used for a specific interactive object.
        public bool HasOwnVertexDrawMode() { return (myVertexDrawMode != Prs3d_VertexDrawMode.Prs3d_VDM_Inherited); }

        Prs3d_VertexDrawMode myVertexDrawMode;

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
        static Quantity_NameOfColor THE_DEF_COLOR_UnFreeBoundary = Quantity_NameOfColor.Quantity_NOC_YELLOW;
        static Quantity_NameOfColor THE_DEF_COLOR_Wire = Quantity_NameOfColor.Quantity_NOC_RED;
        static Quantity_NameOfColor THE_DEF_COLOR_Line = Quantity_NameOfColor.Quantity_NOC_YELLOW;
        static Quantity_NameOfColor THE_DEF_COLOR_SeenLine = Quantity_NameOfColor.Quantity_NOC_YELLOW;
        static Quantity_NameOfColor THE_DEF_COLOR_HiddenLine = Quantity_NameOfColor.Quantity_NOC_YELLOW;
        static Quantity_NameOfColor THE_DEF_COLOR_Vector = Quantity_NameOfColor.Quantity_NOC_SKYBLUE;
        static Quantity_NameOfColor THE_DEF_COLOR_Section = Quantity_NameOfColor.Quantity_NOC_ORANGE;

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
            Prs3d_Drawer aLink = (theDefaults != null && theDefaults != this) ? theDefaults : myLink;
            if (myUIsoAspect == null)
            {
                isUpdateNeeded = true;
                myUIsoAspect = new Prs3d_IsoAspect(new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY75), Aspect_TypeOfLine.Aspect_TOL_SOLID, 1.0, 1);
                Prs3d_IsoAspect aLinked = aLink != null ? aLink.UIsoAspect() : null;
                if (aLinked != null)
                {
                    //myUIsoAspect.Aspect() = aLinked.Aspect();
                    myUIsoAspect.myAspect = aLinked.Aspect();
                    myUIsoAspect.SetNumber(aLinked.Number());
                }
            }
            if (myVIsoAspect == null)
            {
                isUpdateNeeded = true;
                myVIsoAspect = new Prs3d_IsoAspect(new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY75), Aspect_TypeOfLine.Aspect_TOL_SOLID, 1.0, 1);
                Prs3d_IsoAspect aLinked = aLink != null ? aLink.VIsoAspect() : null;
                if (aLinked != null)
                {
                    //myVIsoAspect.Aspect() = aLinked.Aspect();
                    myVIsoAspect.myAspect = aLinked.Aspect();
                    myVIsoAspect.SetNumber(aLinked.Number());
                }
            }
            if (myWireAspect == null)
            {
                isUpdateNeeded = true;
                myWireAspect = new Prs3d_LineAspect(new Quantity_Color(THE_DEF_COLOR_Wire), Aspect_TypeOfLine.Aspect_TOL_SOLID, 1.0);
                Prs3d_LineAspect aLinked = aLink != null ? aLink.WireAspect() : null;
                if (aLinked != null)
                {
                    myWireAspect.myAspect = aLinked.Aspect();
                }
            }/*
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
                Prs3d_LineAspect aLinked = aLink != null ? aLink.FreeBoundaryAspect() : null;
                if (aLinked != null)
                {
                    myFreeBoundaryAspect.myAspect = aLinked.Aspect();
                }
            }
            if (myUnFreeBoundaryAspect == null)
            {
                isUpdateNeeded = true;
                myUnFreeBoundaryAspect = new Prs3d_LineAspect(new Quantity_Color(THE_DEF_COLOR_UnFreeBoundary), Aspect_TypeOfLine.Aspect_TOL_SOLID, 1.0);
                Prs3d_LineAspect aLinked = aLink != null ? aLink.UnFreeBoundaryAspect() : null;
                if (aLinked != null)
                {
                    myUnFreeBoundaryAspect.myAspect = aLinked.Aspect();
                }
            }
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
        Aspect_TypeOfDeflection myTypeOfDeflection;
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

        Prs3d_IsoAspect myUIsoAspect;
        Prs3d_IsoAspect myVIsoAspect;
        Prs3d_LineAspect myWireAspect;

        bool myWireDraw;
        bool myHasOwnWireDraw;
        /*
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

        public Prs3d_LineAspect FreeBoundaryAspect()
        {
            if (myFreeBoundaryAspect == null && myLink != null)
            {
                return myLink.FreeBoundaryAspect();
            }
            return myFreeBoundaryAspect;
        }

        public Prs3d_LineAspect UnFreeBoundaryAspect()
        {
            if (myUnFreeBoundaryAspect == null
            && myLink != null)
            {
                return myLink.UnFreeBoundaryAspect();
            }
            return myUnFreeBoundaryAspect;
        }


        //! Returns true if there is a local setting for deviation
        //! angle in this framework for a specific interactive object.
        public bool HasOwnDeviationAngle() { return myDeviationAngle > 0.0; }

        //! Returns true if there is a local setting for deviation
        //! coefficient in this framework for a specific interactive object.
        public bool HasOwnDeviationCoefficient() { return myDeviationCoefficient > 0.0; }

        internal double PreviousDeviationAngle()
        {
            return HasOwnDeviationAngle()
        ? myPreviousDeviationAngle
        : 0.0;
        }

        internal double PreviousDeviationCoefficient()
        {
            return HasOwnDeviationCoefficient()
        ? myPreviousDeviationCoefficient
        : 0.0;
        }

        internal void UpdatePreviousDeviationAngle()
        {
            if (HasOwnDeviationAngle())
            {
                myPreviousDeviationAngle = DeviationAngle();
            }
        }

        internal void UpdatePreviousDeviationCoefficient()
        {
            if (HasOwnDeviationCoefficient())
            {
                myPreviousDeviationCoefficient = DeviationCoefficient();
            }
        }

        bool myHasOwnDrawHiddenLine;
        bool myDrawHiddenLine;

        //! Returns Standard_True if the hidden lines are to be drawn.
        //! By default the hidden lines are not drawn.
        public bool DrawHiddenLine()
        {
            return myHasOwnDrawHiddenLine || myLink == null
                 ? myDrawHiddenLine
                 : myLink.DrawHiddenLine();
        }
    }
    public class Prs3d_LineAspect : Prs3d_BasicAspect
    {
        //! Returns the line aspect. This is defined as the set of
        //! color, type and thickness attributes.
        public Graphic3d_AspectLine3d Aspect() { return myAspect; }
        public Graphic3d_AspectLine3d myAspect;
        public Prs3d_LineAspect(Quantity_Color theColor,
                                     Aspect_TypeOfLine theType,
                                     double theWidth)

        {
            myAspect = new Graphic3d_AspectLine3d(theColor, theType, theWidth);
        }


    }
    //! All basic Prs3d_xxxAspect must inherits from this class
    //! The aspect classes qualifies how to represent a given kind of object.
    public class Prs3d_BasicAspect
    {
    }
}

