using System;
using System.Reflection.Metadata;

namespace OCCPort
{
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