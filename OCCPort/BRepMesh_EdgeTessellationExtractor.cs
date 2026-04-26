using OCCPort.Interfaces;

namespace OCCPort
{
    //! Auxiliary class implements functionality retrieving tessellated
    //! representation of an edge stored in polygon.
    public class BRepMesh_EdgeTessellationExtractor : IMeshTools_CurveTessellator
    {
        public BRepMesh_EdgeTessellationExtractor(IMeshData_Edge theEdge, IMeshData_Face theFace)
        {
            Poly_Triangulation aTriangulation =
    BRep_Tool.Triangulation(theFace.GetFace(), ref myLoc);

            Poly_PolygonOnTriangulation aPolygon =
              BRep_Tool.PolygonOnTriangulation(theEdge.GetEdge(), aTriangulation, myLoc);

            myTriangulation = aTriangulation;
            myIndices = aPolygon.Nodes();
            myProvider.Init(theEdge, TopAbs_Orientation.TopAbs_FORWARD, theFace, aPolygon.Parameters());
        }

        BRepMesh_EdgeParameterProvider myProvider;

        public bool Value(int theIndex, out gp_Pnt thePoint, out double theParameter)
        {
            gp_Pnt aRefPnt = myTriangulation.Node(myIndices.Value(theIndex));
            thePoint = BRepMesh_ShapeTool.UseLocation(aRefPnt, myLoc);

            theParameter = myProvider.Parameter(theIndex, thePoint);
            return true;
        }

        TColStd_Array1OfInteger myIndices;
        TopLoc_Location myLoc;
        Poly_Triangulation myTriangulation;

        public int PointsNb()
        {
            return myIndices.Length();
        }
    }

}