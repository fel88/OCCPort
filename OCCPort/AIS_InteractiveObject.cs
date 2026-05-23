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

        internal void SetDisplayStatus(PrsMgr_DisplayStatus theStatus)
        {
            myDisplayStatus = theStatus;
        }

        //! Indicates whether the Interactive Object has a pointer to an interactive context.
        public bool HasInteractiveContext() { return myCTXPtr != null; }


        internal void SetContext(AIS_InteractiveContext theCtx)
        {
            if (myCTXPtr == theCtx)
            {
                return;
            }

            myCTXPtr = theCtx;
            if (theCtx != null)
            {
                myDrawer.Link(theCtx.DefaultDrawer());
            }

        }


    }
}