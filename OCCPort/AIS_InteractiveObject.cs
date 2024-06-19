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



        internal int DisplayMode()
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
                //myDrawer.Link(theCtx.DefaultDrawer());
            }

        }


    }
}