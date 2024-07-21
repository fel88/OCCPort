namespace OCCPort.Tester
{
    public class Poly_PolygonOnTriangulation
    {
        //! Returns the number of nodes for this polygon.
        //! Note: If the polygon is closed, the point of closure is
        //! repeated at the end of its table of nodes. Thus, on a closed
        //! triangle, the function NbNodes returns 4.
        public int NbNodes() { return myNodes.Length(); }
        TColStd_Array1OfInteger myNodes;

        //! Returns the table of nodes for this polygon.
        //! A node value is an index in the table of nodes specific to an existing triangulation of a shape.
        public TColStd_Array1OfInteger Nodes() { return myNodes; }

    }
}