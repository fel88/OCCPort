using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using TKernel;
using TKService;

namespace OCCPort.OpenGL
{
    //! This class defines an OpenGl graphic driver
    public class OpenGl_GraphicDriver : Graphic3d_GraphicDriver
    {

        //! Constructor.
        //! @param theDisp connection to display, required on Linux but optional on other systems
        //! @param theToInitialize perform initialization of default OpenGL context on construction
        public OpenGl_GraphicDriver(Aspect_DisplayConnection theDisp, bool theToInitialize = true) : base(theDisp)
        {

            myIsOwnContext = (false);
            //myEglDisplay = (NULL);
            //myEglContext = (NULL);
            //myEglConfig = (null);
            myCaps = new OpenGl_Caps();
            //myMapOfView = new(1, NCollection_BaseAllocator:CommonBaseAllocator());
            //myMapOfStructure = new(1, NCollection_BaseAllocator::CommonBaseAllocator());
            if (theToInitialize
 && !InitContext())
            {
                throw new Aspect_GraphicDeviceDefinitionError("OpenGl_GraphicDriver: default context can not be initialized!");
            }
        }

        //! Perform initialization of default OpenGL context.
        public bool InitContext()
        {
            ReleaseContext();

            chooseVisualInfo();
            myIsOwnContext = true;
            return true;
        }

        private void chooseVisualInfo()
        {
            //throw new NotImplementedException();
        }

        private void ReleaseContext()
        {
            //  throw new NotImplementedException();
        }

        OpenGl_Caps myCaps;
        MyMapOfView myMapOfView = new MyMapOfView();
        NCollection_DataMap<int, OpenGl_Structure> myMapOfStructure = new();
        bool myIsOwnContext; //!< indicates that shared context has been created within OpenGl_GraphicDriver

        //! Specify swap buffer behavior.
        public void SetBuffersNoSwap(bool theIsNoSwap)
        {
            myCaps.buffersNoSwap = theIsNoSwap;
        }

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
            foreach (var aLayer in myLayers)
            {
                aView.InsertLayerAfter(aLayer.LayerId(), aLayer.LayerSettings(), Graphic3d_ZLayerId.Graphic3d_ZLayerId_UNKNOWN);

            }
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

        //! Returns unique ID for primitive arrays.
        internal int GetNextPrimitiveArrayUID()
        {
            return myUIDGenerator.Increment();
        }
        OpenGl_StateCounter myUIDGenerator = new OpenGl_StateCounter(); //!< Unique ID counter for primitive arrays.

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



        //! Set device lost flag for redrawn views.
        internal void setDeviceLost()
        {
            if (myMapOfStructure.IsEmpty())
            {
                return;
            }

            foreach (var aView in myMapOfView)
            {
                if (aView.myWasRedrawnGL)
                {
                    aView.StructureManager().SetDeviceLost();
                }
            }
        }
    }
}