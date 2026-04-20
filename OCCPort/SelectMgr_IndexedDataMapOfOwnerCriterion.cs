using System;

namespace OCCPort
{
    internal class SelectMgr_IndexedDataMapOfOwnerCriterion: NCollection_IndexedDataMap<SelectMgr_EntityOwner,SelectMgr_SortCriterion>
    {
        internal SelectMgr_SortCriterion FindFromIndex(int anOwnerIdx)
        {
            throw new NotImplementedException();
        }
        //typedef NCollection_IndexedDataMap<Handle(SelectMgr_EntityOwner), SelectMgr_SortCriterion, TColStd_MapTransientHasher> SelectMgr_IndexedDataMapOfOwnerCriterion;


    
        internal SelectMgr_EntityOwner FindKey(int anOwnerIdx)
        {
            throw new NotImplementedException();
        }
    }
}