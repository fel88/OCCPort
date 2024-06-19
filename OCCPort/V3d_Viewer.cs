using System;

namespace OCCPort
{
    public class V3d_Viewer
    {

		public V3d_Viewer(Graphic3d_GraphicDriver theDriver)
		{
			myDriver = theDriver;
			myStructureManager = (new Graphic3d_StructureManager(theDriver));

		}
        //! Returns the structure manager associated to this viewer.
        public Graphic3d_StructureManager StructureManager() { return myStructureManager; }
		public V3d_View CreateView()
		{
			return new V3d_View(this, myDefaultTypeOfView);
		}
		V3d_TypeOfView myDefaultTypeOfView;
        Graphic3d_StructureManager myStructureManager;

        internal void Update()
        {
            throw new NotImplementedException();
        }
		Graphic3d_GraphicDriver myDriver;
		//! Return Graphic Driver instance.
		public Graphic3d_GraphicDriver Driver() { return myDriver; }

    }
	//! Defines the type of projection of the view.
	public enum V3d_TypeOfView
	{
		V3d_ORTHOGRAPHIC,
		V3d_PERSPECTIVE
	};

}