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

		public int Mode()
        {
			return myMode;
        }

		public bool MustBeUpdated() { return myMustBeUpdated; }
		enum BeforeHighlightState
		{
			State_Empty,
			State_Hidden,
			State_Visible
		};

		public override void Display()
		{

			display(false);
			myBeforeHighlightState = (int)BeforeHighlightState.State_Visible;

		}
		//=======================================================================
		//function : display
		//purpose  :
		//=======================================================================
		void display(bool theIsHighlight)
		{
			if (!base.IsDisplayed())
			{
				base.SetIsForHighlight(theIsHighlight); // optimization - disable frustum culling for this presentation
				base.Display();
			}
			else if (!base.IsVisible())
			{
				base.SetVisible(true);
			}
		}


		int myBeforeHighlightState;
		int myMode;
		bool myMustBeUpdated;

		public void SetUpdateStatus(bool theUpdateStatus) { myMustBeUpdated = theUpdateStatus; }

		//! returns the PresentationManager in which the presentation has been created.
		public PrsMgr_PresentationManager PresentationManager() { return myPresentationManager; }

    }
}