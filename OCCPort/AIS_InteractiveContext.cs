using OCCPort;
using OCCPort.Tester;
using System;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Policy;

namespace OCCPort
{
    public class AIS_InteractiveContext
    {
        public AIS_InteractiveContext(V3d_Viewer MainViewer)
        {
			myMainPM = (new PrsMgr_PresentationManager(MainViewer.StructureManager()));
			myMainVwr = (MainViewer);
            mgrSelector = new SelectMgr_SelectionManager(new StdSelect_ViewerSelector3d());
			myDefaultDrawer = (new Prs3d_Drawer());
			myToHilightSelected = true;
			mySelection = (new AIS_Selection());
			myFilters = (new SelectMgr_AndOrFilter(SelectMgr_FilterType.SelectMgr_FilterType_OR));
			myDefaultDrawer = (new Prs3d_Drawer());
			myCurDetected = (0);
			myCurHighlighted = (0);
			myPickingStrategy = SelectMgr_PickingStrategy.SelectMgr_PickingStrategy_FirstAcceptable;
			myAutoHilight = true;
			myIsAutoActivateSelMode = (true);

        }

		//! Returns the current viewer.
		public V3d_Viewer CurrentViewer() { return myMainVwr; }


        SelectMgr_SelectionManager mgrSelector;
        StdSelect_ViewerSelector3d MainSelector()
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
        //Prs3d_Drawer myStyles[Prs3d_TypeOfHighlight_NB];
        int[] myDetectedSeq;
        int myCurDetected;
        int myCurHighlighted;
        SelectMgr_PickingStrategy myPickingStrategy; //!< picking strategy to be applied within MoveTo()
        bool myAutoHilight;
        bool myIsAutoActivateSelMode;

        //=======================================================================
        //function : Display
        //purpose  :
        //=======================================================================
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

        private void Erase(AIS_InteractiveObject theIObj, bool theToUpdateViewer)
        {
            throw new NotImplementedException();
        }

        private void setObjectStatus(AIS_InteractiveObject theIObj, PrsMgr_DisplayStatus prsMgr_DisplayStatus_Displayed, int theDispMode, int theSelectionMode)
        {
			//theIObj.SetDisplayStatus(theStatus);
			//if (theStatus != PrsMgr_DisplayStatus_None)
			//{
			//	AIS_GlobalStatus aStatus = new AIS_GlobalStatus();
			//	aStatus.SetDisplayMode(theDispMode);
			//	if (theSelectionMode != -1)
			//	{
			//		aStatus.AddSelectionMode(theSelectionMode);
			//	}
			//	myObjects.Bind(theIObj, aStatus);
			//}
			//else
			//{
			//	myObjects.UnBind(theIObj);
			//}

			//for (PrsMgr_ListOfPresentableObjectsIter aPrsIter (theIObj->Children()); aPrsIter.More(); aPrsIter.Next())
			//{
			//	AIS_InteractiveObject aChild=(Handle(AIS_InteractiveObject)::DownCast(aPrsIter.Value()));
			//	if (aChild.IsNull())
			//	{
			//		continue;
			//	}

			//	setObjectStatus(aChild, theStatus, theDispMode, theSelectionMode);
			//}
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
            //if (theView.Viewer() != myMainVwr)
            {
                //throw new Exception("AIS_InteractiveContext::SelectRectangle() - invalid argument");
            }

            //myLastActiveView = theView.get();
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

        public void SetDisplayMode(AIS_Shape shape, AIS_DisplayMode aIS_Shaded, bool v)
        {
            throw new NotImplementedException();
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
