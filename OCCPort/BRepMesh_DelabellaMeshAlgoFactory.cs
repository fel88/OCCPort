using System.Runtime.InteropServices;

namespace OCCPort
{
    internal class BRepMesh_DelabellaMeshAlgoFactory : IMeshTools_MeshAlgoFactory
    {
        public IMeshTools_MeshAlgo GetAlgo(GeomAbs_SurfaceType theSurfaceType,
            ref IMeshTools_Parameters theParameters)
        {
            //=======================================================================
            // Function: GetAlgo
            // Purpose :
            //=======================================================================
            {
                var algo1 = new BRepMesh_DelaunayNodeInsertionMeshAlgo();
                var algo2 = new BRepMesh_DelabellaBaseMeshAlgo();
                switch (theSurfaceType)
                {

                    /*  struct BaseMeshAlgo
  {
    typedef BRepMesh_DelabellaBaseMeshAlgo Type;
  };*/
                    /*
  template<class RangeSplitter>
  struct NodeInsertionMeshAlgo
  {
    typedef BRepMesh_DelaunayNodeInsertionMeshAlgo<RangeSplitter, BRepMesh_CustomDelaunayBaseMeshAlgo<BRepMesh_DelabellaBaseMeshAlgo> > Type;
  };*/

                    case GeomAbs_SurfaceType.GeomAbs_Plane:
                        return theParameters.InternalVerticesMode ?
                          //new NodeInsertionMeshAlgo<BRepMesh_DefaultRangeSplitter>::Type :
                          algo1 :
                          //new BaseMeshAlgo::Type;
                          algo2;
                        break;

                    //case GeomAbs_Sphere:
                    //    {
                    //        NodeInsertionMeshAlgo<BRepMesh_SphereRangeSplitter>::Type* aMeshAlgo =
                    //          new NodeInsertionMeshAlgo<BRepMesh_SphereRangeSplitter>::Type;
                    //        aMeshAlgo->SetPreProcessSurfaceNodes(Standard_True);
                    //        return aMeshAlgo;
                    //    }
                    //    break;

                    //case GeomAbs_Cylinder:
                    //    return theParameters.InternalVerticesMode ?
                    //      new DefaultNodeInsertionMeshAlgo<BRepMesh_CylinderRangeSplitter>::Type :
                    //      new DefaultBaseMeshAlgo::Type;
                    //    break;

                    //case GeomAbs_Cone:
                    //    {
                    //        NodeInsertionMeshAlgo<BRepMesh_ConeRangeSplitter>::Type* aMeshAlgo =
                    //          new NodeInsertionMeshAlgo<BRepMesh_ConeRangeSplitter>::Type;
                    //        aMeshAlgo->SetPreProcessSurfaceNodes(Standard_True);
                    //        return aMeshAlgo;
                    //    }
                    //    break;

                    //case GeomAbs_Torus:
                    //    {
                    //        NodeInsertionMeshAlgo<BRepMesh_TorusRangeSplitter>::Type* aMeshAlgo =
                    //          new NodeInsertionMeshAlgo<BRepMesh_TorusRangeSplitter>::Type;
                    //        aMeshAlgo->SetPreProcessSurfaceNodes(Standard_True);
                    //        return aMeshAlgo;
                    //    }
                    //    break;

                    //case GeomAbs_SurfaceOfRevolution:
                    //    {
                    //        DeflectionControlMeshAlgo<BRepMesh_BoundaryParamsRangeSplitter>::Type* aMeshAlgo =
                    //          new DeflectionControlMeshAlgo<BRepMesh_BoundaryParamsRangeSplitter>::Type;
                    //        aMeshAlgo->SetPreProcessSurfaceNodes(Standard_True);
                    //        return aMeshAlgo;
                    //    }
                    //    break;

                    default:
                        {
                            /*DeflectionControlMeshAlgo<BRepMesh_NURBSRangeSplitter>::Type* aMeshAlgo =
                              new DeflectionControlMeshAlgo<BRepMesh_NURBSRangeSplitter>::Type;
                            aMeshAlgo->SetPreProcessSurfaceNodes(Standard_True);
                            return aMeshAlgo;*/
                            return null;
                        }
                }
            }
        }
    }
}