using System;
using System.Reflection.Metadata;

namespace OCCPort
{
    public class Select3D_SensitiveEntity
    {
        SelectMgr_EntityOwner myOwnerId;
        int mySFactor;

        //! Sets owner of the entity
        public virtual void Set(SelectMgr_EntityOwner theOwnerId)
        {
            myOwnerId = theOwnerId;
        }
        //! Clears up all resources and memory
        public virtual void Clear() { Set(null); }

        //! allows a better sensitivity for a specific entity in selection algorithms useful for small sized entities.
        public int SensitivityFactor() { return mySFactor; }
        //! Allows to manage sensitivity of a particular sensitive entity
        public void SetSensitivityFactor(int theNewSens)
        {
            new Standard_ASSERT_RAISE(theNewSens >= 0, "Error! Selection sensitivity should not be negative value.");
            mySFactor = theNewSens;
        }

    }
}