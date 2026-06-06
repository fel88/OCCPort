using OCCPort.Common;
using TKernel;
using TKMath;
using TKService;

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
            /*  Graphic3d_MaterialAspect aMat=new Graphic3d_MaterialAspect ( Graphic3d_NameOfMaterial_Brass);
              myDefaultDrawer.ShadingAspect().SetMaterial(aMat);

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
              aPlaneAspect->EdgesAspect()->SetColor(Quantity_NOC_SKYBLUE);*/
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
        int[] myDetectedSeq;
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
            AIS_ListOfInteractive aDisplayedObjects;
            DisplayedObjects(out aDisplayedObjects);

            for (AIS_ListOfInteractive.Iterator anIter = new AIS_ListOfInteractive.Iterator(aDisplayedObjects); anIter.More(); anIter.Next())
            {
                Deactivate(anIter.Value());
            }
        }


        // ============================================================================
        public void Deactivate(int theMode)
        {
            AIS_ListOfInteractive aDisplayedObjects;
            DisplayedObjects(out aDisplayedObjects);
            for (AIS_ListOfInteractive.Iterator anIter = new AIS_ListOfInteractive.Iterator(aDisplayedObjects); anIter.More(); anIter.Next())
            {
                Deactivate(anIter.Value(), theMode);
            }
        }

        // ============================================================================
        // function : Activate
        // purpose  :
        // ============================================================================
        public void Activate(int theMode,
                                        bool theIsForce)
        {
            AIS_ListOfInteractive aDisplayedObjects;
            DisplayedObjects(out aDisplayedObjects);
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

        private void DisplayedObjects(out AIS_ListOfInteractive aDisplayedObjects)
        {
            throw new NotImplementedException();
        }

        private AIS_StatusOfPick Select(AIS_NArray1OfEntityOwner aPickedOwners, AIS_SelectionScheme theSelScheme)
        {
            throw new NotImplementedException();
        }

        public void SetDisplayMode(AIS_Shape theIObj, AIS_DisplayMode theMode, bool theToUpdateViewer)
        {
            setContextToObject(theIObj);
            if (!myObjects.IsBound(theIObj))
            {
                theIObj.SetDisplayMode((int)theMode);
                return;
            }
            /*else if (!theIObj.AcceptDisplayMode(theMode))
            {
                return;
            }*/

            //AIS_GlobalStatus aStatus = myObjects(theIObj);
            /*if (theIObj->DisplayStatus() != PrsMgr_DisplayStatus_Displayed)
            {
                aStatus->SetDisplayMode(theMode);
                theIObj->SetDisplayMode(theMode);
                return;
            }

            // erase presentations for all display modes different from <aMode>
            const Standard_Integer anOldMode = aStatus->DisplayMode();
            if (anOldMode != theMode)
            {
                if (myMainPM->IsHighlighted(theIObj, anOldMode))
                {
                    unhighlightGlobal(theIObj);
                }
                myMainPM->SetVisibility(theIObj, anOldMode, Standard_False);
            }

            aStatus->SetDisplayMode(theMode);

            myMainPM->Display(theIObj, theMode);
            if (aStatus->IsHilighted())
            {
                highlightGlobal(theIObj, getSelStyle(theIObj, theIObj->GlobalSelOwner()), theMode);
            }
            if (aStatus->IsSubIntensityOn())
            {
                highlightWithSubintensity(theIObj, theMode);
            }
            */
            if (theToUpdateViewer)
            {
                myMainVwr.Update();
            }
            theIObj.SetDisplayMode((int)theMode);
        }
    }

    internal class AIS_ListOfInteractive
    {
        internal class Iterator
        {


            public Iterator(AIS_ListOfInteractive aDisplayedObjects)
            {

            }

            internal bool More()
            {
                throw new NotImplementedException();
            }

            internal object Next()
            {
                throw new NotImplementedException();
            }

            internal int Value()
            {
                throw new NotImplementedException();
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


    //! Stores information about objects in graphic context:
    public class AIS_GlobalStatus
    {
        internal bool AddSelectionMode(int theMode)
        {
            if (!mySelModes.Contains(theMode))
            {
                mySelModes.Append(theMode);
                return true;
            }
            return false;
        }


        //! Returns active selection modes of the object.
        public TColStd_ListOfInteger SelectionModes() { return mySelModes; }
        TColStd_ListOfInteger mySelModes = new TColStd_ListOfInteger();

        internal int DisplayMode()
        {
            return myDispMode;
        }

        internal Prs3d_Drawer HilightStyle()
        {
            return myHiStyle;
        }

        internal bool IsHilighted()
        {
            return myIsHilit;
        }
        Prs3d_Drawer myHiStyle;


        int myDispMode;
        bool myIsHilit;
        bool mySubInt;
        //! Sets display mode.
        internal void SetDisplayMode(int theMode)
        {
            myDispMode = theMode;
        }
    }

    //! Class holding the list of selected owners.
    public class AIS_Selection
    {
    }
    public class AIS_NArray1OfEntityOwner : NCollection_Array1<SelectMgr_EntityOwner>
    {

    }
    public enum SelectMgr_PickingStrategy
    {

        //Enumeration defines picking strategy - which entities detected by picking line will be accepted, considering selection filters.

        SelectMgr_PickingStrategy_FirstAcceptable,

        //the first detected entity passing selection filter is accepted(e.g.any)
        SelectMgr_PickingStrategy_OnlyTopmost

        //only topmost detected entity passing selection filter is accepted
    }

    public class SelectMgr_AndOrFilter
    {
        public SelectMgr_AndOrFilter(SelectMgr_FilterType selectMgr_FilterType_OR)
        {
        }
    }
    public enum SelectMgr_FilterType
    {

        //Enumeration defines the filter type.

        SelectMgr_FilterType_AND,

        //an object should be suitable for all filters.
        SelectMgr_FilterType_OR

        //an object should be suitable at least one filter. 
    }
}

