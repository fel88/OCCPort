namespace OCCPort
{
	//! Enumerates built-in meshing algorithms factories implementing IMeshTools_MeshAlgoFactory interface.
	public enum IMeshTools_MeshAlgoType
	{
		IMeshTools_MeshAlgoType_DEFAULT = -1, //!< use global default (IMeshTools_MeshAlgoType_Watson or CSF_MeshAlgo)
		IMeshTools_MeshAlgoType_Watson = 0,  //!< generate 2D Delaunay triangulation based on Watson algorithm (BRepMesh_MeshAlgoFactory)
		IMeshTools_MeshAlgoType_Delabella,    //!< generate 2D Delaunay triangulation based on Delabella algorithm (BRepMesh_DelabellaMeshAlgoFactory)
	};
}