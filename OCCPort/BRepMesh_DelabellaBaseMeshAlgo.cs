namespace OCCPort
{
    //! Class provides base functionality to build face triangulation using Delabella project.
    //! Performs generation of mesh using raw data from model.
    public class BRepMesh_DelabellaBaseMeshAlgo : BRepMesh_CustomBaseMeshAlgo
    {
        public override void buildBaseTriangulation()
        {
            BRepMesh_DataStructureOfDelaun aStructure = this.getStructure();

            //Bnd_B2d aBox = new Bnd_B2d();
            int aNodesNb = aStructure.NbNodes();
            //List<Standard_Real> aPoints(2 * (aNodesNb + 4));
            double[] aPoints = new double[(2 * (aNodesNb + 4))];
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

            IDelaBella aTriangulator = IDelaBella.Create();
            if (aTriangulator == null) // should never happen
            {
                throw new Standard_ProgramError("BRepMesh_DelabellaBaseMeshAlgo::buildBaseTriangulation: unable creating a triangulation algorithm");
            }

            //          aTriangulator->SetErrLog(logDelabella2Occ, NULL);
            //          try
            //          {
            int aVerticesNb = aTriangulator.Triangulate(
             (int)(aPoints.Length / 2),
             aPoints, 0, 1, 2 * sizeof(double));

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
}