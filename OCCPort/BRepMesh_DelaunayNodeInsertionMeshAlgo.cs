namespace OCCPort
{
    //! Extends base Delaunay meshing algo in order to enable possibility 
    //! of addition of free vertices and internal nodes into the mesh.

    public class BRepMesh_DelaunayNodeInsertionMeshAlgo : BRepMesh_NodeInsertionMeshAlgo
    {
        //! Returns size of cell to be used by acceleration circles grid structure.
        public override (int, int) getCellsCount(int theVerticesNb)
        {
            return BRepMesh_GeomTool.CellsCount(getDFace().GetSurface(), theVerticesNb,
                                                  getDFace().GetDeflection(),
                                                  getRangeSplitter());
        }

    }

}