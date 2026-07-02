global using V3d_Light = TKService.Graphic3d_CLight;
using OCCPort.Common;
using System;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using TKernel;
using TKMath;
using TKService;

namespace TKV3d
{
    //! Defines services on Viewer type objects.
    //! The methods of this class allow editing and
    //! interrogation of the parameters linked to the viewer
    //! its friend classes (View,light,plane).
    public class V3d_Viewer
    {
        //! Return a list of active views.
        public V3d_ListOfView ActiveViews() { return myActiveViews; }
        double myViewSize;
        V3d_TypeOfVisualization myVisualization;

        bool myComputedMode;
        bool myDefaultComputedMode;

        gp_Ax3 myPrivilegedPlane;
        Graphic3d_Structure myPlaneStructure;
        bool myDisplayPlane;
        double myDisplayPlaneLength;
        bool myGridEcho;

        //! Returns the default size of the view.
        public double DefaultViewSize()  { return myViewSize; }

        //! Returns the default type of Visualization.
    public     V3d_TypeOfVisualization DefaultVisualization()  { return myVisualization; }

        //! Returns the default background colour.
        public Aspect_Background GetBackgroundColor()  { return myBackground; }
        Aspect_Background myBackground = new Aspect_Background();
        Aspect_GradientBackground myGradientBackground = new Aspect_GradientBackground();
        //! Returns the gradient background of the view.
        public Aspect_GradientBackground GetGradientBackground()  { return myGradientBackground; }
  
        public bool IsGlobalLight(V3d_Light theLight)
        {
            return myActiveLights.Contains(theLight);
        }
        public void SetLightOn()
        {
            for (V3d_ListOfLight.Iterator aDefLightIter = new NCollection_List<Graphic3d_CLight>.Iterator(myDefinedLights); aDefLightIter.More(); aDefLightIter.Next())
            {
                if (!myActiveLights.Contains(aDefLightIter.Value()))
                {
                    myActiveLights.Append(aDefLightIter.Value());
                    for (V3d_ListOfView.Iterator anActiveViewIter = new NCollection_List<V3d_View>.Iterator(myActiveViews); anActiveViewIter.More(); anActiveViewIter.Next())
                    {
                        anActiveViewIter.Value().SetLightOn(aDefLightIter.Value());
                    }
                }
            }
        }

        [Obsolete("! Deprecated, Redraw() should be used instead.")]
        public void Update() { Redraw(); }
        public V3d_Viewer(Graphic3d_GraphicDriver theDriver)
        {
            myDriver = theDriver;
            myStructureManager = (new Graphic3d_StructureManager(theDriver));
            /**
			 *   myZLayerGenId (1, IntegerLast()),
  myBackground (Quantity_NOC_GRAY30),*/
            myViewSize = 1000.0;
            myViewProj = V3d_TypeOfOrientation.V3d_XposYnegZpos;

            myVisualization = V3d_TypeOfVisualization.V3d_ZBUFFER;

            myDefaultTypeOfView = V3d_TypeOfView.V3d_ORTHOGRAPHIC;

            myComputedMode = true;
            myDefaultComputedMode = false;
            //myPrivilegedPlane (gp_Ax3 (gp_Pnt (0.,0.,0), gp_Dir (0.,0.,1.), gp_Dir (1.,0.,0.))),
            myDisplayPlane = false;
            myDisplayPlaneLength = 1000.0;
            myGridType = Aspect_GridType.Aspect_GT_Rectangular;
            myGridEcho = (true);
            //myGridEchoLastVert (ShortRealLast(), ShortRealLast(), ShortRealLast())            

        }
        public void RedrawImmediate()
        {
            for (int aSubViewPass = 0; aSubViewPass < 2; ++aSubViewPass)
            {
                // redraw subviews first
                bool isSubViewPass = (aSubViewPass == 0);
                foreach (V3d_View aViewIter in myDefinedViews)
                {
                    if (isSubViewPass
                     && aViewIter.IsSubview())
                    {
                        aViewIter.RedrawImmediate();
                    }
                    else if (!isSubViewPass
                          && !aViewIter.IsSubview())
                    {
                        aViewIter.RedrawImmediate();
                    }
                }
            }
        }
        public void Invalidate()
        {
            for (V3d_ListOfView.Iterator aDefViewIter = new NCollection_List<V3d_View>.Iterator(myDefinedViews); aDefViewIter.More(); aDefViewIter.Next())
            {
                aDefViewIter.Value().Invalidate();
            }
        }

        //! Returns the structure manager associated to this viewer.
        public Graphic3d_StructureManager StructureManager() { return myStructureManager; }
        public V3d_View CreateView()
        {
            return new V3d_View(this, myDefaultTypeOfView);
        }

        V3d_TypeOfView myDefaultTypeOfView;
        Graphic3d_StructureManager myStructureManager;
        public Graphic3d_ZLayerSettings ZLayerSettings(Graphic3d_ZLayerId theLayerId)
        {
            return myDriver.ZLayerSettings(theLayerId);
        }
        //! Return an iterator for active views.
        public V3d_ListOfViewIterator ActiveViewIterator() { return new V3d_ListOfViewIterator(myActiveViews); }

        public void AddView(V3d_View theView)
        {
            if (!myDefinedViews.Contains(theView))
            {
                myDefinedViews.Append(theView);
            }
        }
        V3d_TypeOfOrientation myViewProj;
        //! Returns the default Projection.
        public V3d_TypeOfOrientation DefaultViewProj() { return myViewProj; }


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

        public V3d_ListOfView myActiveViews = new V3d_ListOfView();



        V3d_ListOfLight myDefinedLights = new V3d_ListOfLight();

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
            return null;
        }

        void AddLight(V3d_Light theLight)
        {
            if (!myDefinedLights.Contains(theLight))
            {
                myDefinedLights.Append(theLight);
            }
        }

        void DelLight(V3d_Light theLight)
        {
            SetLightOff(theLight);
            myDefinedLights.Remove(theLight);
        }

        void SetLightOff(V3d_Light theLight)
        {
            myActiveLights.Remove(theLight);
            for (V3d_ListOfView.Iterator anActiveViewIter = new NCollection_List<V3d_View>.Iterator(myActiveViews); anActiveViewIter.More(); anActiveViewIter.Next())
            {
                anActiveViewIter.Value().SetLightOff(theLight);
            }
        }

        public void SetDefaultLights()
        {
            while (!myDefinedLights.IsEmpty())
            {
                V3d_Light aLight = myDefinedLights.First();
                DelLight(aLight);
            }

            V3d_DirectionalLight aDirLight = new V3d_DirectionalLight(V3d_TypeOfOrientation.V3d_Zneg, Quantity_NameOfColor.Quantity_NOC_WHITE);
            aDirLight.SetName("headlight");
            aDirLight.SetHeadlight(true);
            V3d_AmbientLight anAmbLight = new V3d_AmbientLight(Quantity_NameOfColor.Quantity_NOC_WHITE);
            anAmbLight.SetName("amblight");
            AddLight(aDirLight);
            AddLight(anAmbLight);
            SetLightOn(aDirLight);
            SetLightOn(anAmbLight);

        }
        void SetLightOn(V3d_Light theLight)
        {
            if (!myActiveLights.Contains(theLight))
            {
                myActiveLights.Append(theLight);
            }

            for (V3d_ListOfView.Iterator anActiveViewIter = new NCollection_List<V3d_View>.Iterator(myActiveViews); anActiveViewIter.More(); anActiveViewIter.Next())
            {
                anActiveViewIter.Value().SetLightOn(theLight);
            }
        }
        Graphic3d_Structure myGridEchoStructure;

        //! Deactivates the grid in all views of <me>.
        public void DeactivateGrid()
        {
            Aspect_Grid aGrid = Grid(false);
            if (aGrid == null)
                return;

            aGrid.Erase();
            aGrid.Deactivate();

            myGridType = Aspect_GridType.Aspect_GT_Rectangular;
            for (V3d_ListOfView.Iterator anActiveViewIter = new NCollection_List<V3d_View>.Iterator(myActiveViews); anActiveViewIter.More(); anActiveViewIter.Next())
            {
                anActiveViewIter.Value().SetGridActivity(false);
                if (myGridEcho
                && myGridEchoStructure != null)
                {
                    myGridEchoStructure.Erase();
                }
            }
        }

        //! Activates the grid in all views of <me>.
        public void ActivateGrid(Aspect_GridType theType, Aspect_GridDrawMode theMode)
        {
            Aspect_Grid anOldGrid = Grid(false);
            if (anOldGrid != null)
            {
                anOldGrid.Erase();
            }

            myGridType = theType;
            Aspect_Grid aGrid = Grid(true);
            aGrid.SetDrawMode(theMode);
            if (theMode != Aspect_GridDrawMode.Aspect_GDM_None)
            {
                aGrid.Display();
            }
            aGrid.Activate();
            for (V3d_ListOfView.Iterator anActiveViewIter = new NCollection_List<V3d_View>.Iterator(myActiveViews); anActiveViewIter.More(); anActiveViewIter.Next())
            {
                anActiveViewIter.Value().SetGrid(myPrivilegedPlane, aGrid);
            }
        }
    }

    public class V3d_ListOfViewIterator
    {
        V3d_ListOfView list;
        public V3d_ListOfViewIterator(V3d_ListOfView myActiveViews)
        {
            list = myActiveViews;
        }

        public V3d_ListOfViewIterator(V3d_ListOfViewIterator v3d_ListOfViewIterator)
        {
            this.list = v3d_ListOfViewIterator.list;
            this.index = v3d_ListOfViewIterator.index;
        }
        int index = 0;
        public bool More()
        {
            return index < list.Count;
        }

        public void Next()
        {
            index++;
        }

        public V3d_View Value()
        {
            return list[index];
        }
    }
    internal class V3d_RectangularGrid : Aspect_RectangularGrid
    {
        public V3d_RectangularGrid(V3d_Viewer aViewer, Quantity_Color quantity_Color1, Quantity_Color quantity_Color2)
            : base(1.0, 1.0)
        {
            myViewer = (aViewer);

        }

        Graphic3d_Structure myStructure;
        Graphic3d_Group myGroup;
        gp_Ax3 myCurViewPlane;
        V3d_Viewer myViewer;
    }
    public class V3d_CircularGrid : Aspect_CircularGrid
    {
        public V3d_CircularGrid(V3d_Viewer aViewer, Quantity_Color aColor, Quantity_Color aTenthColor)
            : base(1.0, 8)
        {
            /*myViewer = (aViewer);
            myCurAreDefined = (false);
    myToComputePrs(Standard_False),
    myCurDrawMode(Aspect_GDM_Lines),
    myCurXo(0.0),
    myCurYo(0.0),
    myCurAngle(0.0),*/

        }
    }
}


