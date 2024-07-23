using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OCCPort.Tester
{
	//! Auxiliary procedures to prepare Shaded presentation of specified shape.
	public class StdPrs_ShadedShape : Prs3d_Root
	{
		public static void Add(Prs3d_Presentation thePrs,
							   TopoDS_Shape theShape,
							   Prs3d_Drawer theDrawer,
							  bool theHasTexels,
							  gp_Pnt2d theUVOrigin,
							  gp_Pnt2d theUVRepeat,
							  gp_Pnt2d theUVScale,
							  StdPrs_Volume theVolume = StdPrs_Volume.StdPrs_Volume_Autodetection)
		{
			if (theShape.IsNull())
			{
				return;
			}

			// Use automatic re-triangulation with deflection-check logic only if this feature is enable
			if (theDrawer.IsAutoTriangulation())
			{
				// Triangulation completeness is important for "open-closed" analysis - perform tessellation beforehand
				StdPrs_ToolTriangulatedShape.Tessellate(theShape, theDrawer);
			}

			// add wireframe presentation for isolated edges and vertices
			wireframeFromShape(thePrs, theShape, theDrawer);

			// add special wireframe presentation for faces without triangulation
			//   wireframeNoTriangFacesFromShape(thePrs, theShape, theDrawer);

			// The shape types listed below need advanced analysis as potentially containing
			// both closed and open parts. Solids are also included, because they might
			// contain non-manifold parts inside (internal open shells)
			//if ((theShape.ShapeType() == TopAbs_COMPOUND
			//  || theShape.ShapeType() == TopAbs_COMPSOLID
			//  || theShape.ShapeType() == TopAbs_SOLID)
			// && theVolume == StdPrs_Volume_Autodetection)
			//{
			//    // collect two compounds: for opened and closed (solid) sub-shapes
			//    TopoDS_Compound anOpened, aClosed;
			//    BRep_Builder aBuilder;
			//    aBuilder.MakeCompound(aClosed);
			//    aBuilder.MakeCompound(anOpened);
			//    ExploreSolids(theShape, aBuilder, aClosed, anOpened, Standard_True);

			//    if (aClosed.NbChildren() > 0)
			//    {
			//        shadeFromShape(aClosed, thePrs, theDrawer,
			//                        theHasTexels, theUVOrigin, theUVRepeat, theUVScale, true);
			//    }

			//    if (anOpened.NbChildren() > 0)
			//    {
			//        shadeFromShape(anOpened, thePrs, theDrawer,
			//                        theHasTexels, theUVOrigin, theUVRepeat, theUVScale, false);
			//    }
			//}
			//else
			//{
			//    // if the shape type is not compound, composolid or solid, use autodetection back-facing filled
			//    shadeFromShape(theShape, thePrs, theDrawer,
			//                    theHasTexels, theUVOrigin, theUVRepeat, theUVScale,
			//                    theVolume == StdPrs_Volume_Closed);
			//}

			// if (theDrawer.FaceBoundaryDraw())
			{
				Graphic3d_ArrayOfSegments aBndSegments = fillFaceBoundaries(theShape, theDrawer.FaceBoundaryUpperContinuity());
				if (aBndSegments != null)
				{
					Graphic3d_Group aPrsGrp = thePrs.NewGroup();
					//aPrsGrp.SetGroupPrimitivesAspect(theDrawer.FaceBoundaryAspect().Aspect());
					aPrsGrp.AddPrimitiveArray(aBndSegments);
				}
			}
		}


		//! Computes wireframe presentation for free wires and vertices
		public static void wireframeFromShape(Prs3d_Presentation thePrs,
						TopoDS_Shape theShape,
						Prs3d_Drawer theDrawer)
		{
			TopExp_Explorer aShapeIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE);
			if (!aShapeIter.More())
			{
				StdPrs_WFShape.Add(thePrs, theShape, theDrawer);
				return;
			}
			bool aDrawAllVerticesFlag = (theDrawer.VertexDrawMode() == Prs3d_VertexDrawMode.Prs3d_VDM_All);
			if (!aDrawAllVerticesFlag && theShape.ShapeType() != TopAbs_ShapeEnum.TopAbs_COMPOUND)
			{
				return;
			}

			// We have to create a compound and collect all subshapes not drawn by the shading algo.
			// This includes:
			// - isolated edges
			// - isolated vertices, if aDrawAllVerticesFlag == Standard_False
			// - all shape's vertices, if aDrawAllVerticesFlag == Standard_True
			TopoDS_Compound aCompoundWF = new TopoDS_Compound();
			BRep_Builder aBuilder = new BRep_Builder();
			aBuilder.MakeCompound(aCompoundWF);
			bool hasElement = false;

			// isolated edges
			for (aShapeIter.Init(theShape, TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE); aShapeIter.More(); aShapeIter.Next())
			{
				hasElement = true;
				aBuilder.Add(aCompoundWF, aShapeIter.Current());
			}
			// isolated or all vertices
			aShapeIter.Init(theShape, TopAbs_ShapeEnum.TopAbs_VERTEX, aDrawAllVerticesFlag ? TopAbs_ShapeEnum.TopAbs_SHAPE : TopAbs_ShapeEnum.TopAbs_EDGE);
			for (; aShapeIter.More(); aShapeIter.Next())
			{
				hasElement = true;
				aBuilder.Add(aCompoundWF, aShapeIter.Current());
			}
			if (hasElement)
			{
				StdPrs_WFShape.Add(thePrs, aCompoundWF, theDrawer);
			}
		}

		//! Compute boundary presentation for faces of the shape.
		static Graphic3d_ArrayOfSegments fillFaceBoundaries(TopoDS_Shape theShape,
															   GeomAbs_Shape theUpperContinuity)
		{
			// collection of all triangulation nodes on edges
			// for computing boundaries presentation
			int aNodeNumber = 0;
			int aNbPolylines = 0;

			TopLoc_Location aTrsf = new TopLoc_Location();

			TColgp_SequenceOfPnt aSeqPntsExtra = null;
			for (TopExp_Explorer aFaceIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceIter.More(); aFaceIter.Next())
			{
				TopoDS_Face aFace = TopoDS.Face(aFaceIter.Current());
				if (aFace.NbChildren() == 0)
				{
					// handle specifically faces without boundary definition (triangulation-only)
					if (aSeqPntsExtra == null)
					{
						//Handle(NCollection_IncAllocator) anIncAlloc = new NCollection_IncAllocator();
						aSeqPntsExtra = new TColgp_SequenceOfPnt();
					}
					StdPrs_WFShape.AddEdgesOnTriangulation(aSeqPntsExtra, aFace, false);
				}
			}

			// explore all boundary edges
			TopTools_IndexedDataMapOfShapeListOfShape anEdgesMap = new TopTools_IndexedDataMapOfShapeListOfShape();
			TopExp.MapShapesAndAncestors(theShape, TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE, anEdgesMap);
			foreach (var item in anEdgesMap.items)

			//  for (TopTools_IndexedDataMapOfShapeListOfShape::Iterator anEdgeIter (anEdgesMap); anEdgeIter.More(); anEdgeIter.Next())
			{
				var anEdgeIter = item;
				// reject free edges
				if (anEdgeIter.list.Extent() == 0)
				{
					continue;
				}

				// take one of the shared edges and get edge triangulation
				TopoDS_Face aFace = TopoDS.Face(anEdgeIter.list.First());
				Poly_Triangulation aTriangulation = BRep_Tool.Triangulation(aFace, ref aTrsf);
				if (aTriangulation == null)
				{
					continue;
				}

				TopoDS_Edge anEdge = TopoDS.Edge(anEdgeIter.shape);
				if (theUpperContinuity < GeomAbs_Shape.GeomAbs_CN
				 && anEdgeIter.list.Extent() >= 2
				 && BRep_Tool.MaxContinuity(anEdge) > theUpperContinuity)
				{
					continue;
				}

				Poly_PolygonOnTriangulation anEdgePoly = BRep_Tool.PolygonOnTriangulation(anEdge, aTriangulation, aTrsf);
				if (anEdgePoly != null
				  && anEdgePoly.Nodes().Length() >= 2)
				{
					aNodeNumber += anEdgePoly.Nodes().Length();
					++aNbPolylines;
				}
			}
			Graphic3d_ArrayOfSegments aSegments = null;

			int aNbExtra = aSeqPntsExtra != null ? aSeqPntsExtra.Size() : 0;
			if (aNodeNumber == 0)
			{
				if (aNbExtra < 2)
				{
					return null;
				}

				aSegments = new Graphic3d_ArrayOfSegments(aNbExtra);
				foreach (var aPntIter in aSeqPntsExtra.list)

				//for (TColgp_SequenceOfPnt::Iterator aPntIter (*aSeqPntsExtra); aPntIter.More(); aPntIter.Next())
				{
					aSegments.AddVertex(aPntIter);
				}
				return aSegments;
			}

			// create indexed segments array to pack polylines from different edges into single array
			int aSegmentEdgeNb = (aNodeNumber - aNbPolylines) * 2;

			aSegments = new Graphic3d_ArrayOfSegments(aNodeNumber + aNbExtra, aSegmentEdgeNb + aNbExtra);
			foreach (var anEdgeIter in anEdgesMap.items)
			{


				//for (TopTools_IndexedDataMapOfShapeListOfShape::Iterator anEdgeIter (anEdgesMap); anEdgeIter.More(); anEdgeIter.Next())

				if (anEdgeIter.list.Extent() == 0)
				{
					continue;
				}

				TopoDS_Face aFace = TopoDS.Face(anEdgeIter.list.First());
				Poly_Triangulation aTriangulation = BRep_Tool.Triangulation(aFace, ref aTrsf);
				if (aTriangulation == null)
				{
					continue;
				}

				TopoDS_Edge anEdge = TopoDS.Edge(anEdgeIter.shape);
				if (theUpperContinuity < GeomAbs_Shape.GeomAbs_CN
				 && anEdgeIter.list.Extent() >= 2
				 && BRep_Tool.MaxContinuity(anEdge) > theUpperContinuity)
				{
					continue;
				}

				Poly_PolygonOnTriangulation anEdgePoly = BRep_Tool.PolygonOnTriangulation(anEdge, aTriangulation, aTrsf);
				if (anEdgePoly == null
				 || anEdgePoly.Nodes().Length() < 2)
				{
					continue;
				}

				// get edge nodes indexes from face triangulation
				TColStd_Array1OfInteger anEdgeNodes = anEdgePoly.Nodes();

				// collect the edge nodes
				int aSegmentEdge = aSegments.VertexNumber() + 1;
				for (int aNodeIdx = anEdgeNodes.Lower(); aNodeIdx <= anEdgeNodes.Upper(); ++aNodeIdx)
				{
					// node index in face triangulation
					// get node and apply location transformation to the node
					int aTriIndex = anEdgeNodes.Value(aNodeIdx);
					gp_Pnt aTriNode = aTriangulation.Node(aTriIndex);
					if (!aTrsf.IsIdentity())
					{
						aTriNode.Transform(aTrsf);
					}

					aSegments.AddVertex(aTriNode);
					if (aNodeIdx != anEdgeNodes.Lower())
					{
						aSegments.AddEdge(aSegmentEdge);
						aSegments.AddEdge(++aSegmentEdge);
					}
				}
			}

			if (aSeqPntsExtra != null)
			{
				int aSegmentEdge = aSegments.VertexNumber();
				foreach (var aPntIter in aSeqPntsExtra.list)
				{
					//for (TColgp_SequenceOfPnt::Iterator aPntIter (*aSeqPntsExtra); aPntIter.More(); aPntIter.Next())
					//{
					aSegments.AddVertex(aPntIter);
					aSegments.AddEdge(++aSegmentEdge);
				}
			}

			return aSegments;
		}
	}
}