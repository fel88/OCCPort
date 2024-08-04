namespace OCCPort
{
    internal class BRepMesh_MeshAlgoFactory : IMeshTools_MeshAlgoFactory
    {
        public IMeshTools_MeshAlgo GetAlgo(GeomAbs_SurfaceType theSurfaceType, ref IMeshTools_Parameters theParameters)
        {
            var algo1 = new BRepMesh_DelaunayNodeInsertionMeshAlgo();
            var algo2 = new BRepMesh_DelabellaBaseMeshAlgo();
            var algo3 = new BRepMesh_DelaunayDeflectionControlMeshAlgo();
            switch (theSurfaceType)
            {
                /**
                 *   struct DeflectionControlMeshAlgo
  {
    typedef BRepMesh_DelaunayDeflectionControlMeshAlgo<RangeSplitter, BRepMesh_DelaunayBaseMeshAlgo> Type;
  };
                 */
                case GeomAbs_SurfaceType.GeomAbs_Plane:
                    return theParameters.EnableControlSurfaceDeflectionAllSurfaces ?
                        algo3 : theParameters.InternalVerticesMode ?
                        algo1 : algo2;
                    /* new DeflectionControlMeshAlgo<BRepMesh_DefaultRangeSplitter>::Type :
                       (theParameters.InternalVerticesMode ?
                        new NodeInsertionMeshAlgo<BRepMesh_DefaultRangeSplitter>::Type :
                        new BaseMeshAlgo::Type);*/

                    break;
            }
            return null;
        }
    }


    //! Extends node insertion Delaunay meshing algo in order to control 
    //! deflection of generated trianges. Splits triangles failing the check.
    //template<class RangeSplitter, class BaseAlgo>
    class BRepMesh_DelaunayDeflectionControlMeshAlgo : BRepMesh_DelaunayNodeInsertionMeshAlgo//<RangeSplitter, BaseAlgo>
    {
    }
}