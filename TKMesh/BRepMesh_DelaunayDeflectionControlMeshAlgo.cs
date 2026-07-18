namespace TKMesh
{
    //! Extends node insertion Delaunay meshing algo in order to control 
    //! deflection of generated trianges. Splits triangles failing the check.
    //template<class RangeSplitter, class BaseAlgo>
    class BRepMesh_DelaunayDeflectionControlMeshAlgo<RangeSplitter> : BRepMesh_DelaunayNodeInsertionMeshAlgo<RangeSplitter> where RangeSplitter : AbstractRangeSplitter, new()//<RangeSplitter, BaseAlgo>
    {
    }
}



