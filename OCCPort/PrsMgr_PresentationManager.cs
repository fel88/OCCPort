using OpenTK.Graphics.ES20;
using System;
using System.Reflection.Metadata;

namespace OCCPort
{
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
            //aPrs.SetZLayer(thePrsObj.ZLayer());
            //	aPrs.CStructure().ViewAffinity = !theSelObj.IsNull() ? theSelObj->ViewAffinity() : thePrsObj->ViewAffinity();
            thePrsObj.Presentations().Append(aPrs);
            //thePrsObj.Fill(this, aPrs, theMode);

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

        internal void SetVisibility(AIS_InteractiveObject theIObj, int anOldMode, bool v)
        {
            throw new NotImplementedException();
        }
    }


    //! Creates an arbitrary located instance of another Interactive Object,
    //! which serves as a reference.
    //! This allows you to use the Connected Interactive
    //! Object without having to recalculate presentation,
    //! selection or graphic structure. These are deduced
    //! from your reference object.
    //! The relation between the connected interactive object
    //! and its source is generally one of geometric transformation.
    //! AIS_ConnectedInteractive class supports selection mode 0 for any InteractiveObject and
    //! all standard modes if its reference based on AIS_Shape.
    //! Descendants may redefine ComputeSelection() though.
    //! Also ConnectedInteractive will handle HLR if its reference based on AIS_Shape.
    public class AIS_ConnectedInteractive : AIS_InteractiveObject
    {

        AIS_InteractiveObject myReference;
        TopoDS_Shape myShape;
        //! Returns true if there is a connection established
        //! between the presentation and its source reference.
        public bool HasConnection() { return myReference != null; }

        public override void Compute(PrsMgr_PresentationManager thePrsMgr,
            Prs3d_Presentation thePrs, int theMode)
        {
            if (HasConnection())
            {
                thePrs.Clear(false);
                //thePrs.DisconnectAll(Graphic3d_TOC_DESCENDANT);

                if (!myReference.HasInteractiveContext())
                {
                 //   myReference.SetContext(GetContext());
                }
                thePrsMgr.Connect(this, myReference, theMode, theMode);
               // if (thePrsMgr.Presentation(myReference, theMode)->MustBeUpdated())
                {
                    //thePrsMgr.Update(myReference, theMode);
                }
            }

            if (thePrs != null)
            {
                thePrs.ReCompute();
            }
        }

        public override void ComputeSelection(SelectMgr_Selection theSelection, int theMode)
        {
            throw new NotImplementedException();
        }
    }
}