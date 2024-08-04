namespace OCCPort
{
	//! Class implementing default context of BRepMesh algorithm.
	//! Initializes context by default algorithms.
	public class BRepMesh_Context : IMeshTools_Context
	{ //! Sets instance of a tool to be used to build discrete model.
		void SetModelBuilder(IMeshTools_ModelBuilder theBuilder)
		{
			myModelBuilder = theBuilder;
		}

		public BRepMesh_Context(IMeshTools_MeshAlgoType theMeshType)
		{
			if (theMeshType == IMeshTools_MeshAlgoType.IMeshTools_MeshAlgoType_DEFAULT)
			{
				//string aValue = OSD_Environment("CSF_MeshAlgo").Value();
				string aValue = "Watson";
				aValue = aValue.ToLower();

				if (aValue == "watson"
				 || aValue == "0")
				{
					theMeshType = IMeshTools_MeshAlgoType.IMeshTools_MeshAlgoType_Watson;
				}
				else if (aValue == "delabella"
					  || aValue == "1")
				{
					theMeshType = IMeshTools_MeshAlgoType.IMeshTools_MeshAlgoType_Delabella;
				}

				else
				{
					if (!aValue.IsEmpty())
					{
						//Message::SendWarning(TCollection_AsciiString("BRepMesh_Context, ignore unknown algorithm '") + aValue + "' specified in CSF_MeshAlgo variable");
					}
					theMeshType = IMeshTools_MeshAlgoType.IMeshTools_MeshAlgoType_Watson;
				}
			}

			IMeshTools_MeshAlgoFactory aAlgoFactory = null;
			switch (theMeshType)
			{
				case IMeshTools_MeshAlgoType.IMeshTools_MeshAlgoType_DEFAULT:
				case IMeshTools_MeshAlgoType.IMeshTools_MeshAlgoType_Watson:
					aAlgoFactory = new BRepMesh_MeshAlgoFactory();
					break;
				case IMeshTools_MeshAlgoType.IMeshTools_MeshAlgoType_Delabella:
					aAlgoFactory = new BRepMesh_DelabellaMeshAlgoFactory();
					break;
			}

			SetModelBuilder(new BRepMesh_ModelBuilder());
			/*SetEdgeDiscret(new BRepMesh_EdgeDiscret);
			SetModelHealer(new BRepMesh_ModelHealer);
			SetPreProcessor(new BRepMesh_ModelPreProcessor);*/
			SetFaceDiscret(new BRepMesh_FaceDiscret(aAlgoFactory));
			//SetPostProcessor(new BRepMesh_ModelPostProcessor);
		}  //! Sets instance of meshing algorithm.
		
		

	}


}