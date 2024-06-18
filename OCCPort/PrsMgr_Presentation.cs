using OCCPort.Tester;
using System;

namespace OCCPort
{
    public class PrsMgr_Presentation : Prs3d_Presentation

    {
        public PrsMgr_Presentation(PrsMgr_PresentationManager thePrsMgr,
                                           PrsMgr_PresentableObject thePrsObject,
                                           int theMode) : base(thePrsMgr.StructureManager())

        {

            myPresentationManager = (thePrsMgr);
            myPresentableObject = (thePrsObject);
            //myBeforeHighlightState=(State_Empty);
            //myMode = (theMode);
            /*myMustBeUpdated = false;
			if (thePrsObject.TypeOfPresentation3d() == PrsMgr_TOP_ProjectorDependent)
			{
				SetVisual(Graphic3d_TOS_COMPUTED);
			}
			SetOwner(myPresentableObject);
			SetMutable(myPresentableObject->IsMutable());*/
        }

        PrsMgr_PresentationManager myPresentationManager;
        PrsMgr_PresentableObject myPresentableObject;
        //=======================================================================
        //function : Compute
        //purpose  :
        //=======================================================================
        public override void Compute()
        {
            int aDispMode = 0;
            for (PrsMgr_Presentations.Iterator aPrsIter = new PrsMgr_Presentations.Iterator(myPresentableObject.myPresentations); aPrsIter.More(); aPrsIter.Next())
            {
                PrsMgr_Presentation aModedPresentation = aPrsIter.Value();
                if (aModedPresentation == this)
                {
                    aDispMode = aModedPresentation.Mode();
                    break;
                }
            }

            myPresentableObject.Compute(myPresentationManager, this, aDispMode);
        }

        private int Mode()
        {
            throw new NotImplementedException();
        }
    }
}