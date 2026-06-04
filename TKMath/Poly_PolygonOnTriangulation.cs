using OCCPort.Common;
using TKernel;

namespace TKMath
{
    //! This class provides a polygon in 3D space, based on the triangulation
    //! of a surface. It may be the approximate representation of a
    //! curve on the surface, or more generally the shape.
    //! A PolygonOnTriangulation is defined by a table of
    //! nodes. Each node is an index in the table of nodes specific
    //! to a triangulation, and represents a point on the surface. If
    //! the polygon is closed, the index of the point of closure is
    //! repeated at the end of the table of nodes.
    //! If the polygon is an approximate representation of a curve
    //! on a surface, you can associate with each of its nodes the
    //! value of the parameter of the corresponding point on the
    //! curve.represents a 3d Polygon
    public class Poly_PolygonOnTriangulation
    {


        public Poly_PolygonOnTriangulation
   (TColStd_Array1OfInteger Nodes,
    TColStd_Array1OfReal Parameters)

        {
            myDeflection = (0.0);
            myNodes = new TColStd_Array1OfInteger(1, Nodes.Length());
            myNodes = Nodes;
            //myParameters = new TColStd_HArray1OfReal(1, Parameters.Length());
            // myParameters.ChangeArray1() = Parameters;
        }

        //! Returns the number of nodes for this polygon.
        //! Note: If the polygon is closed, the point of closure is
        //! repeated at the end of its table of nodes. Thus, on a closed
        //! triangle, the function NbNodes returns 4.
        public int NbNodes() { return myNodes.Length(); }
        TColStd_Array1OfInteger myNodes;
        //! Returns the deflection of this polygon
        public double Deflection() { return myDeflection; }
        //! Sets the deflection of this polygon.
        //! See more on deflection in Poly_Polygones2D.
        public void Deflection(double theDefl) { myDeflection = theDefl; }

        //! Returns the table of the parameters associated with each node in this polygon.
        //! Warning! Use the function HasParameters to check if parameters are associated with the nodes in this polygon.
        public TColStd_HArray1OfReal Parameters() { return myParameters; }

        double myDeflection;
        TColStd_HArray1OfReal myParameters;

        //! Returns the table of nodes for this polygon.
        //! A node value is an index in the table of nodes specific to an existing triangulation of a shape.
        public TColStd_Array1OfInteger Nodes() { return myNodes; }

        //! Returns true if parameters are associated with the nodes in this polygon.
        internal bool HasParameters()
        {

            return myParameters != null;

        }

        internal int Node(int aNodeIt)
        {
            throw new NotImplementedException();
        }
    }
}
