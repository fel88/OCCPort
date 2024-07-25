namespace OCCPort
{
	internal class BRepMesh_ModelBuilder : IMeshTools_ModelBuilder
	{
		//=======================================================================
		// Function: Perform
		// Purpose : 
		//=======================================================================
		public IMeshData_Model performInternal(
  TopoDS_Shape theShape,
  IMeshTools_Parameters theParameters)
		{
			BRepMeshData_Model aModel = null;

			Bnd_Box aBox;
			BRepBndLib.Add(theShape, aBox, false);

			if (!aBox.IsVoid())
			{
				// Build data model for further processing.
				aModel = new BRepMeshData_Model(theShape);

				if (theParameters.Relative)
				{
					Standard_Real aMaxSize;
					BRepMesh_ShapeTool::BoxMaxDimension(aBox, aMaxSize);
					aModel->SetMaxSize(aMaxSize);
				}
				else
				{
					aModel->SetMaxSize(Max(theParameters.Deflection,
										   theParameters.DeflectionInterior));
				}

				Handle(IMeshTools_ShapeVisitor) aVisitor =
				  new BRepMesh_ShapeVisitor(aModel);

				IMeshTools_ShapeExplorer aExplorer(theShape);
				aExplorer.Accept(aVisitor);
				SetStatus(Message_Done1);
			}
			else
			{
				SetStatus(Message_Fail1);
			}

			return aModel;
		}
	}
}