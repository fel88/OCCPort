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


    //! Extends base Delaunay meshing algo in order to enable possibility 
    //! of addition of free vertices and internal nodes into the mesh.

    public class BRepMesh_DelaunayNodeInsertionMeshAlgo : BRepMesh_NodeInsertionMeshAlgo
    {
    }
    //! Extends base meshing algo in order to enable possibility 
    //! of addition of free vertices into the mesh.

    public class BRepMesh_NodeInsertionMeshAlgo : BRepMesh_DelabellaBaseMeshAlgo //: BaseAlgo
    {
    }


    //! Class provides base functionality to build face triangulation using custom
    //! triangulation algorithm with possibility to modify final mesh.
    //! Performs generation of mesh using raw data from model.

    public class BRepMesh_CustomDelaunayBaseMeshAlgo

    {
    }


    //! Default tool to define range of discrete face model and 
    //! obtain grid points distributed within this range.
    public class BRepMesh_DefaultRangeSplitter
    {
    }


    //! Class provides base functionality to build face triangulation using Delabella project.
    //! Performs generation of mesh using raw data from model.
    public class BRepMesh_DelabellaBaseMeshAlgo : BRepMesh_CustomBaseMeshAlgo
    {
        public override void buildBaseTriangulation()
        {
  //          const Handle(BRepMesh_DataStructureOfDelaun)&aStructure = this->getStructure();

  //          Bnd_B2d aBox;
  //          const Standard_Integer aNodesNb = aStructure->NbNodes();
  //          std::vector<Standard_Real> aPoints(2 * (aNodesNb + 4));
  //          for (Standard_Integer aNodeIt = 0; aNodeIt < aNodesNb; ++aNodeIt)
  //          {
  //              const BRepMesh_Vertex&aVertex = aStructure->GetNode(aNodeIt + 1);

  //              const size_t aBaseIdx = 2 * static_cast<size_t>(aNodeIt);
  //              aPoints[aBaseIdx + 0] = aVertex.Coord().X();
  //              aPoints[aBaseIdx + 1] = aVertex.Coord().Y();

  //              aBox.Add(gp_Pnt2d(aVertex.Coord()));
  //          }

  //          aBox.Enlarge(0.1 * (aBox.CornerMax() - aBox.CornerMin()).Modulus());
  //          const gp_XY aMin = aBox.CornerMin();
  //          const gp_XY aMax = aBox.CornerMax();

  //          aPoints[2 * aNodesNb + 0] = aMin.X();
  //          aPoints[2 * aNodesNb + 1] = aMin.Y();
  //          aStructure->AddNode(BRepMesh_Vertex(
  //            aPoints[2 * aNodesNb + 0],
  //            aPoints[2 * aNodesNb + 1], BRepMesh_Free));

  //          aPoints[2 * aNodesNb + 2] = aMax.X();
  //          aPoints[2 * aNodesNb + 3] = aMin.Y();
  //          aStructure->AddNode(BRepMesh_Vertex(
  //            aPoints[2 * aNodesNb + 2],
  //            aPoints[2 * aNodesNb + 3], BRepMesh_Free));

  //          aPoints[2 * aNodesNb + 4] = aMax.X();
  //          aPoints[2 * aNodesNb + 5] = aMax.Y();
  //          aStructure->AddNode(BRepMesh_Vertex(
  //            aPoints[2 * aNodesNb + 4],
  //            aPoints[2 * aNodesNb + 5], BRepMesh_Free));

  //          aPoints[2 * aNodesNb + 6] = aMin.X();
  //          aPoints[2 * aNodesNb + 7] = aMax.Y();
  //          aStructure->AddNode(BRepMesh_Vertex(
  //            aPoints[2 * aNodesNb + 6],
  //            aPoints[2 * aNodesNb + 7], BRepMesh_Free));

  //          const Standard_Real aDiffX = (aMax.X() - aMin.X());
  //          const Standard_Real aDiffY = (aMax.Y() - aMin.Y());
  //          for (size_t i = 0; i < aPoints.size(); i += 2)
  //          {
  //              aPoints[i + 0] = (aPoints[i + 0] - aMin.X()) / aDiffX - 0.5;
  //              aPoints[i + 1] = (aPoints[i + 1] - aMin.Y()) / aDiffY - 0.5;
  //          }

  //          IDelaBella* aTriangulator = IDelaBella::Create();
  //          if (aTriangulator == NULL) // should never happen
  //          {
  //              throw Standard_ProgramError("BRepMesh_DelabellaBaseMeshAlgo::buildBaseTriangulation: unable creating a triangulation algorithm");
  //          }

  //          aTriangulator->SetErrLog(logDelabella2Occ, NULL);
  //          try
  //          {
  //              const int aVerticesNb = aTriangulator->Triangulate(
  //                static_cast<int>(aPoints.size() / 2),
  //                &aPoints[0], &aPoints[1], 2 * sizeof(Standard_Real));

  //              if (aVerticesNb > 0)
  //              {
  //                  const DelaBella_Triangle* aTrianglePtr = aTriangulator->GetFirstDelaunayTriangle();
  //                  while (aTrianglePtr != NULL)
  //                  {
  //                      Standard_Integer aNodes[3] = {
  //        aTrianglePtr->v[0]->i + 1,
  //        aTrianglePtr->v[2]->i + 1,
  //        aTrianglePtr->v[1]->i + 1
  //      };

  //                      Standard_Integer aEdges[3];
  //                      Standard_Boolean aOrientations[3];
  //                      for (Standard_Integer k = 0; k < 3; ++k)
  //                      {
  //                          const BRepMesh_Edge aLink(aNodes[k], aNodes[(k + 1) % 3], BRepMesh_Free);

  //                          const Standard_Integer aLinkInfo = aStructure->AddLink(aLink);
  //                          aEdges[k] = Abs(aLinkInfo);
  //                          aOrientations[k] = aLinkInfo > 0;
  //                      }

  //                      const BRepMesh_Triangle aTriangle(aEdges, aOrientations, BRepMesh_Free);
  //                      aStructure->AddElement(aTriangle);

  //                      aTrianglePtr = aTrianglePtr->next;
  //                  }
  //              }

  //              aTriangulator->Destroy();
  //              aTriangulator = NULL;
  //          }
  //          catch (Standard_Failure const&theException)
  //{
  //              if (aTriangulator != NULL)
  //              {
  //                  aTriangulator->Destroy();
  //                  aTriangulator = NULL;
  //              }

  //              throw Standard_Failure(theException);
  //          }
  //catch (...)
  //{
  //              if (aTriangulator != NULL)
  //              {
  //                  aTriangulator->Destroy();
  //                  aTriangulator = NULL;
  //              }

  //              throw Standard_Failure("BRepMesh_DelabellaBaseMeshAlgo::buildBaseTriangulation: exception in triangulation algorithm");
            }
    }

    //! Class provides base functionality to build face triangulation using custom triangulation algorithm.
    //! Performs generation of mesh using raw data from model.
    public abstract class BRepMesh_CustomBaseMeshAlgo : BRepMesh_ConstrainedBaseMeshAlgo
    {

        //! Builds base triangulation using custom triangulation algorithm.
        public abstract void buildBaseTriangulation();

        //! Generates mesh for the contour stored in data structure.
        public override void generateMesh(Message_ProgressRange theRange)
        {
            BRepMesh_DataStructureOfDelaun aStructure = this.getStructure();
            int aNodesNb = aStructure.NbNodes();

            buildBaseTriangulation();

            //std::pair<Standard_Integer, Standard_Integer> aCellsCount = this->getCellsCount(aStructure->NbNodes());
            //BRepMesh_Delaun aMesher(aStructure, aCellsCount.first, aCellsCount.second, Standard_False);

            //const Standard_Integer aNewNodesNb = aStructure->NbNodes();
            //const Standard_Boolean isRemoveAux = aNewNodesNb > aNodesNb;
            //if (isRemoveAux)
            //{
            //    IMeshData::VectorOfInteger aAuxVertices(aNewNodesNb -aNodesNb);
            //    for (Standard_Integer aExtNodesIt = aNodesNb + 1; aExtNodesIt <= aNewNodesNb; ++aExtNodesIt)
            //    {
            //        aAuxVertices.Append(aExtNodesIt);
            //    }

            //    // Set aux vertices if there are some to clean up mesh correctly.
            //    aMesher.SetAuxVertices(aAuxVertices);
            //}

            //aMesher.ProcessConstraints();

            //// Destruction of triangles containing aux vertices added (possibly) during base mesh computation.
            //if (isRemoveAux)
            //{
            //    aMesher.RemoveAuxElements();
            //}

            //BRepMesh_MeshTool aCleaner(aStructure);
            //aCleaner.EraseFreeLinks();

            //postProcessMesh(aMesher, theRange);
        }
    }
}