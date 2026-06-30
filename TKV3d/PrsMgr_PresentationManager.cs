using OCCPort.Common;
using System.Reflection.Metadata;
using TKMath;
using TKService;

namespace TKV3d
{
    //! A framework to manage 3D displays, graphic entities and their updates.
    //! Used in the AIS package (Application Interactive Services), to enable the advanced user to define the
    //! default display mode of a new interactive object which extends the list of signatures and types.
    //! Definition of new display types is handled by calling the presentation algorithms provided by the StdPrs package.
    public class PrsMgr_PresentationManager
    {
        public PrsMgr_PresentationManager(Graphic3d_StructureManager theStructureManager)

        {
            myStructureManager = (theStructureManager);
            myImmediateModeOn = (0);
            //
        }
        //! Internal function that scans thePrsList for shadow presentations
        //! and applies transformation theTrsf to them in case if parent ID
        //! of shadow presentation is equal to theRefId
        static void updatePrsTransformation(PrsMgr_ListOfPresentations thePrsList,
                                        int theRefId,
                                        TopLoc_Datum3D theTrsf)
        {
            foreach (var aPrs in thePrsList)
            {

                if (aPrs == null)
                    continue;

                Prs3d_PresentationShadow aShadowPrs = (aPrs as Prs3d_PresentationShadow);
                if (aShadowPrs == null || aShadowPrs.ParentId() != theRefId)
                    continue;

                aShadowPrs.CStructure().SetTransformation(theTrsf);
            }
        }
        Graphic3d_StructureManager myStructureManager;
        int myImmediateModeOn;
        PrsMgr_ListOfPresentations myImmediateList;
        PrsMgr_ListOfPresentations myViewDependentImmediateList;
        //! Returns the structure manager.
        public Graphic3d_StructureManager StructureManager() { return myStructureManager; }

        public void UpdateHighlightTrsf(V3d_Viewer theViewer,
                                                        PrsMgr_PresentableObject theObj,
                                                        int theMode,
                                                        PrsMgr_PresentableObject theSelObj = null)
        {
            if (theObj == null)
                return;

            PrsMgr_Presentation aPrs = Presentation(theSelObj != null ? theSelObj : theObj, theMode, false);
            if (aPrs == null)
            {
                return;
            }

            TopLoc_Datum3D aTrsf = theObj.LocalTransformationGeom();
            int aParentId = aPrs.CStructure().Identification();
            updatePrsTransformation(myImmediateList, aParentId, aTrsf);

            if (!myViewDependentImmediateList.IsEmpty())
            {
                //for (V3d_ListOfViewIterator anActiveViewIter = (theViewer.ActiveViewIterator()); anActiveViewIter.More(); anActiveViewIter.Next())
                foreach (var anActiveViewIter in theViewer.myActiveViews)
                {
                    Graphic3d_CView aView = anActiveViewIter.View();
                    Graphic3d_Structure aViewDepParentPrs = new Graphic3d_Structure();
                    if (aView.IsComputed(aParentId, ref aViewDepParentPrs))
                    {
                        updatePrsTransformation(myViewDependentImmediateList,
                                                 aViewDepParentPrs.CStructure().Identification(),
                                                 aTrsf);
                    }
                }
            }
        }
        public void Connect(PrsMgr_PresentableObject thePrsObject,
                                          PrsMgr_PresentableObject theOtherObject,
                                          int theMode,
                                          int theOtherMode)
        {
            PrsMgr_Presentation aPrs = Presentation(thePrsObject, theMode, true);
            PrsMgr_Presentation aPrsOther = Presentation(theOtherObject, theOtherMode, true);
            aPrs.Connect(aPrsOther, Graphic3d_TypeOfConnection.Graphic3d_TOC_DESCENDANT);
        }

        internal void Display(AIS_InteractiveObject thePrsObj, int theMode)
        {

            if (thePrsObj.HasOwnPresentations())
            {
                PrsMgr_Presentation aPrs = Presentation(thePrsObj, theMode, true);
                if (aPrs.MustBeUpdated())
                {
                    Update(thePrsObj, theMode);
                }

                if (myImmediateModeOn > 0)
                {
                    AddToImmediateList(aPrs);
                }
                else
                {
                    aPrs.Display();
                }
            }
            else
            {
                thePrsObj.Compute(this, new Prs3d_Presentation(), theMode);
            }

            /*if (thePrsObj.ToPropagateVisualState())
			{
				for (PrsMgr_ListOfPresentableObjectsIter anIter(thePrsObj.Children()); anIter.More(); anIter.Next())
				{
					PrsMgr_PresentableObject aChild = anIter.Value();
					if (aChild.DisplayStatus() != PrsMgr_DisplayStatus_Erased)
					{
						Display(anIter.Value(), theMode);
					}
				}
			}*/

        }

        private void Update(AIS_InteractiveObject thePrsObj, int theMode)
        {
            throw new NotImplementedException();
        }

        //! Returns the presentation Presentation of the presentable object thePrsObject in this framework.
        //! When theToCreate is true - automatically creates presentation for specified mode when not exist.
        //! Optional argument theSelObj specifies parent decomposed object to inherit its view affinity.
        private PrsMgr_Presentation Presentation(PrsMgr_PresentableObject thePrsObj, int theMode = 0,
             bool theToCreate = false,
             PrsMgr_PresentableObject theSelObj = null)
        {
            PrsMgr_Presentations aPrsList = thePrsObj.Presentations();
            for (PrsMgr_Presentations.Iterator aPrsIter = new PrsMgr_Presentations.Iterator(aPrsList); aPrsIter.More(); aPrsIter.Next())
            {
                PrsMgr_Presentation aPrs2 = aPrsIter.Value();
                PrsMgr_PresentationManager aPrsMgr = aPrs2.PresentationManager();
                if (theMode == aPrs2.Mode()
                 && this == aPrsMgr)
                {
                    return aPrs2;
                }
            }

            if (!theToCreate)
            {
                return null;
            }

            PrsMgr_Presentation aPrs = new PrsMgr_Presentation(this, thePrsObj, theMode);
            aPrs.SetZLayer(thePrsObj.ZLayer());
            aPrs.CStructure().ViewAffinity = theSelObj != null ? theSelObj.ViewAffinity() : thePrsObj.ViewAffinity();
            thePrsObj.Presentations().Append(aPrs);
            thePrsObj.Fill(this, aPrs, theMode);

            // set layer index accordingly to object's presentations
            aPrs.SetUpdateStatus(false);
            return aPrs;

        }

        private void AddToImmediateList(PrsMgr_Presentation aPrs)
        {
            throw new NotImplementedException();
        }

        internal bool IsHighlighted(AIS_InteractiveObject theIObj, int anOldMode)
        {
            throw new NotImplementedException();
        }

        public void SetVisibility(PrsMgr_PresentableObject thePrsObj, int theMode, bool theValue)
        {
            if (thePrsObj.ToPropagateVisualState())
            {
                for (PrsMgr_ListOfPresentableObjectsIter anIter = new PrsMgr_ListOfPresentableObjectsIter(thePrsObj.Children()); anIter.More(); anIter.Next())
                {
                    PrsMgr_PresentableObject aChild = anIter.Value();
                    if (!theValue
                      || aChild.DisplayStatus() != PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_Erased)
                    {
                        SetVisibility(anIter.Value(), theMode, theValue);
                    }
                }
            }
            if (!thePrsObj.HasOwnPresentations())
            {
                return;
            }

            PrsMgr_Presentation aPrs = Presentation(thePrsObj, theMode);
            if (aPrs != null)
            {
                aPrs.SetVisible(theValue);
            }
        }

        //! returns True if the immediate display has been done.
        public void EndImmediateDraw(V3d_Viewer theViewer)
        {
            if (--myImmediateModeOn > 0)
            {
                return;
            }

            displayImmediate(theViewer);
        }
        // =======================================================================
        // function : displayImmediate
        // purpose  : Handles the structures from myImmediateList and its visibility
        //            in all views of the viewer given by setting proper affinity
        // =======================================================================
        void displayImmediate(V3d_Viewer theViewer)
        {
            for (V3d_ListOfViewIterator anActiveViewIter = new V3d_ListOfViewIterator(theViewer.ActiveViewIterator()); anActiveViewIter.More(); anActiveViewIter.Next())
            {
                Graphic3d_CView aView = anActiveViewIter.Value().View();

                for (PrsMgr_ListOfPresentations.Iterator anIter = new TKernel.NCollection_List<Graphic3d_Structure>.Iterator(myImmediateList); anIter.More(); anIter.Next())
                {
                    Prs3d_Presentation aPrs = anIter.Value();
                    if (aPrs == null)
                        continue;

                    Graphic3d_Structure aViewDepPrs = new Graphic3d_Structure();
                    Prs3d_PresentationShadow aShadowPrs = (Prs3d_PresentationShadow)(aPrs);
                    if (aShadowPrs != null && aView.IsComputed(aShadowPrs.ParentId(), ref aViewDepPrs))
                    {
                        Graphic3d_ZLayerId aZLayer = aShadowPrs.GetZLayer();
                        aShadowPrs = null;

                        /*aShadowPrs = new Prs3d_PresentationShadow(myStructureManager, aViewDepPrs);
                        aShadowPrs.SetZLayer(aZLayer);
                        aShadowPrs.SetClipPlanes(aViewDepPrs->ClipPlanes());
                        aShadowPrs.CStructure().IsForHighlight = 1;
                        aShadowPrs.Highlight(aPrs.HighlightStyle());*/
                        myViewDependentImmediateList.Append(aShadowPrs);
                    }
                    // handles custom highlight presentations which were defined in overridden
                    // HilightOwnerWithColor method of a custom AIS objects and maintain its
                    // visibility in different views on their own
                    else if (aShadowPrs == null)
                    {
                        aPrs.Display();
                        continue;
                    }

                    if (!aShadowPrs.IsDisplayed())
                    {
                        aShadowPrs.CStructure().ViewAffinity = new Graphic3d_ViewAffinity();
                        aShadowPrs.CStructure().ViewAffinity.SetVisible(false);
                        aShadowPrs.Display();
                    }

                    int aViewId = aView.Identification();
                    bool isParentVisible = aShadowPrs.ParentAffinity() == null ?
                      true : aShadowPrs.ParentAffinity().IsVisible(aViewId);
                    aShadowPrs.CStructure().ViewAffinity.SetVisible(aViewId, isParentVisible);
                }
            }
        }

        internal void BeginImmediateDraw()
        {

            if (++myImmediateModeOn > 1)
                return;

            ClearImmediateDraw();
        }

        void ClearImmediateDraw()
        {
            //for (PrsMgr_ListOfPresentations::Iterator anIter (myImmediateList); anIter.More(); anIter.Next())
            foreach (var anIter in myImmediateList)
            {
                anIter.Erase();

            }

            //for (PrsMgr_ListOfPresentations::Iterator anIter (myViewDependentImmediateList); anIter.More(); anIter.Next())
            foreach (var anIter in myViewDependentImmediateList)
            {
                anIter.Erase();
            }

            myImmediateList.Clear();
            myViewDependentImmediateList.Clear();
        }
    }
}

