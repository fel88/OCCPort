using OCCPort.Interfaces;

namespace OCCPort
{
    //! Auxiliary class implements functionality retrieving tessellated
    //! representation of an edge stored in polygon.
    public class BRepMesh_EdgeTessellationExtractor : IMeshTools_CurveTessellator
    {
        public BRepMesh_EdgeTessellationExtractor(IMeshData_Edge theDEdge, IMeshData_Face theDFace)
        {
        }
    }

}