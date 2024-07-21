using System;

namespace OCCPort.OpenGL
{
	public class OpenGl_Workspace
	{



		public OpenGl_Context GetGlContext() { return myGlContext; }

		internal bool Activate()
		{
			throw new NotImplementedException();
		}

        //! @return true if usage of Z buffer is enabled.
        public bool UseZBuffer() { return myUseZBuffer; }
        

        internal OpenGl_Aspects Aspects()
		{
			throw new NotImplementedException();
		}

		internal int RenderFilter()
		{
			throw new NotImplementedException();
		}

		internal void ResetAppliedAspect()
		{
			throw new NotImplementedException();
		}

		internal void SetAllowFaceCulling(object value)
		{
			throw new NotImplementedException();
		}

		internal OpenGl_Aspects SetAspects(OpenGl_Aspects myCubeMapParams)
		{
			throw new NotImplementedException();
		}

		internal void SetRenderFilter(int aPrevFilter)
		{
			throw new NotImplementedException();
		}

		internal bool SetUseZBuffer(bool theToUse)
		{
			bool wasUsed = myUseZBuffer;
			myUseZBuffer = theToUse;
			return wasUsed;

		}
		int myRenderFilter;         //!< active filter for skipping rendering of elements by some criteria (multiple render passes)

		internal bool ShouldRender(OpenGl_Element theElement, OpenGl_Group theGroup)
		{
			if ((myRenderFilter & (int)OpenGl_RenderFilter.OpenGl_RenderFilter_SkipTrsfPersistence) != 0)
			{
				if (theGroup.HasPersistence())
				{
					return false;
				}
			}

			// render only non-raytracable elements when RayTracing is enabled
			if ((myRenderFilter & (int)OpenGl_RenderFilter.OpenGl_RenderFilter_NonRaytraceableOnly) != 0)
			{
				if (!theGroup.HasPersistence() && OpenGl_Raytrace.IsRaytracedElement(theElement))
				{
					return false;
				}
			}
			else if ((myRenderFilter & (int)OpenGl_RenderFilter.OpenGl_RenderFilter_FillModeOnly) != 0)
			{
				if (!theElement.IsFillDrawMode())
				{
					return false;
				}
			}
			// handle opaque/transparency render passes
			if ((myRenderFilter & (int)OpenGl_RenderFilter.OpenGl_RenderFilter_OpaqueOnly) != 0)
			{
				if (!theElement.IsFillDrawMode())
				{
					return true;
				}

				if (OpenGl_Context.CheckIsTransparent(myAspectsSet, myHighlightStyle))
				{
					++myNbSkippedTranspElems;
					return false;
				}
			}
			else if ((myRenderFilter & (int)OpenGl_RenderFilter.OpenGl_RenderFilter_TransparentOnly) != 0)
			{
				if (!theElement.IsFillDrawMode())
				{
					if ( ( OpenGl_Aspects) (theElement) == null)
      {
						return false;
					}
				}
				else if (!OpenGl_Context.CheckIsTransparent(myAspectsSet, myHighlightStyle))
				{
					return false;
				}
			}
			return true;


		}
		Graphic3d_PresentationAttributes myHighlightStyle; //!< active highlight style

		OpenGl_Aspects myAspectsSet;
		int myNbSkippedTranspElems; //!< counter of skipped transparent elements for OpenGl_LayerList two rendering passes method

		internal OpenGl_View View()
		{
			return myView;
		}

        internal void SetUseDepthWrite(bool v)
        {
			myUseDepthWrite = v;
        }

        OpenGl_View myView;
		OpenGl_Window myWindow;
		OpenGl_Context myGlContext;
		bool myUseZBuffer;
		bool myUseDepthWrite;
		OpenGl_Aspects myNoneCulling;
		OpenGl_Aspects myFrontCulling;

		public OpenGl_Workspace(OpenGl_View theView, OpenGl_Window theWindow)
		{
			myView = (theView);
			myWindow = (theWindow);
			myGlContext = (theWindow != null ? theWindow.GetGlContext() : null);
			myUseZBuffer = (true);
			myUseDepthWrite = (true);
		}
	}
}