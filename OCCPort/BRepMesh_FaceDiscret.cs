namespace OCCPort
{
	//! Class implements functionality starting triangulation of model's faces.
	//! Each face is processed separately and can be executed in parallel mode.
	//! Uses mesh algo factory passed as initializer to create instance of triangulation 
	//! algorithm according to type of surface of target face.
	public class BRepMesh_FaceDiscret : IMeshTools_ModelAlgo
	{
		public BRepMesh_FaceDiscret(IMeshTools_MeshAlgoFactory theAlgoFactory)
		{
			myAlgoFactory = (theAlgoFactory);
		}

		IMeshTools_MeshAlgoFactory myAlgoFactory;
	}


}