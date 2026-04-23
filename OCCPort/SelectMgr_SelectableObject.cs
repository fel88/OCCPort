using OCCPort;
using OCCPort.Tester;
using System;
using System.Reflection.Metadata;

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

        //! Computes sensitive primitives for the given selection mode - key interface method of Selectable Object.
        //! @param theSelection selection to fill
        //! @param theMode selection mode to create sensitive primitives
        public abstract void ComputeSelection(SelectMgr_Selection theSelection,
                                 int theMode);
        //==================================================
        // Function: RecomputePrimitives
        // Purpose : IMPORTANT: Do not use this method to update
        //           selection primitives except implementing custom
        //           selection manager! This method does not take
        //           into account necessary BVH updates, but may
        //           invalidate the pointers it refers to.
        //           TO UPDATE SELECTION properly from outside classes,
        //           use method UpdateSelection.
        //==================================================
        public void RecomputePrimitives(int theMode)
        {
            SelectMgr_SelectableObject aSelParent = (SelectMgr_SelectableObject)(Parent());
            foreach (var item in myselections)
            {
                SelectMgr_Selection aSel = item;
                if (aSel.Mode() == theMode)
                {
                    aSel.Clear();
                    ComputeSelection(aSel, theMode);
                    aSel.UpdateStatus(SelectMgr_TypeOfUpdate.SelectMgr_TOU_Partial);
                    aSel.UpdateBVHStatus(SelectMgr_TypeOfBVHUpdate.SelectMgr_TBU_Renew);
                    if (theMode == 0 && aSelParent != null)
                    {
                        SelectMgr_EntityOwner anAsmOwner = aSelParent.GetAssemblyOwner();
                        if (anAsmOwner != null)
                        {
                            SetAssemblyOwner(anAsmOwner, theMode);
                        }
                    }
                    return;
                }
            }

            SelectMgr_Selection aNewSel = new SelectMgr_Selection(theMode);
            ComputeSelection(aNewSel, theMode);

            if (theMode == 0 && aSelParent != null)
            {
                SelectMgr_EntityOwner anAsmOwner = aSelParent.GetAssemblyOwner();
                if (anAsmOwner != null)
                {
                    SetAssemblyOwner(anAsmOwner, theMode);
                }
            }

            aNewSel.UpdateStatus(SelectMgr_TypeOfUpdate.SelectMgr_TOU_Partial);
            aNewSel.UpdateBVHStatus(SelectMgr_TypeOfBVHUpdate.SelectMgr_TBU_Add);

            myselections.Append(aNewSel);
        }

        public virtual SelectMgr_EntityOwner GetAssemblyOwner()
        {
            throw null;
        }

        //=======================================================================
        //function : SetAssemblyOwner
        //purpose  : Sets common entity owner for assembly sensitive object entities
        //=======================================================================
        public void SetAssemblyOwner(SelectMgr_EntityOwner theOwner,
                                                   int theMode)
        {
            if (theMode == -1)
            {
                foreach (var item in myselections)
                {
                    SelectMgr_Selection aSel = item;
                    foreach (var ent in aSel.Entities())
                    {

                        ent.BaseSensitive().Set(theOwner);
                    }
                }
                return;
            }

            foreach (var item in myselections)
            {
                SelectMgr_Selection aSel = item;
                if (aSel.Mode() == theMode)
                {
                    foreach (var ent in aSel.Entities())
                    {
                        ent.BaseSensitive().Set(theOwner);
                    }
                    return;
                }
            }
        }
        //! Return the sequence of selections.
        public SelectMgr_SequenceOfSelection Selections() { return myselections; }

        public override void ResetTransformation()
        {
            foreach (var item in myselections)
            {
                SelectMgr_Selection aSel = item;
                aSel.UpdateStatus(SelectMgr_TypeOfUpdate.SelectMgr_TOU_Partial);
                aSel.UpdateBVHStatus(SelectMgr_TypeOfBVHUpdate.SelectMgr_TBU_None);
            }

            base.ResetTransformation();
        }

        SelectMgr_SequenceOfSelection myselections;    //!< list of selections
        Prs3d_Presentation mySelectionPrs;  //!< optional presentation for highlighting selected object
        Prs3d_Presentation myHilightPrs;    //!< optional presentation for highlighting detected object
        int myGlobalSelMode; //!< global selection mode
        bool myAutoHilight;   //!< auto-highlighting flag defining

        //! Returns the mode for selection of object as a whole; 0 by default.
        public int GlobalSelectionMode() { return myGlobalSelMode; }


    }
}