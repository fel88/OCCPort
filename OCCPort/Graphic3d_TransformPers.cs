using System;

namespace OCCPort
{
	public class Graphic3d_TransformPers
	{
		internal bool IsTrihedronOr2d(Graphic3d_TransModeFlags theMode)
		{
			return (theMode & (Graphic3d_TransModeFlags.Graphic3d_TMF_TriedronPers | Graphic3d_TransModeFlags.Graphic3d_TMF_2d)) != 0;

		}

		Graphic3d_TransModeFlags myMode;  //!< Transformation persistence mode flags

		//! Return true for Graphic3d_TMF_TriedronPers and Graphic3d_TMF_2d modes.
		public bool IsTrihedronOr2d() { return IsTrihedronOr2d(myMode); }
	}


}