using System;

namespace OCCPort
{
	internal class SelectMgr_BaseIntersector
	{
		//! Return camera definition.
		public Graphic3d_Camera Camera() { return myCamera; }

		Graphic3d_Camera myCamera;        //!< camera definition (if builder isn't NULL it is the same as its camera)
		SelectMgr_SelectionType mySelectionType; //!< type of selection

	}
}