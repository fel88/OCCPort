using OCCPort.Tester;

namespace OCCPort
{
    public abstract class SelectMgr_SelectableObject : PrsMgr_PresentableObject
    {
		public SelectMgr_SelectableObject(PrsMgr_TypeOfPresentation3d aTypeOfPresentation3d)
			: base(aTypeOfPresentation3d)
		{

			myGlobalSelMode = (0);
			myAutoHilight = (true);
		}

		SelectMgr_SequenceOfSelection myselections;    //!< list of selections
		Prs3d_Presentation mySelectionPrs;  //!< optional presentation for highlighting selected object
		Prs3d_Presentation myHilightPrs;    //!< optional presentation for highlighting detected object
		int myGlobalSelMode; //!< global selection mode
		bool myAutoHilight;   //!< auto-highlighting flag defining

		//! Returns the mode for selection of object as a whole; 0 by default.
		public  int GlobalSelectionMode()  { return myGlobalSelMode; }

		
    }
}