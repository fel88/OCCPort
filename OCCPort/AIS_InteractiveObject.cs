using System;

namespace OCCPort
{
	public abstract class AIS_InteractiveObject : SelectMgr_SelectableObject
    {
        public AIS_InteractiveObject(PrsMgr_TypeOfPresentation3d aTypeOfPresentation3d = PrsMgr_TypeOfPresentation3d.PrsMgr_TOP_AllView)
            : base(aTypeOfPresentation3d)
        {

            myCTXPtr = null;
        }
        AIS_InteractiveContext myCTXPtr; //!< pointer to Interactive Context, where object is currently displayed; @sa SetContext()
        object myOwner;  //!< application-specific owner object

        internal bool AcceptDisplayMode(object value)
        {
            throw new NotImplementedException();
        }

        internal int DisplayMode()
        {
            throw new NotImplementedException();
        }

        internal int GlobalSelectionMode()
        {
            throw new NotImplementedException();
        }

        

        internal bool HasHilightMode()
        {
            throw new NotImplementedException();
        }

        internal int HilightMode()
        {
            throw new NotImplementedException();
        }

        internal void SetDisplayStatus(PrsMgr_DisplayStatus prsMgr_DisplayStatus_Displayed)
        {
            throw new NotImplementedException();
        }

        internal bool HasInteractiveContext()
        {
            throw new NotImplementedException();
        }

        internal void SetContext(AIS_InteractiveContext aIS_InteractiveContext)
        {
            throw new NotImplementedException();
        }

        internal object Children()
        {
            throw new NotImplementedException();
        }
    }
}