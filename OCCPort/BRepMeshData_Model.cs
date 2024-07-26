﻿namespace OCCPort
{
	//! Default implementation of model entity.
	public class BRepMeshData_Model : IMeshData_Model
	{
		public BRepMeshData_Model(TopoDS_Shape theShape) : base(theShape)
		{
			myMaxSize = 0.0;
			//myAllocator(new NCollection_IncAllocator(IMeshData::MEMORY_BLOCK_SIZE_HUGE)),
			//myDFaces(256, myAllocator),
			//myDEdges(256, myAllocator)

			//myAllocator->SetThreadSafe();
		}

		//=======================================================================
		// Function: FacesNb
		// Purpose : 
		//=======================================================================
		public override int FacesNb()
		{
			return myDFaces.Size();
		}

		IMeshData.VectorOfIFaceHandles myDFaces;

		//! Sets maximum size of shape's bounding box.
		public void SetMaxSize(double theValue)
		{

			myMaxSize = theValue;
		}

		double myMaxSize;
		//Handle(NCollection_IncAllocator) myAllocator;
		//IMeshData::VectorOfIFaceHandles myDFaces;
		//IMeshData::VectorOfIEdgeHandles myDEdges;
	}

}