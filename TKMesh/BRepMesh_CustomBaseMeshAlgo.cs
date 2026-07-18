using TKernel;

namespace TKMesh
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

            (int, int) aCellsCount = getCellsCount(aStructure.NbNodes());
            BRepMesh_Delaun aMesher = new BRepMesh_Delaun(aStructure, aCellsCount.Item1, aCellsCount.Item2, false);

            int aNewNodesNb = aStructure.NbNodes();
            bool isRemoveAux = aNewNodesNb > aNodesNb;
            if (isRemoveAux)
            {
                VectorOfInteger aAuxVertices = new VectorOfInteger(aNewNodesNb - aNodesNb);
                for (int aExtNodesIt = aNodesNb + 1; aExtNodesIt <= aNewNodesNb; ++aExtNodesIt)
                {
                    aAuxVertices.Append(aExtNodesIt);
                }

                // Set aux vertices if there are some to clean up mesh correctly.
                aMesher.SetAuxVertices(aAuxVertices);
            }

            aMesher.ProcessConstraints();

            // Destruction of triangles containing aux vertices added (possibly) during base mesh computation.
            if (isRemoveAux)
            {
                aMesher.RemoveAuxElements();
            }

            BRepMesh_MeshTool aCleaner = new BRepMesh_MeshTool(aStructure);
            aCleaner.EraseFreeLinks();

            postProcessMesh(aMesher, theRange);
        }
    }
}



