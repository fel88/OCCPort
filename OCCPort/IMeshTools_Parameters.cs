namespace OCCPort
{
	public interface IMeshTools_Parameters
	{  //! 2D Delaunay triangulation algorithm factory to use
		IMeshTools_MeshAlgoType MeshAlgo { get; set; }
	}
}