using System;

namespace OCCPort
{
    public class Prs3d_Drawer: Graphic3d_PresentationAttributes
    {
        

    internal bool IsAutoTriangulation()
        {

			return myHasOwnIsAutoTriangulated || myLink == null
				 ? myIsAutoTriangulated
				 : myLink.IsAutoTriangulation();

        }

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

		
    }
}