using System;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace OCCPort
{
	public class Poly_PolygonOnTriangulation
	{
		//! Returns the number of nodes for this polygon.
		//! Note: If the polygon is closed, the point of closure is
		//! repeated at the end of its table of nodes. Thus, on a closed
		//! triangle, the function NbNodes returns 4.
		public int NbNodes() { return myNodes.Length(); }
		TColStd_Array1OfInteger myNodes;
		//! Returns the deflection of this polygon
		public double Deflection() { return myDeflection; }

        //! Returns the table of the parameters associated with each node in this polygon.
        //! Warning! Use the function HasParameters to check if parameters are associated with the nodes in this polygon.
      public   TColStd_HArray1OfReal Parameters()  { return myParameters; }

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
    }
}