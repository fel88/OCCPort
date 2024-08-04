namespace OCCPort
{
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