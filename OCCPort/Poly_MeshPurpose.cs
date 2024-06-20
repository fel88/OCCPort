namespace OCCPort
{
    public enum Poly_MeshPurpose
	{
		Poly_MeshPurpose_NONE,

		//no special use (default)
		Poly_MeshPurpose_Calculation,

		//mesh for algorithms
		Poly_MeshPurpose_Presentation,

		//mesh for presentation(Ls usage)
		Poly_MeshPurpose_Active,

		//mesh marked as currently active in a list
		Poly_MeshPurpose_Loaded,

		//mesh has currently loaded data
		Poly_MeshPurpose_AnyFallback,

		//a special flag for BRep_Tools::Triangulation() to return any other defined mesh,
		Poly_MeshPurpose_USER,

		//application-defined flags

	}
 }

