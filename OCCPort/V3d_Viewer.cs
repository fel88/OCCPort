using System;
using System.Linq;
using System.Net.Http.Headers;

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
		internal void SetViewOn()
		{
			//for (V3d_ListOfView.Iterator aDefViewIter (myDefinedViews); aDefViewIter.More(); aDefViewIter.Next())
			foreach (var aDefViewIter in myDefinedViews)
			{
				SetViewOn(aDefViewIter);
			}
		}
		V3d_ListOfLight myActiveLights = new V3d_ListOfLight();

		V3d_ListOfView myActiveViews = new V3d_ListOfView();

		gp_Ax3 myPrivilegedPlane;
		internal void SetViewOn(V3d_View theView)
		{
			Graphic3d_CView aViewImpl = theView.View();
			if (!aViewImpl.IsDefined() || myActiveViews.Contains(theView))
			{
				return;
			}
			myActiveViews.Append(theView);
			aViewImpl.Activate();
			//for (V3d_ListOfLight.Iterator anActiveLightIter (myActiveLights); anActiveLightIter.More(); anActiveLightIter.Next())
			foreach (var anActiveLightIter in myActiveLights)
			{
				theView.SetLightOn(anActiveLightIter);
			}

			Aspect_Grid aGrid = Grid(false);
			if (aGrid != null)
			{
				theView.SetGrid(myPrivilegedPlane, aGrid);
				theView.SetGridActivity(aGrid.IsActive());
			}
			if (theView.SetImmediateUpdate(false))
			{
				theView.Redraw();
				theView.SetImmediateUpdate(true);
			}
		}

		Aspect_GridType myGridType;
		V3d_CircularGrid myCGrid;
		V3d_RectangularGrid myRGrid;

		//! Returns the defined grid in <me>.
		Aspect_Grid Grid(bool theToCreate = true) { return Grid(myGridType, theToCreate); }

		private Aspect_Grid Grid(Aspect_GridType theGridType, bool theToCreate)
		{
			switch (theGridType)
			{
				case Aspect_GridType.Aspect_GT_Circular:
					{
						if (myCGrid == null && theToCreate)
						{
							myCGrid = new V3d_CircularGrid(this, new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY50), new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY70));
						}
						return (Aspect_Grid)(myCGrid);
					}
				case Aspect_GridType.Aspect_GT_Rectangular:
					{
						if (myRGrid == null && theToCreate)
						{
							myRGrid = new V3d_RectangularGrid(this, new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY50), new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY70));
						}
						return (Aspect_Grid)(myRGrid);
					}
			}
			return new Aspect_Grid();
		}
	}
}