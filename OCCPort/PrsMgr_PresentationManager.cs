using System;

namespace OCCPort
{
    public  class PrsMgr_PresentationManager
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
		public  Graphic3d_StructureManager StructureManager()  { return myStructureManager; }


        internal void Display(AIS_InteractiveObject theIObj, int theDispMode)
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