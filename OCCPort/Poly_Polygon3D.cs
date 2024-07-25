namespace OCCPort
{
	public class Poly_Polygon3D
	{
		//! Returns the number of nodes in this polygon.
		//! Note: If the polygon is closed, the point of closure is
		//! repeated at the end of its table of nodes. Thus, on a closed
		//! triangle the function NbNodes returns 4.
		public int NbNodes() { return myNodes.Length(); }

		double myDeflection;
		TColgp_Array1OfPnt myNodes;
	}
}