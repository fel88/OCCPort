using OCCPort.Tester;
using System.Diagnostics;

namespace OCCPort
{
	//! Explores TopoDS_Shape for parts to be meshed - faces and free edges.
	public class IMeshTools_ShapeExplorer : IMeshData_Shape
	{//=======================================================================
	 // Function: Constructor
	 // Purpose : 
	 //=======================================================================
		public IMeshTools_ShapeExplorer(TopoDS_Shape theShape)
			: base(theShape)
		{
		}

		//=======================================================================
		// Function: visitEdges
		// Purpose : Explodes the given shape on edges according to the specified
		//           criteria and visits each one in order to add it to data model.
		//=======================================================================
		void visitEdges(IMeshTools_ShapeVisitor theVisitor,
				   TopoDS_Shape theShape,
				   bool isResetLocation,
				   TopAbs_ShapeEnum theToFind,
				   TopAbs_ShapeEnum theToAvoid = TopAbs_ShapeEnum.TopAbs_SHAPE)
		{

			TopExp_Explorer aEdgesIt = new TopExp_Explorer(theShape, theToFind, theToAvoid);
			for (; aEdgesIt.More(); aEdgesIt.Next())
			{
				TopoDS_Edge aEdge = TopoDS.Edge(aEdgesIt.Current());
				if (!BRep_Tool.IsGeometric(aEdge))
				{
					continue;
				}

				//theVisitor.Visit(isResetLocation ?
				//  TopoDS.Edge(aEdge.Located(new TopLoc_Location())) :
				//  aEdge);
			}
		}

		//=======================================================================
		// Function: Accept
		// Purpose : 
		//=======================================================================
		public void Accept(IMeshTools_ShapeVisitor theVisitor)
		{
			// Explore all free edges in shape.
			visitEdges(theVisitor, GetShape(), true, TopAbs_ShapeEnum. TopAbs_EDGE,TopAbs_ShapeEnum. TopAbs_FACE);

			// Explore all related to some face edges in shape.
			// make array of faces suitable for processing (excluding faces without surface)
			TopTools_ListOfShape aFaceList;
			//BRepLib.ReverseSortFaces(GetShape(), aFaceList);
			//TopTools_MapOfShape aFaceMap;

			//TopLoc_Location aEmptyLoc;
			//TopTools_ListIteratorOfListOfShape aFaceIter(aFaceList);
			//for (; aFaceIter.More(); aFaceIter.Next())
			//{
			//	TopoDS_Shape aFaceNoLoc = aFaceIter.Value();
			//	aFaceNoLoc.Location(aEmptyLoc);
			//	if (!aFaceMap.Add(aFaceNoLoc))
			//	{
			//		continue; // already processed
			//	}

			//	TopoDS_Face aFace = TopoDS.Face(aFaceIter.Value());
			//	if (!BRep_Tool.IsGeometric(aFace))
			//	{
			//		continue;
			//	}

			//	// Explore all edges in face.
			//	visitEdges(theVisitor, aFace, false, TopAbs_ShapeEnum. TopAbs_EDGE);

			//	// Store only forward faces in order to prevent inverse issue.
			//	theVisitor->Visit(TopoDS::Face(aFace.Oriented(TopAbs_FORWARD)));
			//}
		}
	}


}