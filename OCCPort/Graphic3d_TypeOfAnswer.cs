namespace OCCPort
{
	//! The answer of the method AcceptDisplay
	//! AcceptDisplay  means is it possible to display the
	//! specified structure in the specified view ?
	//! TOA_YES yes
	//! TOA_NO  no
	//! TOA_COMPUTE yes but we have to compute the representation
	enum Graphic3d_TypeOfAnswer
	{
		Graphic3d_TOA_YES,
		Graphic3d_TOA_NO,
		Graphic3d_TOA_COMPUTE
	}

}