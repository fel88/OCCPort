using System;

namespace OCCPort
{
	public abstract class Graphic3d_GraphicDriver
	{
		//! Creates new view for this graphic driver.
		public abstract Graphic3d_CView CreateView(Graphic3d_StructureManager theMgr);

		public abstract Graphic3d_CStructure CreateStructure(Graphic3d_StructureManager theManager);

		internal int NewIdentification()
		{
			return myStructGenId.Next();
		}
		Aspect_GenId myStructGenId = new Aspect_GenId();
	}

	

}