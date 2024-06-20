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
			Redraw();
		}
		V3d_ListOfView myDefinedViews = new V3d_ListOfView();
		public void Redraw()
		{
			for (int aSubViewPass = 0; aSubViewPass < 2; ++aSubViewPass)
			{
				// redraw subviews first
				bool isSubViewPass = (aSubViewPass == 0);
				foreach (var aViewIter in myDefinedViews)
				{

					if (isSubViewPass && aViewIter.IsSubview())
					{
						aViewIter.Redraw();
        }
					else if (!isSubViewPass
						  && !aViewIter.IsSubview())
					{
						aViewIter.Redraw();
					}
				}
			}
		}

		Graphic3d_GraphicDriver myDriver;
		//! Return Graphic Driver instance.
		public Graphic3d_GraphicDriver Driver() { return myDriver; }

		internal void SetViewOn(V3d_View v3d_View)
		{
			throw new NotImplementedException();
		}
    }
	//! Defines the type of projection of the view.
	public enum V3d_TypeOfView
	{
		V3d_ORTHOGRAPHIC,
		V3d_PERSPECTIVE
	};

}