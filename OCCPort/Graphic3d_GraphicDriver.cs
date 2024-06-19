using System;

namespace OCCPort
{
	public abstract class Graphic3d_GraphicDriver
	{
		//! Creates new view for this graphic driver.
		public abstract Graphic3d_CView CreateView(Graphic3d_StructureManager theMgr);


	}

	

}