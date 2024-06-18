using System;

namespace OCCPort.Tester
{
	//! Tool for computing wireframe presentation of a TopoDS_Shape.
	class StdPrs_WFShape : Prs3d_Root

	{


		public static void Add(Prs3d_Presentation thePresentation,
							  TopoDS_Shape theShape,
							  Prs3d_Drawer theDrawer,
							  bool theIsParallel = false)
		{
			if (theShape.IsNull())
			{
				return;
			}

			if (theDrawer.IsAutoTriangulation())
			{
				StdPrs_ToolTriangulatedShape.Tessellate(theShape, theDrawer);
			}

			// draw triangulation-only edges
			Graphic3d_ArrayOfPrimitives aTriFreeEdges = AddEdgesOnTriangulation(theShape, true);
			if (aTriFreeEdges != null)
			{
				Graphic3d_Group aGroup = thePresentation.NewGroup();
				aGroup.SetPrimitivesAspect(theDrawer.FreeBoundaryAspect().Aspect());
				aGroup.AddPrimitiveArray(aTriFreeEdges);
			}

		}
		public void AddEdgesOnTriangulation(TColgp_SequenceOfPnt theSegments,

											  TopoDS_Shape theShape,
											  bool theToExcludeGeometric)
		{
			TopLoc_Location aLocation, aDummyLoc;
			for (TopExp_Explorer aFaceIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceIter.More(); aFaceIter.Next())
			{
				TopoDS_Face aFace = TopoDS.Face(aFaceIter.Current());
				if (theToExcludeGeometric)
				{
					Geom_Surface aSurf = BRep_Tool.Surface(aFace, aDummyLoc);
					if (!aSurf.IsNull())
					{
						continue;
					}
				}
				Poly_Triangulation aPolyTri = BRep_Tool.Triangulation(aFace, aLocation);
				if (aPolyTri != null)
				{
					Prs3d.AddFreeEdges(theSegments, aPolyTri, aLocation);
				}
			}
		}

		public Graphic3d_ArrayOfPrimitives AddEdgesOnTriangulation(TopoDS_Shape theShape,
																			 bool theToExcludeGeometric)
		{
			TColgp_SequenceOfPnt aSeqPnts = new TColgp_SequenceOfPnt();
			AddEdgesOnTriangulation(aSeqPnts, theShape, theToExcludeGeometric);
			if (aSeqPnts.Size() < 2)
			{
				return new Graphic3d_ArrayOfSegments();
			}

			int aNbVertices = aSeqPnts.Size();
			Graphic3d_ArrayOfSegments aSurfArray = new Graphic3d_ArrayOfSegments(aNbVertices);
			for (int anI = 1; anI <= aNbVertices; anI += 2)
			{
				aSurfArray.AddVertex(aSeqPnts.Value(anI));
				aSurfArray.AddVertex(aSeqPnts.Value(anI + 1));
			}
			return aSurfArray;
		}

	}
}