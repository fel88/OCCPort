global using AIS_NArray1OfEntityOwner = TKernel.NCollection_Array1<TKV3d.SelectMgr_EntityOwner>;
global using Graphic3d_NMapOfTransient = TKernel.NCollection_Map<object>;
global using SelectMgr_ListIteratorOfListOfFilter = TKernel.NCollection_List<TKV3d.SelectMgr_Filter>.Iterator;
global using SelectMgr_ListOfFilter = TKernel.NCollection_List<TKV3d.SelectMgr_Filter>;
global using TColStd_ListIteratorOfListOfInteger = TKernel.NCollection_List<int>.Iterator;
global using TColStd_ListOfInteger = TKernel.NCollection_List<int>;
global using TColStd_SequenceOfInteger = TKernel.NCollection_Sequence<int>;
global using AIS_NListOfEntityOwner = TKernel.NCollection_List<TKV3d.SelectMgr_EntityOwner>;
using TKV3d;





using OCCPort.Common;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.JavaScript;
using TKernel;
using TKMath;
using TKService;
using TKTopAlgo;
using TKV3d;

namespace TKV3d
{
    //! The Interactive Context allows you to manage graphic behavior and selection of Interactive Objects in one or more viewers.
    //! Class methods make this highly transparent.
    //! It is essential to remember that an Interactive Object which is already known by the Interactive Context must be modified using Context methods.
    //! You can only directly call the methods available for an Interactive Object if it has not been loaded into an Interactive Context.
    //!
    //! Each selectable object must specify the selection mode that is
    //! responsible for selection of object as a whole (global selection mode).
    //! Interactive context itself supports decomposed object selection with selection filters support.
    //! By default, global selection mode is equal to 0, but it might be redefined if needed.
    public class AIS_InteractiveContext
    {
        public AIS_InteractiveContext(V3d_Viewer MainViewer)
        {
            myMainPM = (new PrsMgr_PresentationManager(MainViewer.StructureManager()));
            myMainVwr = (MainViewer);
            mgrSelector = new SelectMgr_SelectionManager(new StdSelect_ViewerSelector3d());

            myToHilightSelected = true;
            mySelection = (new AIS_Selection());
            myFilters = (new SelectMgr_AndOrFilter(SelectMgr_FilterType.SelectMgr_FilterType_OR));
            myDefaultDrawer = (new Prs3d_Drawer());
            //myDefaultDrawer.SetDisplayMode((int)AIS_DisplayMode.AIS_Shaded);
            myCurDetected = (0);
            myCurHighlighted = (0);
            myPickingStrategy = SelectMgr_PickingStrategy.SelectMgr_PickingStrategy_FirstAcceptable;
            myAutoHilight = true;
            myIsAutoActivateSelMode = (true);

            myStyles[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_None] = myDefaultDrawer;
            myStyles[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_Selected] = new Prs3d_Drawer();
            myStyles[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_Dynamic] = new Prs3d_Drawer();
            myStyles[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_LocalSelected] = new Prs3d_Drawer();
            myStyles[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_LocalDynamic] = new Prs3d_Drawer();
            myStyles[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_SubIntensity] = new Prs3d_Drawer();


            myDefaultDrawer.SetupOwnDefaults();
            myDefaultDrawer.SetZLayer(Graphic3d_ZLayerId.Graphic3d_ZLayerId_Default);
            myDefaultDrawer.SetDisplayMode(0);
            {
                Prs3d_Drawer aStyle = myStyles[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_Dynamic];
                aStyle.Link(myDefaultDrawer);
                initDefaultHilightAttributes(aStyle, new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_CYAN1));
                aStyle.SetZLayer(Graphic3d_ZLayerId.Graphic3d_ZLayerId_Top);
            }
            {
                Prs3d_Drawer aStyle = myStyles[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_LocalDynamic];
                aStyle.Link(myDefaultDrawer);
                initDefaultHilightAttributes(aStyle, new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_CYAN1));
                aStyle.SetZLayer(Graphic3d_ZLayerId.Graphic3d_ZLayerId_Topmost);
            }
            {
                Prs3d_Drawer aStyle = myStyles[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_Selected];
                aStyle.Link(myDefaultDrawer);
                initDefaultHilightAttributes(aStyle, new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY80));
                aStyle.SetZLayer(Graphic3d_ZLayerId.Graphic3d_ZLayerId_UNKNOWN);
            }
            {
                Prs3d_Drawer aStyle = myStyles[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_LocalSelected];
                aStyle.Link(myDefaultDrawer);
                initDefaultHilightAttributes(aStyle, new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY80));
                aStyle.SetZLayer(Graphic3d_ZLayerId.Graphic3d_ZLayerId_UNKNOWN);
            }
            {
                Prs3d_Drawer aStyle = myStyles[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_SubIntensity];
                aStyle.SetZLayer(Graphic3d_ZLayerId.Graphic3d_ZLayerId_UNKNOWN);
                aStyle.SetMethod(Aspect_TypeOfHighlightMethod.Aspect_TOHM_COLOR);
                //aStyle.SetColor(new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY40));
            }

            InitAttributes();
        }

        //! Return Handle(AIS_InteractiveObject)::DownCast (SelectedOwner()->Selectable()).
        //! @sa SelectedOwner().
        public AIS_InteractiveObject SelectedInteractive()
        {
            return !mySelection.More()
                 ? null
                 : (AIS_InteractiveObject)(mySelection.Value().Selectable());
        }

        //! Returns true if there is another object found by the scan of the list of selected objects.
        //! @sa SelectedOwner(), InitSelected(), NextSelected().
        public bool MoreSelected() { return mySelection.More(); }

        //! Continues the scan to the next object in the list of selected objects.
        //! @sa SelectedOwner(), InitSelected(), MoreSelected().
        public void NextSelected() { mySelection.Next(); }

        //! Count a number of selected entities using InitSelected()+MoreSelected()+NextSelected() iterator.
        //! @sa SelectedOwner(), InitSelected(), MoreSelected(), NextSelected().
        public int NbSelected() { return mySelection.Extent(); }

        //! returns Standard_True if the hidden lines are to be drawn.
        //! By default the hidden lines are not drawn.
        public bool DrawHiddenLine() { return myDefaultDrawer.DrawHiddenLine(); }

        public void EnableDrawHiddenLine()
        {
            myDefaultDrawer.EnableDrawHiddenLine();
        }

        //! Select and hilights the previous detected via AIS_InteractiveContext::MoveTo() method;
        //! unhilights the previous picked.
        //! Viewer should be explicitly redrawn after selection.
        //! @param theSelScheme [in] selection scheme
        //! @return picking status
        //!
        //! @sa HighlightStyle() defining default highlight styles of selected owners (Prs3d_TypeOfHighlight_Selected and Prs3d_TypeOfHighlight_LocalSelected)
        //! @sa PrsMgr_PresentableObject::HilightAttributes() defining per-object highlight style of selected owners (overrides defaults)
        public AIS_StatusOfPick SelectDetected(AIS_SelectionScheme theSelScheme = AIS_SelectionScheme.AIS_SelectionScheme_Replace)

        {
            if (theSelScheme == AIS_SelectionScheme.AIS_SelectionScheme_Replace && myLastPicked != null)
            {
                Graphic3d_Vec2i aMousePos = new NCollection_Vec2<int>(-1, -1);
                gp_Pnt2d aMouseRealPos = MainSelector().GetManager().GetMousePosition();
                if (!Precision.IsInfinite(aMouseRealPos.X()) &&
                    !Precision.IsInfinite(aMouseRealPos.Y()))
                {
                    aMousePos.SetValues((int)aMouseRealPos.X(), (int)aMouseRealPos.Y());
                }
                //if (myLastPicked.HandleMouseClick(aMousePos, Aspect_VKeyMouse_LeftButton, Aspect_VKeyFlags_NONE, false))
                {
                    //    return  AIS_StatusOfPick.AIS_SOP_NothingSelected;
                }
            }

            AIS_NArray1OfEntityOwner aPickedOwners = new AIS_NArray1OfEntityOwner(1, 1);
            aPickedOwners.SetValue(1, myLastPicked);
            return Select(aPickedOwners, theSelScheme);
        }




        //! Relays mouse position in pixels theXPix and theYPix to the interactive context selectors.
        //! This is done by the view theView passing this position to the main viewer and updating it.
        //! If theToRedrawOnUpdate is set to false, callee should call RedrawImmediate() to highlight detected object.
        //! @sa PickingStrategy()
        //! @sa HighlightStyle() defining default dynamic highlight styles of detected owners
        //!                      (Prs3d_TypeOfHighlight_Dynamic and Prs3d_TypeOfHighlight_LocalDynamic)
        //! @sa PrsMgr_PresentableObject::DynamicHilightAttributes() defining per-object dynamic highlight style of detected owners (overrides defaults)
        public AIS_StatusOfDetection MoveTo(int theXPix,
                                                 int theYPix,
                                                 V3d_View theView,
                                                 bool theToRedrawOnUpdate)
        {
            if (theView.Viewer() != myMainVwr)
            {
                throw new Standard_ProgramError("AIS_InteractiveContext::MoveTo() - invalid argument");
            }
            MainSelector().Pick(theXPix, theYPix, theView);
            return moveTo(theView, theToRedrawOnUpdate);
        }

        //! Return display mode for highlighting.
        int getHilightMode(AIS_InteractiveObject theObj,
                                    Prs3d_Drawer theStyle,
                                    int theDispMode)
        {
            if (theStyle != null
             && theStyle.DisplayMode() != -1
             && theObj.AcceptDisplayMode(theStyle.DisplayMode()))
            {
                return theStyle.DisplayMode();
            }
            else if (theDispMode != -1)
            {
                return theDispMode;
            }
            else if (theObj.HasDisplayMode())
            {
                return theObj.DisplayMode();
            }
            return myDefaultDrawer.DisplayMode();
        }

        //=======================================================================
        //function : HilightWithColor
        //purpose  : 
        //=======================================================================
        public void HilightWithColor(AIS_InteractiveObject theObj,
                                               Prs3d_Drawer theStyle,
                                               bool theIsToUpdate)
        {
            if (theObj == null)
            {
                return;
            }

            setContextToObject(theObj);
            if (!myObjects.IsBound(theObj))
            {
                return;
            }

            AIS_GlobalStatus aStatus = myObjects[theObj];
            aStatus.SetHilightStatus(true);

            if (theObj.DisplayStatus() == PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Displayed)
            {
                highlightGlobal(theObj, theStyle, aStatus.DisplayMode());
                aStatus.SetHilightStyle(theStyle);
            }

            if (theIsToUpdate)
            {
                myMainVwr.Update();
            }
        }


        //=======================================================================
        //function : highlightWithColor
        //purpose  :
        //=======================================================================
        void highlightWithColor(SelectMgr_EntityOwner theOwner,
                                                  V3d_Viewer theViewer)
        {
            AIS_InteractiveObject anObj = (AIS_InteractiveObject)(theOwner.Selectable());
            if (anObj == null)
            {
                return;
            }

            Prs3d_Drawer aStyle = getHiStyle(anObj, theOwner);
            int aHiMode = getHilightMode(anObj, aStyle, -1);

            myMainPM.BeginImmediateDraw();
            //theOwner.HilightWithColor(myMainPM, aStyle, aHiMode);
            myMainPM.EndImmediateDraw(theViewer == null ? myMainVwr : theViewer);
        }

        AIS_StatusOfDetection moveTo(V3d_View theView,
                                                       bool theToRedrawOnUpdate)
        {
            myCurDetected = 0;
            myCurHighlighted = 0;
            //myDetectedSeq.Clear();
            myLastActiveView = theView;

            // preliminaries
            AIS_StatusOfDetection aStatus = AIS_StatusOfDetection.AIS_SOD_Nothing;
            bool toUpdateViewer = false;

            // filling of myAISDetectedSeq sequence storing information about detected AIS objects
            // (the objects must be AIS_Shapes)
            int aDetectedNb = MainSelector().NbPicked();
            int aNewDetected = 0;
            bool toIgnoreDetTop = false;
            for (int aDetIter = 1; aDetIter <= aDetectedNb; ++aDetIter)
            {
                SelectMgr_EntityOwner anOwner = MainSelector().Picked(aDetIter);
                if (anOwner == null
                 || !myFilters.IsOk(anOwner))
                {
                    if (myPickingStrategy == SelectMgr_PickingStrategy.SelectMgr_PickingStrategy_OnlyTopmost)
                    {
                        toIgnoreDetTop = true;
                    }
                    continue;
                }

                if (aNewDetected < 1
                && !toIgnoreDetTop)
                {
                    aNewDetected = aDetIter;
                }

                myDetectedSeq.Append(aDetIter);
            }

            if (aNewDetected >= 1)
            {
                myCurHighlighted = myDetectedSeq.Lower();

                // Does nothing if previously detected object is equal to the current one.
                // However in advanced selection modes the owners comparison
                // is not effective because in that case only one owner manage the
                // selection in current selection mode. It is necessary to check the current detected
                // entity and hilight it only if the detected entity is not the same as
                // previous detected (IsForcedHilight call)
                SelectMgr_EntityOwner aNewPickedOwner = MainSelector().Picked(aNewDetected);
                if (aNewPickedOwner == myLastPicked && !aNewPickedOwner.IsForcedHilight())
                {
                    return myLastPicked.IsSelected()
                         ? AIS_StatusOfDetection.AIS_SOD_Selected
                         : AIS_StatusOfDetection.AIS_SOD_OnlyOneDetected;
                }

                // Previously detected object is unhilighted if it is not selected or hilighted
                // with selection color if it is selected. Such highlighting with selection color
                // is needed only if myToHilightSelected flag is true. In this case previously detected
                // object has been already highlighted with myHilightColor during previous MoveTo()
                // method call. As result it is necessary to rehighligt it with mySelectionColor.
                if (myLastPicked != null && myLastPicked.HasSelectable())
                {
                    if (isSlowHiStyle(myLastPicked, theView.Viewer()))
                    {
                        theView.Viewer().Invalidate();
                    }

                    clearDynamicHighlight();
                    toUpdateViewer = true;
                }

                // initialize myLastPicked field with currently detected object
                myLastPicked = aNewPickedOwner;

                // highlight detected object if it is not selected or myToHilightSelected flag is true
                if (myLastPicked.HasSelectable())
                {
                    if (myAutoHilight
                     && (!myLastPicked.IsSelected()
                       || myToHilightSelected))
                    {
                        if (isSlowHiStyle(myLastPicked, theView.Viewer()))
                        {
                            theView.Viewer().Invalidate();
                        }

                        highlightWithColor(myLastPicked, theView.Viewer());
                        toUpdateViewer = true;
                    }

                    aStatus = myLastPicked.IsSelected()
                            ? AIS_StatusOfDetection.AIS_SOD_Selected
                            : AIS_StatusOfDetection.AIS_SOD_OnlyOneDetected;
                }
            }
            else
            {
                // previously detected object is unhilighted if it is not selected or hilighted
                // with selection color if it is selected
                aStatus = AIS_StatusOfDetection.AIS_SOD_Nothing;
                if (myAutoHilight
                && myLastPicked != null
                 && myLastPicked.HasSelectable())
                {
                    if (isSlowHiStyle(myLastPicked, theView.Viewer()))
                    {
                        theView.Viewer().Invalidate();
                    }

                    clearDynamicHighlight();
                    toUpdateViewer = true;
                }

                myLastPicked = null;
            }

            if (toUpdateViewer
             && theToRedrawOnUpdate)
            {
                if (theView.ComputedMode())
                {
                    theView.Viewer().Update();
                }
                else
                {
                    if (theView.IsInvalidated())
                    {
                        theView.Viewer().Redraw();
                    }
                    else
                    {
                        theView.Viewer().RedrawImmediate();
                    }
                }
            }

            return aStatus;
        }
        //! Helper function that returns correct dynamic highlight style for the object:
        //! if custom style is defined via object's highlight drawer, it will be used. Otherwise,
        //! dynamic highlight style of interactive context will be returned.
        //! @param theObj [in] the object to check
        Prs3d_Drawer getHiStyle(AIS_InteractiveObject theObj,
                                          SelectMgr_EntityOwner theOwner)
        {
            Prs3d_Drawer aHiDrawer = theObj.DynamicHilightAttributes();
            if (aHiDrawer != null)
            {
                return aHiDrawer;
            }

            if (theOwner != null)
                return myStyles[(int)(theOwner.ComesFromDecomposition() ? Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_LocalDynamic : Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_Dynamic)];

            return myStyles[0];
        }

        //! Return TRUE if highlight style of owner requires full viewer redraw.
        private bool isSlowHiStyle(SelectMgr_EntityOwner theOwner, V3d_Viewer theViewer)
        {
            AIS_InteractiveObject anObj = (AIS_InteractiveObject)(theOwner.Selectable());
            if (anObj != null)
            {
                Prs3d_Drawer aHiStyle = getHiStyle(anObj, myLastPicked);
                return aHiStyle.ZLayer() == Graphic3d_ZLayerId.Graphic3d_ZLayerId_UNKNOWN
                   || !theViewer.ZLayerSettings(aHiStyle.ZLayer()).IsImmediate();
            }
            return false;
        }


        //! Returns the owner of the detected sensitive primitive which is currently dynamically highlighted.
        //! WARNING! This method is irrelevant to InitDetected()/MoreDetected()/NextDetected().
        //! @sa HasDetected(), HasNextDetected(), HilightPreviousDetected(), HilightNextDetected().
        public SelectMgr_EntityOwner DetectedOwner() { return myLastPicked; }

        //! Returns the default attribute manager.
        //! This contains all the color and line attributes which can be used by interactive objects which do not have their own attributes.
        public Prs3d_Drawer DefaultDrawer() { return myDefaultDrawer; }

        //! Return rotation gravity point.
        public gp_Pnt GravityPoint(V3d_View theView)
        {
            return theView.GravityPoint();
        }

        void InitAttributes()
        {
            Graphic3d_MaterialAspect aMat = new Graphic3d_MaterialAspect(Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Brass);
            myDefaultDrawer.ShadingAspect().SetMaterial(aMat);
            /*
              //  myDefaultDrawer->ShadingAspect()->SetColor(Quantity_NOC_GRAY70);
              Handle(Prs3d_LineAspect) aLineAspect = myDefaultDrawer->HiddenLineAspect();
            aLineAspect->SetColor(Quantity_NOC_GRAY20);
            aLineAspect->SetWidth(1.0);
            aLineAspect->SetTypeOfLine(Aspect_TOL_DASH);

            // tolerance to 2 pixels...
            SetPixelTolerance(2);

            // Customizing the drawer for trihedrons and planes...
            Handle(Prs3d_DatumAspect) aTrihAspect = myDefaultDrawer->DatumAspect();
            const Standard_Real aLength = 100.0;
            aTrihAspect->SetAxisLength(aLength, aLength, aLength);
            const Quantity_Color aColor = Quantity_NOC_LIGHTSTEELBLUE4;
            aTrihAspect->LineAspect(Prs3d_DatumParts_XAxis)->SetColor(aColor);
            aTrihAspect->LineAspect(Prs3d_DatumParts_YAxis)->SetColor(aColor);
            aTrihAspect->LineAspect(Prs3d_DatumParts_ZAxis)->SetColor(aColor);

            Handle(Prs3d_PlaneAspect) aPlaneAspect = myDefaultDrawer->PlaneAspect();
            const Standard_Real aPlaneLength = 200.0;
            aPlaneAspect->SetPlaneLength(aPlaneLength, aPlaneLength);
            aPlaneAspect->EdgesAspect()->SetColor(Quantity_NOC_SKYBLUE); */
        }

        //! Initialize default highlighting attributes.
        static void initDefaultHilightAttributes(Prs3d_Drawer theDrawer,
                                             Quantity_Color theColor)
        {
            theDrawer.SetMethod(Aspect_TypeOfHighlightMethod.Aspect_TOHM_COLOR);
            theDrawer.SetDisplayMode(0);
            //theDrawer.SetColor(theColor);

            theDrawer.SetupOwnShadingAspect();
            //theDrawer.SetupOwnPointAspect();
            /*theDrawer.SetLineAspect(new Prs3d_LineAspect(Quantity_NOC_BLACK, Aspect_TOL_SOLID, 1.0));
        *theDrawer->LineAspect()->Aspect() = *theDrawer->Link()->LineAspect()->Aspect();
        */
            theDrawer.SetWireAspect(new Prs3d_LineAspect(new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_BLACK), Aspect_TypeOfLine.Aspect_TOL_SOLID, 1.0));
            //theDrawer.WireAspect().Aspect() = theDrawer.Link().WireAspect().Aspect();
            /*
            theDrawer->SetPlaneAspect(new Prs3d_PlaneAspect());
        *theDrawer->PlaneAspect()->EdgesAspect() = *theDrawer->Link()->PlaneAspect()->EdgesAspect();
            theDrawer->SetFreeBoundaryAspect(new Prs3d_LineAspect(Quantity_NOC_BLACK, Aspect_TOL_SOLID, 1.0));
        *theDrawer->FreeBoundaryAspect()->Aspect() = *theDrawer->Link()->FreeBoundaryAspect()->Aspect();
            theDrawer->SetUnFreeBoundaryAspect(new Prs3d_LineAspect(Quantity_NOC_BLACK, Aspect_TOL_SOLID, 1.0));
        *theDrawer->UnFreeBoundaryAspect()->Aspect() = *theDrawer->Link()->UnFreeBoundaryAspect()->Aspect();
            theDrawer->SetDatumAspect(new Prs3d_DatumAspect());

        theDrawer->ShadingAspect()->SetColor(theColor);
            theDrawer->WireAspect()->SetColor(theColor);
            theDrawer->LineAspect()->SetColor(theColor);
            theDrawer->PlaneAspect()->ArrowAspect()->SetColor(theColor);
            theDrawer->PlaneAspect()->IsoAspect()->SetColor(theColor);
            theDrawer->PlaneAspect()->EdgesAspect()->SetColor(theColor);
            theDrawer->FreeBoundaryAspect()->SetColor(theColor);
            theDrawer->UnFreeBoundaryAspect()->SetColor(theColor);
            theDrawer->PointAspect()->SetColor(theColor);
        for (Standard_Integer aPartIter = 0; aPartIter<Prs3d_DatumParts_None; ++aPartIter)
        {
          if (Handle(Prs3d_LineAspect) aLineAsp = theDrawer->DatumAspect()->LineAspect((Prs3d_DatumParts)aPartIter))
          {
            aLineAsp->SetColor(theColor);
        }
    }

    theDrawer->WireAspect()->SetWidth(2.0);
    theDrawer->LineAspect()->SetWidth(2.0);
    theDrawer->PlaneAspect()->EdgesAspect()->SetWidth(2.0);
    theDrawer->FreeBoundaryAspect()->SetWidth(2.0);
    theDrawer->UnFreeBoundaryAspect()->SetWidth(2.0);
    theDrawer->PointAspect()->SetTypeOfMarker(Aspect_TOM_O_POINT);
    theDrawer->PointAspect()->SetScale(2.0);
            */
            // the triangulation should be computed using main presentation attributes,
            // and should not be overridden by highlighting
            theDrawer.SetAutoTriangulation(false);
        }

        //! Returns the current viewer.
        public V3d_Viewer CurrentViewer() { return myMainVwr; }
        public void SetLocation(AIS_InteractiveObject theIObj,
                                           TopLoc_Location theLoc)
        {
            if (theIObj == null)
            {
                return;
            }

            if (theIObj.HasTransformation()
             && theLoc.IsIdentity())
            {
                theIObj.ResetTransformation();
                mgrSelector.Update(theIObj, false);
                return;
            }
            else if (theLoc.IsIdentity())
            {
                return;
            }

            // first reset the previous location to properly clean everything...
            if (theIObj.HasTransformation())
            {
                theIObj.ResetTransformation();
            }

            theIObj.SetLocalTransformation(theLoc.Transformation());

            mgrSelector.Update(theIObj, false);

            // if the object or its part is highlighted dynamically, it is necessary to apply location transformation
            // to its highlight structure immediately
            if (myLastPicked != null && myLastPicked.IsSameSelectable(theIObj))
            {
                int aHiMod = theIObj.HasHilightMode() ? theIObj.HilightMode() : 0;
                myLastPicked.UpdateHighlightTrsf(myMainVwr,
                                                   myMainPM,
                                                   aHiMod);
            }
        }

        SelectMgr_SelectionManager mgrSelector;
        public StdSelect_ViewerSelector3d MainSelector()
        {
            return mgrSelector.Selector();
        }
        //! @name internal fields

        protected AIS_DataMapOfIOStatus myObjects = new AIS_DataMapOfIOStatus();

        PrsMgr_PresentationManager myMainPM;
        V3d_Viewer myMainVwr;
        V3d_View myLastActiveView;
        SelectMgr_EntityOwner myLastPicked;
        bool myToHilightSelected;
        AIS_Selection mySelection;
        SelectMgr_AndOrFilter myFilters; //!< context filter (the content active filters
                                         //!  can be applied with AND or OR operation)
        Prs3d_Drawer myDefaultDrawer;
        Prs3d_Drawer[] myStyles = new Prs3d_Drawer[(int)Prs3d_TypeOfHighlight.Prs3d_TypeOfHighlight_NB];
        TColStd_SequenceOfInteger myDetectedSeq = new TColStd_SequenceOfInteger();
        int myCurDetected;
        int myCurHighlighted;
        SelectMgr_PickingStrategy myPickingStrategy; //!< picking strategy to be applied within MoveTo()
        bool myAutoHilight;
        bool myIsAutoActivateSelMode;
        public void UpdateCurrentViewer()
        {
            if (myMainVwr != null)
                myMainVwr.Update();
        }

        //=======================================================================
        //function : Display
        //purpose  :
        //===========================================v============================
        public void Display(AIS_InteractiveObject theIObj,
                                       bool theToUpdateViewer)
        {
            if (theIObj == null)
            {
                return;
            }

            int aDispMode = 0, aHiMod = -1, aSelMode = -1;
            GetDefModes(theIObj, ref aDispMode, ref aHiMod, ref aSelMode);
            Display(theIObj, aDispMode, myIsAutoActivateSelMode ? aSelMode : -1, theToUpdateViewer);
        }

        public void GetDefModes(AIS_InteractiveObject theIObj,
                                         ref int theDispMode,
                                          ref int theHiMode,
                                          ref int theSelMode)
        {
            if (theIObj == null)
            {
                return;
            }

            theDispMode = theIObj.HasDisplayMode()
                        ? theIObj.DisplayMode()
                        : (theIObj.AcceptDisplayMode(myDefaultDrawer.DisplayMode())
                         ? myDefaultDrawer.DisplayMode()
                         : 0);
            theHiMode = theIObj.HasHilightMode() ? theIObj.HilightMode() : theDispMode;
            theSelMode = theIObj.GlobalSelectionMode();
        }


        //=======================================================================
        //function : Display
        //purpose  :
        //=======================================================================
        public void Display(AIS_InteractiveObject theIObj,
                                           int theDispMode,
                                           int theSelectionMode,
                                           bool theToUpdateViewer,
                                           PrsMgr_DisplayStatus theDispStatus = PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_None)
        {
            if (theIObj == null)
            {
                return;
            }

            if (theDispStatus == PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Erased)
            {
                Erase(theIObj, theToUpdateViewer);
                Load(theIObj, theSelectionMode);
                AIS_GlobalStatus aStatusPtr = myObjects.ChangeSeek(theIObj);
                if (aStatusPtr != null)
                {
                    aStatusPtr.SetDisplayMode(theDispMode);
                }
                return;
            }

            setContextToObject(theIObj);
            if (!myObjects.IsBound(theIObj))
            {
                setObjectStatus(theIObj, PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Displayed, theDispMode, theSelectionMode);
                theIObj.ViewAffinity().SetVisible(true); // reset view affinity mask
                myMainVwr.StructureManager().RegisterObject(theIObj, theIObj.ViewAffinity());
                myMainPM.Display(theIObj, theDispMode);
                if (theSelectionMode != -1)
                {
                    SelectMgr_SelectableObject anObj = theIObj; // to avoid ambiguity
                    if (!mgrSelector.Contains(anObj))
                    {
                        mgrSelector.Load(theIObj);
                    }
                    mgrSelector.Activate(theIObj, theSelectionMode);
                }
            }
            else
            {
                AIS_GlobalStatus aStatus = myObjects[theIObj];

                // Mark the presentation modes hidden of interactive object different from aDispMode.
                // Then make sure aDispMode is displayed and maybe highlighted.
                // Finally, activate selection mode <SelMode> if not yet activated.
                int anOldMode = aStatus.DisplayMode();
                if (anOldMode != theDispMode)
                {
                    if (myMainPM.IsHighlighted(theIObj, anOldMode))
                    {
                        unhighlightGlobal(theIObj);
                    }
                    myMainPM.SetVisibility(theIObj, anOldMode, false);
                }

                aStatus.SetDisplayMode(theDispMode);

                theIObj.SetDisplayStatus(PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Displayed);
                myMainPM.Display(theIObj, theDispMode);
                if (aStatus.IsHilighted())
                {
                    highlightGlobal(theIObj, aStatus.HilightStyle(), theDispMode);
                }
                if (theSelectionMode != -1)
                {
                    SelectMgr_SelectableObject anObj = theIObj; // to avoid ambiguity
                    if (!mgrSelector.Contains(anObj))
                    {
                        mgrSelector.Load(theIObj);
                    }
                    if (!mgrSelector.IsActivated(theIObj, theSelectionMode))
                    {
                        aStatus.AddSelectionMode(theSelectionMode);
                        mgrSelector.Activate(theIObj, theSelectionMode);
                    }
                }
            }

            if (theToUpdateViewer)
            {
                myMainVwr.Update();
            }
        }


        //! Hides the object. The object's presentations are simply flagged as invisible and therefore excluded from redrawing.
        //! To show hidden objects, use Display().
        private void Erase(AIS_InteractiveObject theIObj, bool theToUpdateViewer)
        {
            if (theIObj == null)
            {
                return;
            }

            if (!theIObj.IsAutoHilight())
            {
                theIObj.ClearSelected();
            }

            EraseGlobal(theIObj, false);
            if (theToUpdateViewer)
            {
                myMainVwr.Update();
            }
        }


        public void EraseGlobal(AIS_InteractiveObject theIObj,
                                          bool theToUpdateviewer)
        {
            //AIS_GlobalStatus aStatus;
            //if (theIObj == null
            //|| !myObjects.Find(theIObj, aStatus)
            //|| theIObj.DisplayStatus() == PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Erased)
            //{
            //    return;
            //}

            //int aDispMode = theIObj.HasHilightMode() ? theIObj.HilightMode() : 0;
            //unselectOwners(theIObj);
            //myMainPM.SetVisibility(theIObj, aStatus.DisplayMode(), false);

            //if (!myLastPicked.IsNull()
            //  && myLastPicked->IsSameSelectable(theIObj))
            //{
            //    clearDynamicHighlight();
            //}

            //// make sure highlighting presentations are properly erased
            //theIObj.ErasePresentations(false);

            //if (IsSelected(theIObj)
            // && aStatus.DisplayMode() != aDispMode)
            //{
            //    myMainPM.SetVisibility(theIObj, aDispMode, false);
            //}

            //foreach (var item in aStatus.SelectionModes())
            //{

            //}
            //for (TColStd_ListIteratorOfListOfInteger aSelModeIter (); aSelModeIter.More(); aSelModeIter.Next())
            //{
            //    mgrSelector->Deactivate(theIObj, aSelModeIter.Value());
            //}
            //aStatus->ClearSelectionModes();
            //theIObj->SetDisplayStatus(PrsMgr_DisplayStatus_Erased);

            //if (theToUpdateviewer)
            //{
            //    myMainVwr->Update();
            //}
        }

        //! Clears the list of entities detected by MoveTo() and resets dynamic highlighting.
        //! @param theToRedrawImmediate if TRUE, the main Viewer will be redrawn on update
        //! @return TRUE if viewer needs to be updated (e.g. there were actually dynamically highlighted entities)
        public bool ClearDetected(bool theToRedrawImmediate = false)
        {
            myCurDetected = 0;
            myCurHighlighted = 0;
            myDetectedSeq.Clear();
            bool toUpdate = false;
            if (myLastPicked != null && myLastPicked.HasSelectable())
            {
                toUpdate = true;
                clearDynamicHighlight();
            }
            myLastPicked = null;
            MainSelector().ClearPicked();
            if (toUpdate && theToRedrawImmediate)
            {
                myMainVwr.RedrawImmediate();
            }
            return toUpdate;
        }

        //! fills <aListOfIO> with objects of a particular Type and Signature with no consideration of display status.
        //! by Default, <WhichSignature> = -1 means control only on <WhichKind>.
        //! if <WhichKind> = AIS_KindOfInteractive_None and <WhichSignature> = -1, all the objects are put into the list.
        public void ObjectsInside(AIS_ListOfInteractive theListOfIO,
                                       AIS_KindOfInteractive theKind = AIS_KindOfInteractive.AIS_KindOfInteractive_None,
                                       int theSign = -1)
        {
            if (theKind == AIS_KindOfInteractive.AIS_KindOfInteractive_None
   && theSign == -1)
            {
                for (AIS_DataMapIteratorOfDataMapOfIOStatus anObjIter = new NCollection_DataMap<AIS_InteractiveObject, AIS_GlobalStatus, NCollection_DefaultHasher<object>>.Iterator(myObjects); anObjIter.More(); anObjIter.Next())
                {
                    theListOfIO.Append(anObjIter.Key());
                }
                return;
            }

            for (AIS_DataMapIteratorOfDataMapOfIOStatus anObjIter = new NCollection_DataMap<AIS_InteractiveObject, AIS_GlobalStatus, NCollection_DefaultHasher<object>>.Iterator(myObjects); anObjIter.More(); anObjIter.Next())
            {
                if (anObjIter.Key().Type() != theKind)
                {
                    continue;
                }

                if (theSign == -1
                 || anObjIter.Key().Signature() == theSign)
                {
                    theListOfIO.Append(anObjIter.Key());
                }
            }

        }
        //! Removes all the objects from Context.
        public void RemoveAll(bool theToUpdateViewer)
        {
            ClearDetected();

            AIS_ListOfInteractive aList = new NCollection_List<AIS_InteractiveObject>();
            ObjectsInside(aList);
            for (AIS_ListOfInteractive.Iterator aListIterator = new NCollection_List<AIS_InteractiveObject>.Iterator(aList); aListIterator.More(); aListIterator.Next())
            {
                Remove(aListIterator.Value(), false);
            }

            if (theToUpdateViewer)
            {
                myMainVwr.Update();
            }
        }

        //! Removes Object from every viewer.
        public void Remove(AIS_InteractiveObject theIObj,
                                      bool theToUpdateViewer)
        {
            if (theIObj == null)
            {
                return;
            }

            if (theIObj.HasInteractiveContext())
            {
                if (theIObj.myCTXPtr != this)
                {
                    throw new Standard_ProgramError("AIS_InteractiveContext - object has been displayed in another context!");
                }
                theIObj.SetContext(null);
            }
            ClearGlobal(theIObj, theToUpdateViewer);
        }
        void unselectOwners(AIS_InteractiveObject theObject)
        {
           /* SelectMgr_SequenceOfOwner aSeq;
            for (AIS_NListOfEntityOwner.Iterator aSelIter =new AIS_NListOfEntityOwner.Iterator (mySelection.Objects()); aSelIter.More(); aSelIter.Next())
            {
                if (aSelIter.Value().IsSameSelectable(theObject))
                {
                    aSeq.Append(aSelIter.Value());
                }
            }
           
            for (SelectMgr_SequenceOfOwner.Iterator aDelIter (aSeq); aDelIter.More(); aDelIter.Next())
            {
                AddOrRemoveSelected(aDelIter.Value(), false);
            }*/
        }

        void ClearGlobal(AIS_InteractiveObject theIObj,
                                           bool theToUpdateviewer)
        {
            AIS_GlobalStatus aStatus = null;
            if (theIObj == null
            || !myObjects.Find(theIObj, out aStatus))
            {
                // for cases when reference shape of connected interactives was not displayed
                // but its selection primitives were calculated
                SelectMgr_SelectableObject anObj = theIObj; // to avoid ambiguity
                mgrSelector.Remove(anObj);
                return;
            }

            unselectOwners(theIObj);

            myMainPM.Erase(theIObj, -1);
            theIObj.ErasePresentations(true); // make sure highlighting presentations are properly erased

            // Object removes from Detected sequence
            for (int aDetIter = myDetectedSeq.Lower(); aDetIter <= myDetectedSeq.Upper();)
            {
                SelectMgr_EntityOwner aPicked = MainSelector().Picked(myDetectedSeq[aDetIter]);
                AIS_InteractiveObject anObj = null;
                if (aPicked != null)
                {
                    anObj = (AIS_InteractiveObject)(aPicked.Selectable());
                }

                if (anObj != null
                  && anObj == theIObj)
                {
                    myDetectedSeq.Remove(aDetIter);
                    if (myCurDetected == aDetIter)
                    {
                        myCurDetected = Math.Min(myDetectedSeq.Upper(), aDetIter);
                    }
                    if (myCurHighlighted == aDetIter)
                    {
                        myCurHighlighted = 0;
                    }
                }
                else
                {
                    aDetIter++;
                }
            }

            // remove IO from the selection manager to avoid memory leaks
            SelectMgr_SelectableObject anObj1 = theIObj; // to avoid ambiguity
            mgrSelector.Remove(anObj1);

            setObjectStatus(theIObj, PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_None, -1, -1);
            theIObj.ViewAffinity().SetVisible(true); // reset view affinity mask
            myMainVwr.StructureManager().UnregisterObject(theIObj);

            if (myLastPicked != null)
            {
                if (myLastPicked.IsSameSelectable(theIObj))
                {
                    clearDynamicHighlight();
                    myLastPicked = null;
                }
            }

            if (theToUpdateviewer
             && theIObj.DisplayStatus() == PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Displayed)
            {
                myMainVwr.Update();
            }
        }

        //! Removes dynamic highlight draw
        void clearDynamicHighlight()
        {
            if (myLastPicked == null)
                return;

            //myLastPicked.Selectable().ClearDynamicHighlight(myMainPM);
        }

        private void setObjectStatus(AIS_InteractiveObject theIObj, PrsMgr_DisplayStatus theStatus,
            int theDispMode, int theSelectionMode)
        {
            theIObj.SetDisplayStatus(theStatus);
            if (theStatus != PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_None)
            {
                AIS_GlobalStatus aStatus = new AIS_GlobalStatus();
                aStatus.SetDisplayMode(theDispMode);
                if (theSelectionMode != -1)
                {
                    aStatus.AddSelectionMode(theSelectionMode);
                }
                myObjects.Bind(theIObj, aStatus);
            }
            else
            {
                myObjects.UnBind(theIObj);
            }

            foreach (var aPrsIter in theIObj.Children())
            {
                AIS_InteractiveObject aChild = aPrsIter as AIS_InteractiveObject;
                if (aChild == null)
                    continue;

                setObjectStatus(aChild, theStatus, theDispMode, theSelectionMode);
            }
        }


        public void setContextToObject(AIS_InteractiveObject theObj)
        {
            if (theObj.HasInteractiveContext())
            {
                //if (theObj.myCTXPtr != this)
                {
                    //throw new Standard_ProgramError("AIS_InteractiveContext - object has been already displayed in another context!");
                }
            }
            else
            {
                theObj.SetContext(this);
            }

            for (PrsMgr_ListOfPresentableObjectsIter aPrsIter = new PrsMgr_ListOfPresentableObjectsIter(theObj.Children()); aPrsIter.More(); aPrsIter.Next())
            {
                AIS_InteractiveObject aChild = aPrsIter.Value() as AIS_InteractiveObject;
                if (aChild != null)
                {
                    setContextToObject(aChild);
                }
            }
        }




        private void unhighlightGlobal(AIS_InteractiveObject theIObj)
        {
            throw new NotImplementedException();
        }

        private void highlightGlobal(AIS_InteractiveObject theIObj, object value, int theDispMode)
        {
            throw new NotImplementedException();
        }

        public AIS_StatusOfPick SelectRectangle(Graphic3d_Vec2i thePntMin,
                                                                   Graphic3d_Vec2i thePntMax,
                                                                   V3d_View theView,
                                                                   AIS_SelectionScheme theSelScheme)
        {
            if (theView.Viewer() != myMainVwr)
            {
                throw new Exception("AIS_InteractiveContext::SelectRectangle() - invalid argument");
            }

            myLastActiveView = theView;
            MainSelector().Pick(thePntMin.x(), thePntMin.y(), thePntMax.x(), thePntMax.y(), theView);

            AIS_NArray1OfEntityOwner aPickedOwners = new AIS_NArray1OfEntityOwner();
            if (MainSelector().NbPicked() > 0)
            {
                aPickedOwners.Resize(1, MainSelector().NbPicked(), false);
                for (int aPickIter = 1; aPickIter <= MainSelector().NbPicked(); ++aPickIter)
                {
                    aPickedOwners.SetValue(aPickIter, MainSelector().Picked(aPickIter));
                }
            }

            return Select(aPickedOwners, theSelScheme);
        }

        // ============================================================================
        // function : Deactivate
        // purpose  :
        // ============================================================================
        public void Deactivate()
        {
            AIS_ListOfInteractive aDisplayedObjects = new();
            DisplayedObjects(aDisplayedObjects);

            for (AIS_ListOfInteractive.Iterator anIter = new AIS_ListOfInteractive.Iterator(aDisplayedObjects); anIter.More(); anIter.Next())
            {
                Deactivate(anIter.Value());
            }
        }

        //! Deactivates all the activated selection modes of an object.
        public void Deactivate(AIS_InteractiveObject theObj)
        {
            SetSelectionModeActive(theObj, -1, false, AIS_SelectionModesConcurrency.AIS_SelectionModesConcurrency_Single);
        }


        //! Deactivates all the activated selection modes of the interactive object anIobj with a given selection mode aMode.
        public void Deactivate(AIS_InteractiveObject theObj, int theMode)
        {
            SetSelectionModeActive(theObj, theMode, false);
        }

        // ============================================================================
        public void Deactivate(int theMode)
        {
            AIS_ListOfInteractive aDisplayedObjects = new();
            DisplayedObjects(aDisplayedObjects);
            for (AIS_ListOfInteractive.Iterator anIter = new AIS_ListOfInteractive.Iterator(aDisplayedObjects); anIter.More(); anIter.Next())
            {
                Deactivate(anIter.Value(), theMode);
            }
        }

        //! Activates or deactivates the selection mode for specified object.
        //! Has no effect if selection mode was already active/deactivated.
        //! @param theObj         object to activate/deactivate selection mode
        //! @param theMode        selection mode to activate/deactivate;
        //!                       deactivation of -1 selection mode will effectively deactivate all selection modes;
        //!                       activation of -1 selection mode with AIS_SelectionModesConcurrency_Single
        //!                       will deactivate all selection modes, and will has no effect otherwise
        //! @param theToActivate  activation/deactivation flag
        //! @param theConcurrency specifies how to handle already activated selection modes;
        //!                       default value (AIS_SelectionModesConcurrency_Multiple) means active selection modes should be left as is,
        //!                       AIS_SelectionModesConcurrency_Single can be used if only one selection mode is expected to be active
        //!                       and AIS_SelectionModesConcurrency_GlobalOrLocal can be used if either AIS_InteractiveObject::GlobalSelectionMode()
        //!                       or any combination of Local selection modes is acceptable;
        //!                       this value is considered only if theToActivate set to TRUE
        //! @param theIsForce     when set to TRUE, the display status will be ignored while activating selection mode
        public void SetSelectionModeActive(AIS_InteractiveObject theObj,
                                               int theMode,
                                               bool theIsActive,
                                               AIS_SelectionModesConcurrency theActiveFilter = AIS_SelectionModesConcurrency.AIS_SelectionModesConcurrency_Multiple,
                                               bool theIsForce = false)
        {
            if (theObj == null)
                return;

            AIS_GlobalStatus aStat = myObjects.Seek(theObj);
            if (aStat == null)
                return;


            if (!theIsActive
             || (theMode == -1
              && theActiveFilter == AIS_SelectionModesConcurrency.AIS_SelectionModesConcurrency_Single))
            {
                if (theObj.DisplayStatus() == PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Displayed
                 || theIsForce)
                {
                    if (theMode == -1)
                    {
                        for (TColStd_ListIteratorOfListOfInteger aModeIter = new TColStd_ListIteratorOfListOfInteger(aStat.SelectionModes()); aModeIter.More(); aModeIter.Next())
                        {
                            mgrSelector.Deactivate(theObj, aModeIter.Value());
                        }
                    }
                    else
                    {
                        mgrSelector.Deactivate(theObj, theMode);
                    }
                }

                if (theMode == -1)
                {
                    aStat.ClearSelectionModes();
                }
                else
                {
                    aStat.RemoveSelectionMode(theMode);
                }
                return;
            }
            else if (theMode == -1)
            {
                return;
            }

            //  if ((*aStat)->SelectionModes().Size() == 1
            //    && (*aStat)->SelectionModes().First() == theMode)
            //  {
            //      return;
            // }

            if (theObj.DisplayStatus() == PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Displayed
             || theIsForce)
            {
                switch (theActiveFilter)
                {
                    case AIS_SelectionModesConcurrency.AIS_SelectionModesConcurrency_Single:
                        {
                            //   for (TColStd_ListIteratorOfListOfInteger aModeIter ((*aStat)->SelectionModes()); aModeIter.More(); aModeIter.Next())
                            {
                                //    mgrSelector->Deactivate(theObj, aModeIter.Value());
                            }
                            //    (*aStat)->ClearSelectionModes();
                            break;
                        }
                    case AIS_SelectionModesConcurrency.AIS_SelectionModesConcurrency_GlobalOrLocal:
                        {
                            int aGlobSelMode = theObj.GlobalSelectionMode();
                            TColStd_ListOfInteger aRemovedModes = new TColStd_ListOfInteger();
                            //for (TColStd_ListIteratorOfListOfInteger aModeIter ((*aStat)->SelectionModes()); aModeIter.More(); aModeIter.Next())
                            //{
                            //    if ((theMode == aGlobSelMode && aModeIter.Value() != aGlobSelMode)
                            //     || (theMode != aGlobSelMode && aModeIter.Value() == aGlobSelMode))
                            //    {
                            //        mgrSelector.Deactivate(theObj, aModeIter.Value());
                            //        aRemovedModes.Append(aModeIter.Value());
                            //    }
                            //}
                            //if (aRemovedModes.Size() == (*aStat)->SelectionModes().Size())
                            //{
                            //    (*aStat)->ClearSelectionModes();
                            //}
                            //else
                            //{
                            //    for (TColStd_ListIteratorOfListOfInteger aModeIter (aRemovedModes); aModeIter.More(); aModeIter.Next())
                            //    {
                            //        (*aStat)->RemoveSelectionMode(aModeIter.Value());
                            //    }
                            //}
                            break;
                        }
                    case AIS_SelectionModesConcurrency.AIS_SelectionModesConcurrency_Multiple:
                        {
                            break;
                        }
                }
                mgrSelector.Activate(theObj, theMode);
            }
            aStat.AddSelectionMode(theMode);
        }

        //! Activates the selection mode aMode whose index is given, for the given interactive entity anIobj.
        public void Activate(AIS_InteractiveObject theObj, int theMode = 0, bool theIsForce = false)
        {
            SetSelectionModeActive(theObj, theMode, true, AIS_SelectionModesConcurrency.AIS_SelectionModesConcurrency_GlobalOrLocal, theIsForce);
        }

        //! Activates the given selection mode for the all displayed objects.
        public void Activate(int theMode,
                                        bool theIsForce = false)
        {
            AIS_ListOfInteractive aDisplayedObjects = new();
            DisplayedObjects(aDisplayedObjects);
            for (AIS_ListOfInteractive.Iterator anIter = new AIS_ListOfInteractive.Iterator(aDisplayedObjects); anIter.More(); anIter.Next())
            {
                Load(anIter.Value(), -1);
                Activate(anIter.Value(), theMode, theIsForce);
            }
        }

        //=======================================================================
        //function : Load
        //purpose  :
        //=======================================================================
        public void Load(AIS_InteractiveObject theIObj,
                                    int theSelMode)
        {
            if (theIObj == null)
            {
                return;
            }

            setContextToObject(theIObj);
            if (!myObjects.IsBound(theIObj))
            {
                //int aDispMode, aHiMod, aSelModeDef;                
                int aDispMode = 0, aHiMod = -1, aSelModeDef = -1;
                GetDefModes(theIObj, ref aDispMode, ref aHiMod, ref aSelModeDef);
                setObjectStatus(theIObj, PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Erased, aDispMode, theSelMode != -1 ? theSelMode : aSelModeDef);
                theIObj.ViewAffinity().SetVisible(true); // reset view affinity mask
                myMainVwr.StructureManager().RegisterObject(theIObj, theIObj.ViewAffinity());
            }

            // Register theIObj in the selection manager to prepare further activation of selection
            SelectMgr_SelectableObject anObj = theIObj; // to avoid ambiguity
            if (!mgrSelector.Contains(anObj))
            {
                mgrSelector.Load(theIObj);
            }
        }

        private void Load(int v1, int v2)
        {
            throw new NotImplementedException();
        }

        private void Activate(int v, int theMode, bool theIsForce)
        {
            throw new NotImplementedException();
        }

        private void Deactivate(int v, int theMode)
        {
            throw new NotImplementedException();
        }


        //! Returns the list of displayed objects of a particular Type WhichKind and Signature WhichSignature.
        //! By Default, WhichSignature equals -1. This means that there is a check on type only.
        public void DisplayedObjects(AIS_ListOfInteractive theListOfIO)
        {
            for (AIS_DataMapIteratorOfDataMapOfIOStatus anObjIter = new AIS_DataMapIteratorOfDataMapOfIOStatus(myObjects); anObjIter.More(); anObjIter.Next())
            {
                if (anObjIter.Key().DisplayStatus() == PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Displayed)
                {
                    theListOfIO.Append(anObjIter.Key());
                }
            }
        }

        //! Returns the list theListOfIO of erased objects (hidden objects) particular Type WhichKind and Signature WhichSignature.
        //! By Default, WhichSignature equals 1. This means that there is a check on type only.
        public void ErasedObjects(AIS_ListOfInteractive theListOfIO)
        {
            ObjectsByDisplayStatus(PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Erased, theListOfIO);

        }


        //! Returns the list theListOfIO of objects with indicated display status particular Type WhichKind and Signature WhichSignature.
        //! By Default, WhichSignature equals 1. This means that there is a check on type only.
        public void ObjectsByDisplayStatus(PrsMgr_DisplayStatus theStatus, AIS_ListOfInteractive theListOfIO)
        {
            for (AIS_DataMapIteratorOfDataMapOfIOStatus anObjIter = new AIS_DataMapIteratorOfDataMapOfIOStatus(myObjects); anObjIter.More(); anObjIter.Next())
            {
                if (anObjIter.Key().DisplayStatus() == theStatus)
                {
                    theListOfIO.Append(anObjIter.Key());
                }
            }
        }



        private AIS_StatusOfPick Select(AIS_NArray1OfEntityOwner aPickedOwners, AIS_SelectionScheme theSelScheme)
        {
            throw new NotImplementedException();
        }
        //! Sets the display mode of seen Interactive Objects (which have no overridden Display Mode).
        public void SetDisplayMode(int theMode,
                                        bool theToUpdateViewer)
        {
            if (theMode == myDefaultDrawer.DisplayMode())
            {
                return;
            }

            for (AIS_DataMapIteratorOfDataMapOfIOStatus anObjIter = new NCollection_DataMap<AIS_InteractiveObject, AIS_GlobalStatus, NCollection_DefaultHasher<object>>.Iterator(myObjects); anObjIter.More(); anObjIter.Next())
            {
                AIS_InteractiveObject anObj = anObjIter.Key();
                bool toProcess = anObj is AIS_Shape
                                          || anObj is AIS_ConnectedInteractive
                                          || anObj is AIS_MultipleConnectedInteractive;

                if (!toProcess
                 || anObj.HasDisplayMode()
                 || !anObj.AcceptDisplayMode(theMode))
                {
                    continue;
                }

                AIS_GlobalStatus aStatus = anObjIter.Value();
                aStatus.SetDisplayMode(theMode);

                if (anObj.DisplayStatus() == PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Displayed)
                {
                    myMainPM.Display(anObj, theMode);
                    if (myLastPicked != null && myLastPicked.IsSameSelectable(anObj))
                    {
                        myMainPM.BeginImmediateDraw();
                        unhighlightGlobal(anObj);
                        myMainPM.EndImmediateDraw(myMainVwr);
                    }
                    if (aStatus.IsSubIntensityOn())
                    {
                        highlightWithSubintensity(anObj, theMode);
                    }
                    myMainPM.SetVisibility(anObj, myDefaultDrawer.DisplayMode(), false);
                }
            }

            myDefaultDrawer.SetDisplayMode(theMode);
            if (theToUpdateViewer)
            {
                myMainVwr.Update();
            }
        }

        private void highlightWithSubintensity(AIS_InteractiveObject anObj, int theMode)
        {

        }

        public void SetDisplayMode(AIS_InteractiveObject theIObj, int theMode, bool theToUpdateViewer)
        {
            setContextToObject(theIObj);
            if (!myObjects.IsBound(theIObj))
            {
                theIObj.SetDisplayMode((int)theMode);
                return;
            }
            else if (!theIObj.AcceptDisplayMode(theMode))
            {
                return;
            }

            AIS_GlobalStatus aStatus = myObjects[theIObj];
            if (theIObj.DisplayStatus() != PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Displayed)
            {
                aStatus.SetDisplayMode(theMode);
                theIObj.SetDisplayMode(theMode);
                return;
            }

            // erase presentations for all display modes different from <aMode>
            int anOldMode = aStatus.DisplayMode();
            if (anOldMode != theMode)
            {
                if (myMainPM.IsHighlighted(theIObj, anOldMode))
                {
                    unhighlightGlobal(theIObj);
                }
                myMainPM.SetVisibility(theIObj, anOldMode, false);
            }

            aStatus.SetDisplayMode(theMode);

            myMainPM.Display(theIObj, theMode);
            if (aStatus.IsHilighted())
            {
                //highlightGlobal(theIObj, getSelStyle(theIObj, theIObj.GlobalSelOwner()), theMode);
            }
            if (aStatus.IsSubIntensityOn())
            {
                highlightWithSubintensity(theIObj, theMode);
            }

            if (theToUpdateViewer)
            {
                myMainVwr.Update();
            }
            theIObj.SetDisplayMode((int)theMode);
        }

        //! Initializes a scan of the selected objects.
        //! @sa SelectedOwner(), MoreSelected(), NextSelected().
        public void InitSelected()
        {
            mySelection.Init();

        }

        //! Provides the type of material setting for the view of the Object.
        public void SetMaterial(AIS_InteractiveObject theIObj, Graphic3d_MaterialAspect theMaterial, bool theToUpdateViewer)
        {
            if (theIObj == null)
                return;

            setContextToObject(theIObj);
            theIObj.SetMaterial(theMaterial);
            theIObj.UpdatePresentations();
            if (theToUpdateViewer)
            {
                UpdateCurrentViewer();
            }
        }
    }


    public enum AIS_StatusOfPick
    {
        AIS_SOP_Error,
        AIS_SOP_NothingSelected,
        AIS_SOP_Removed,
        AIS_SOP_OneSelected,
        AIS_SOP_SeveralSelected
    }

    //! Type of highlighting to apply specific style.
    enum Prs3d_TypeOfHighlight
    {
        Prs3d_TypeOfHighlight_None = 0,       //!< no highlighting
        Prs3d_TypeOfHighlight_Selected,       //!< entire object is selected
        Prs3d_TypeOfHighlight_Dynamic,        //!< entire object is dynamically highlighted
        Prs3d_TypeOfHighlight_LocalSelected,  //!< part of the object is selected
        Prs3d_TypeOfHighlight_LocalDynamic,   //!< part of the object is dynamically highlighted
        Prs3d_TypeOfHighlight_SubIntensity,   //!< sub-intensity style
        Prs3d_TypeOfHighlight_NB
    };

    //! A framework to define sensitive 3D points.
    public class Select3D_SensitivePoint : Select3D_SensitiveEntity
    {
    }

    //! Mouse button bitmask
    enum Mouse_button_bitmask
    {
        Aspect_VKeyMouse_NONE = 0,       //!< no buttons

        Aspect_VKeyMouse_LeftButton = 1 << 13, //!< mouse left   button
        Aspect_VKeyMouse_MiddleButton = 1 << 14, //!< mouse middle button (scroll)
        Aspect_VKeyMouse_RightButton = 1 << 15, //!< mouse right  button

        Aspect_VKeyMouse_MainButtons = Aspect_VKeyMouse_LeftButton | Aspect_VKeyMouse_MiddleButton | Aspect_VKeyMouse_RightButton
    };
    public enum AIS_StatusOfDetection
    {
        AIS_SOD_Error,
        AIS_SOD_Nothing,
        AIS_SOD_AllBad,
        AIS_SOD_Selected,
        AIS_SOD_OnlyOneDetected,
        AIS_SOD_OnlyOneGood,
        AIS_SOD_SeveralGood
    };

}

