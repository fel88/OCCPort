using System.Drawing;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace OCCPort
{
    //! Extends base Delaunay meshing algo in order to enable possibility 
    //! of addition of free vertices and internal nodes into the mesh.

    public class BRepMesh_DelaunayNodeInsertionMeshAlgo<RangeSplitter> : BRepMesh_NodeInsertionMeshAlgo<RangeSplitter> where RangeSplitter : AbstractRangeSplitter
    {
        //! Returns size of cell to be used by acceleration circles grid structure.
        public override (int, int) getCellsCount(int theVerticesNb)
        {
            return BRepMesh_GeomTool.CellsCount(getDFace().GetSurface(), theVerticesNb,
                                                  getDFace().GetDeflection(),
                                                  getRangeSplitter());
        }

        bool myIsPreProcessSurfaceNodes;

        //! Performs processing of generated mesh. Generates surface nodes and inserts them into structure.
        public override void postProcessMesh(BRepMesh_Delaun theMesher,
                                Message_ProgressRange theRange)
        {
            if (!theRange.More())
            {
                return;
            }
            base.postProcessMesh(theMesher, new Message_ProgressRange()); // shouldn't be range passed here?

            if (!myIsPreProcessSurfaceNodes)
            {
                ListOfPnt2d aSurfaceNodes =
                  this.getRangeSplitter().GenerateSurfaceNodes(getParameters());

                insertNodes(aSurfaceNodes, theMesher, theRange);
            }
        }

        //! Inserts nodes into mesh.
        bool insertNodes(
    ListOfPnt2d theNodes,
    BRepMesh_Delaun theMesher,
    Message_ProgressRange theRange)
        {
            if (theNodes == null || theNodes.IsEmpty())
            {
                return false;
            }

            VectorOfInteger aVertexIndexes = new VectorOfInteger(theNodes.Size());
            foreach (var aNodesIt in theNodes)
            {
                gp_Pnt2d aPnt2d = aNodesIt;
                //   if (this.getClassifier().Perform(aPnt2d) == TopAbs_State.TopAbs_IN)
                {
                    //   aVertexIndexes.Append(this.registerNode(this.getRangeSplitter().Point(aPnt2d),
                    //       aPnt2d, Enums.BRepMesh_DegreeOfFreedom.BRepMesh_Free,
                    //     false));
                }
            }

            theMesher.AddVertices(aVertexIndexes, theRange);
            if (!theRange.More())
            {
                return false;
            }
            return !aVertexIndexes.IsEmpty();
        }

    }

}