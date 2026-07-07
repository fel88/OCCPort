using TKernel;

namespace TKMesh
{
    //! Class provides base functionality to build face triangulation using Dealunay approach.
    //! Performs generation of mesh using raw data from model.
    public class BRepMesh_DelaunayBaseMeshAlgo : BRepMesh_ConstrainedBaseMeshAlgo

    {
        public override void generateMesh(Message_ProgressRange theRange)
        {
            BRepMesh_DataStructureOfDelaun aStructure = getStructure();
            VectorOfPnt aNodesMap = getNodesMap();

            VectorOfInteger aVerticesOrder = new VectorOfInteger(aNodesMap.Size());
            for (int i = 1; i <= aNodesMap.Size(); ++i)
            {
                aVerticesOrder.Append(i);
            }

            var aCellsCount = getCellsCount(aVerticesOrder.Size());
            BRepMesh_Delaun aMesher = new BRepMesh_Delaun(aStructure, aVerticesOrder, aCellsCount.Item1, aCellsCount.Item2);
            BRepMesh_MeshTool aCleaner = new BRepMesh_MeshTool(aStructure);
            aCleaner.EraseFreeLinks();

            if (!theRange.More())
            {
                return;
            }
            postProcessMesh(aMesher, theRange);
        }
    }
}

