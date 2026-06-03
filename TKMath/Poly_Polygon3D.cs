namespace TKMath
{
    //! This class Provides a polygon in 3D space. It is generally an approximate representation of a curve.
    //! A Polygon3D is defined by a table of nodes. Each node is
    //! a 3D point. If the polygon is closed, the point of closure is
    //! repeated at the end of the table of nodes.
    //! If the polygon is an approximate representation of a curve,
    //! you can associate with each of its nodes the value of the
    //! parameter of the corresponding point on the curve.
    public class Poly_Polygon3D
    {

        //! Constructs a 3D polygon defined by
        //! the table of points, Nodes, and the parallel table of
        //! parameters, Parameters, where each value of the table
        //! Parameters is the parameter of the corresponding point
        //! on the curve approximated by the constructed polygon.
        //! Warning
        //! Both the Nodes and Parameters tables must have the
        //! same bounds. This property is not checked at construction time.
        public Poly_Polygon3D(TColgp_Array1OfPnt Nodes, TColStd_Array1OfReal P)
        {
            myDeflection = (0.0);
            myNodes = new TColgp_Array1OfPnt(1, Nodes.Length());
            myParameters = new TColStd_HArray1OfReal(1, P.Length());
            int i, j = 1;
            for (i = Nodes.Lower(); i <= Nodes.Upper(); i++)
            {
                myNodes[j] = Nodes[i];
                myParameters.SetValue(j, P[i]);
                j++;
            }
        }


        //! Returns the number of nodes in this polygon.
        //! Note: If the polygon is closed, the point of closure is
        //! repeated at the end of its table of nodes. Thus, on a closed
        //! triangle the function NbNodes returns 4.
        public int NbNodes() { return myNodes.Length(); }

        //! Returns the deflection of this polygon
        public double Deflection() { return myDeflection; }

        //! Sets the deflection of this polygon. See more on deflection in Poly_Polygon2D
        public void Deflection(double theDefl) { myDeflection = theDefl; }

        double myDeflection;
        TColgp_Array1OfPnt myNodes;
        TColStd_HArray1OfReal myParameters;

        //! Returns the table of nodes for this polygon.
        public TColgp_Array1OfPnt Nodes() { return myNodes; }
    }
}
