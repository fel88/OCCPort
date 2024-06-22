using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OCCPort.OpenGL
{
	//! This class defines an OpenGl graphic driver
	public class OpenGl_GraphicDriver : Graphic3d_GraphicDriver
	{

		OpenGl_Caps myCaps;
		MyMapOfView myMapOfView = new MyMapOfView();
		Dictionary<int, OpenGl_Structure> myMapOfStructure = new Dictionary<int, OpenGl_Structure>();


		OpenGl_StateCounter myStateCounter; //!< State counter for OpenGl structures.

		public override Graphic3d_CStructure CreateStructure(Graphic3d_StructureManager theManager)
		{
			OpenGl_Structure aStructure = new OpenGl_Structure(theManager);
			myMapOfStructure.Add(aStructure.Identification(), aStructure);
			return aStructure;

		}

		public override Graphic3d_CView CreateView(Graphic3d_StructureManager theMgr)
		{

			OpenGl_View aView = new OpenGl_View(theMgr, this, myCaps, myStateCounter);
			myMapOfView.Add(aView);
			/*for (NCollection_List < Handle(Graphic3d_Layer) >::Iterator aLayerIter(myLayers); aLayerIter.More(); aLayerIter.Next())
			{
				Graphic3d_Layer aLayer = aLayerIter.Value();
				aView.InsertLayerAfter(aLayer->LayerId(), aLayer->LayerSettings(), Graphic3d_ZLayerId_UNKNOWN);
			}*/
			return aView;

		}

		internal OpenGl_Window CreateRenderWindow(Aspect_Window theNativeWindow,
			Aspect_Window theSizeWindow, Aspect_RenderingContext theContext)
		{
			OpenGl_Context aShareCtx = GetSharedContext();
			OpenGl_Window aWindow = new OpenGl_Window();
			aWindow.Init(this, theNativeWindow, theSizeWindow, theContext, myCaps, aShareCtx);
			return aWindow;

		}

		internal int GetNextPrimitiveArrayUID()
		{
			throw new NotImplementedException();
		}

		const OpenGl_Context TheNullGlCtx = null;

		internal OpenGl_Context GetSharedContext(bool theBound = false)
		{
			if (myMapOfView.IsEmpty())
			{
				return TheNullGlCtx;
			}

			foreach (var aViewIter in myMapOfView)
			{
				OpenGl_Window aWindow = aViewIter.GlWindow();
				if (aWindow != null)
				{
					if (!theBound)
					{
						return aWindow.GetGlContext();
					}
					else if (aWindow.GetGlContext().IsCurrent())
					{
						return aWindow.GetGlContext();
					}
				}
			}

			return TheNullGlCtx;
		}

		internal void setDeviceLost()
		{
			throw new NotImplementedException();
		}
	}
}