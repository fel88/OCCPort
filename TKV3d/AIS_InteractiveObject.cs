namespace TKV3d
{
    //! Defines a class of objects with display and selection services.
    //! Entities which are visualized and selected are Interactive Objects.
    //! Specific attributes of entities such as arrow aspect for dimensions must be loaded in a Prs3d_Drawer.
    //!
    //! You can make use of classes of standard Interactive Objects for which all necessary methods have already been programmed,
    //! or you can implement your own classes of Interactive Objects.
    //! Key interface methods to be implemented by every Interactive Object:
    //! * Presentable Object (PrsMgr_PresentableObject)
    //!   Consider defining an enumeration of supported Display Mode indexes for particular Interactive Object or class of Interactive Objects.
    //!   - AcceptDisplayMode() accepting display modes implemented by this object;
    //!   - Compute() computing presentation for the given display mode index;
    //! * Selectable Object (SelectMgr_SelectableObject)
    //!   Consider defining an enumeration of supported Selection Mode indexes for particular Interactive Object or class of Interactive Objects.
    //!   - ComputeSelection() computing selectable entities for the given selection mode index.
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

