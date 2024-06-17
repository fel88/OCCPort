namespace OCCPort
{
	//! Root class for the curve representations. Contains
	//! a location.

	public class BRep_CurveRepresentation
	{
		public Geom_Curve Curve3D()
		{
			throw new Standard_DomainError("BRep_CurveRepresentation");
		}

		public void Curve3D(Geom_Curve cc)
		{
			throw new Standard_DomainError("BRep_CurveRepresentation");
		}


		public TopLoc_Location Location()
		{
			return myLocation;
		}
		//Standard_EXPORT BRep_CurveRepresentation(const TopLoc_Location& L);
		public void Location(TopLoc_Location L)
		{
			myLocation = L;
		}

		TopLoc_Location myLocation;


	}
}