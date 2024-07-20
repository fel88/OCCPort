using System;

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

        Graphic3d_StructureManager myStructureManager;
        int myImmediateModeOn;
        PrsMgr_ListOfPresentations myImmediateList;
        PrsMgr_ListOfPresentations myViewDependentImmediateList;
        //! Returns the structure manager.
        public Graphic3d_StructureManager StructureManager() { return myStructureManager; }


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

        private PrsMgr_Presentation Presentation(AIS_InteractiveObject thePrsObj,
            int theMode,
            bool theToCreate)
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
}