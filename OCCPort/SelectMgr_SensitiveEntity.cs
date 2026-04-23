using System;
using System.Reflection.Metadata;

namespace OCCPort
{

    //! The purpose of this class is to mark sensitive entities selectable or not
    //! depending on current active selection of parent object for proper BVH traverse
    public class SelectMgr_SensitiveEntity
    {
        Select3D_SensitiveEntity mySensitive;      //!< Related SelectBasics entity
        public SelectMgr_SensitiveEntity(Select3D_SensitiveEntity theEntity)
        {
            mySensitive = (theEntity);
            myIsActiveForSelection = (false);

            //
        }

        bool myIsActiveForSelection;       //!< Selection activity status

        //! Returns true if this entity belongs to the active selection
        //! mode of parent object
        public bool IsActiveForSelection() { return myIsActiveForSelection; }

        //! Marks entity as inactive for selection
        public void ResetSelectionActiveStatus() { myIsActiveForSelection = false; }

        //! Marks entity as active for selection
        public void SetActiveForSelection() { myIsActiveForSelection = true; }

        //! Returns related instance of SelectBasics class
        public Select3D_SensitiveEntity BaseSensitive() { return mySensitive; }
        public void Clear()
        {
            mySensitive.Clear();
            mySensitive = null;
        }
    }
}