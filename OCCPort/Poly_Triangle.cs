namespace OCCPort
{
    //! Describes a component triangle of a triangulation (Poly_Triangulation object).
    //! A Triangle is defined by a triplet of nodes within [1, Poly_Triangulation::NbNodes()] range.
    //! Each node is an index in the table of nodes specific to an existing
    //! triangulation of a shape, and represents a point on the surface.
    public class Poly_Triangle
    {
        //! Constructs a triangle and sets all indices to zero.
        public Poly_Triangle() { myNodes[0] = myNodes[1] = myNodes[2] = 0; }

        //! Constructs a triangle and sets its three indices,
        //! where these node values are indices in the table of nodes specific to an existing triangulation of a shape.
        public Poly_Triangle(int theN1, int theN2, int theN3)
        {
            myNodes[0] = theN1;
            myNodes[1] = theN2;
            myNodes[2] = theN3;
        }

        int[] myNodes = new int[3];
    }
}