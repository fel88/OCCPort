namespace OCCPort
{
	//! Builds discrete model of a shape by adding faces and free edges.
	//! Computes deflection for corresponded shape and checks whether it
	//! fits existing polygonal representation. If not, cleans shape from
	//! outdated info.
	public class BRepMesh_ShapeVisitor : IMeshTools_ShapeVisitor
	{
		IMeshData_Model myModel;
		//IMeshData::DMapOfShapeInteger myDEdgeMap;
		//=======================================================================
		// Function: Constructor
		// Purpose : 
		//=======================================================================
		public BRepMesh_ShapeVisitor(IMeshData_Model theModel)

		{
			myModel = (theModel);
			//myDEdgeMap(1, new NCollection_IncAllocator(IMeshData::MEMORY_BLOCK_SIZE_HUGE))
		}
	}
}