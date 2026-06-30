namespace TKV3d
{
    //! A framework to define classes of owners of sensitive primitives.
    //! The owner is the link between application and selection data structures.
    //! For the application to make its own objects selectable, it must define owner classes inheriting this framework.
    public class SelectMgr_EntityOwner
    {  //! Implements immediate application of location transformation of parent object to dynamic highlight structure
        public virtual void UpdateHighlightTrsf(V3d_Viewer theViewer,
                                      PrsMgr_PresentationManager theManager,
                                    int theDispMode)
        {
            if (mySelectable != null)
            {
                theManager.UpdateHighlightTrsf(theViewer, mySelectable, theDispMode);
            }
        }

        //! Returns TRUE if this owner points to a part of object and FALSE for entire object.
        public bool ComesFromDecomposition()  { return myFromDecomposition; }

        //! @return Standard_True if the owner is selected.
        public bool IsSelected()  { return myIsSelected; }

        //! Returns true if there is a selectable object to serve as an owner.
        public bool HasSelectable()  { return mySelectable != null; }

        //! if this method returns TRUE the owner will always call method Hilight for SelectableObject when the owner is detected.
        //! By default it always return FALSE.
        public virtual bool IsForcedHilight()  { return false; }
  
        //! Returns a selectable object detected in the working context.
        public SelectMgr_SelectableObject Selectable() { return mySelectable; }


        //! Returns true if pointer to selectable object of this owner is equal to the given one
        public bool IsSameSelectable(SelectMgr_SelectableObject theOther)
        {
            return mySelectable == theOther;
        }

        protected SelectMgr_SelectableObject mySelectable;        //!< raw pointer to selectable object
        protected int mypriority;          //!< selection priority (for result with the same depth)
        protected bool myIsSelected;        //!< flag indicating selected state
        protected bool myFromDecomposition; //!< flag indicating this owner points to a part of object (TRUE) or to entire object (FALSE)

    }
}

