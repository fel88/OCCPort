using System;

namespace OCCPort
{
	internal class BRepMesh_ModelBuilder : IMeshTools_ModelBuilder
	{
		//=======================================================================
		// Function: Perform
		// Purpose : 
		//=======================================================================
		public override IMeshData_Model performInternal(
  TopoDS_Shape theShape,
  IMeshTools_Parameters theParameters)
		{
			BRepMeshData_Model aModel = null;

			Bnd_Box aBox;
			//BRepBndLib.Add(theShape, aBox, false);

			//if (!aBox.IsVoid())
			//{
			//	// Build data model for further processing.
			//	aModel = new BRepMeshData_Model(theShape);

			//	if (theParameters.Relative)
			//	{
			//		double aMaxSize;
			//		BRepMesh_ShapeTool.BoxMaxDimension(aBox, aMaxSize);
			//		aModel.SetMaxSize(aMaxSize);
			//	}
			//	else
			//	{
			//		aModel.SetMaxSize(Math.Max(theParameters.Deflection,
			//							   theParameters.DeflectionInterior));
			//	}

			//	IMeshTools_ShapeVisitor aVisitor =
			//	  new BRepMesh_ShapeVisitor(aModel);

			//	IMeshTools_ShapeExplorer aExplorer = new IMeshTools_ShapeExplorer(theShape);
			//	aExplorer.Accept(aVisitor);
			//	SetStatus(Message_Status.Message_Done1);
			//}
			//else
			//{
			//	SetStatus(Message_Status.Message_Fail1);
			//}

			return aModel;
		}

		//! Sets maximum size of shape's bounding box.
		public void SetMaxSize(double theValue)
		{
			myMaxSize = theValue;
		}

		double myMaxSize;
		/*Handle(NCollection_IncAllocator) myAllocator;
  IMeshData::VectorOfIFaceHandles myDFaces;
		IMeshData::VectorOfIEdgeHandles myDEdges;*/
	}


}