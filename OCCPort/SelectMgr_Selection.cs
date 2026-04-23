using System;
using System.Reflection.Metadata;

namespace OCCPort
{
    public class SelectMgr_Selection
    {

        public SelectMgr_Selection(int theModeIdx = 0)
        {
            myMode = (theModeIdx);
            mySelectionState = SelectMgr_StateOfSelection.SelectMgr_SOS_Unknown;
            myBVHUpdateStatus = SelectMgr_TypeOfBVHUpdate.SelectMgr_TBU_None;
            mySensFactor = 2;
            myIsCustomSens = false;
        }
        public void Add(Select3D_SensitiveEntity theSensitive)
        {
            // if input is null: in debug mode raise exception
            Standard_NullObject_Raise_if(theSensitive==null, "Null sensitive entity is added to the selection");
            if (theSensitive==null)
            {
                // in release mode do not add
                return;
            }

            SelectMgr_SensitiveEntity anEntity = new SelectMgr_SensitiveEntity(theSensitive);
            myEntities.Append(anEntity);
            if (mySelectionState ==SelectMgr_StateOfSelection. SelectMgr_SOS_Activated
            && !anEntity.IsActiveForSelection())
            {
                anEntity.SetActiveForSelection();
            }

            if (myIsCustomSens)
            {
                anEntity.BaseSensitive().SetSensitivityFactor(mySensFactor);
            }
            else
            {
                mySensFactor = Math. Max(mySensFactor, anEntity.BaseSensitive().SensitivityFactor());
            }
        }

        private void Standard_NullObject_Raise_if(bool value, string v)
        {
            if (value)
                throw new Exception(v);
        }

        internal SelectMgr_TypeOfBVHUpdate BVHUpdateStatus()
        {
            return myBVHUpdateStatus;
        }
        SelectMgr_TypeOfUpdate myUpdateStatus;
        NCollection_Vector<SelectMgr_SensitiveEntity> myEntities;

        public void Clear()
        {
            foreach (var item in myEntities)
            {
                item.Clear();
            }

            myEntities.Clear();
        }

        SelectMgr_StateOfSelection mySelectionState;

        int mySensFactor;
        bool myIsCustomSens;

        int myMode;

        //! returns the selection mode represented by this selection
        public int Mode() { return myMode; }

        public void UpdateBVHStatus(SelectMgr_TypeOfBVHUpdate theStatus) { myBVHUpdateStatus = theStatus; }
        SelectMgr_TypeOfBVHUpdate myBVHUpdateStatus;
        //! Return entities.
        public NCollection_Vector<SelectMgr_SensitiveEntity> Entities() { return myEntities; }

        internal SelectMgr_TypeOfUpdate UpdateStatus()
        {
            return myUpdateStatus;
        }
        internal void UpdateStatus(SelectMgr_TypeOfUpdate theStatus)
        {


            myUpdateStatus = theStatus;

        }
    }
}