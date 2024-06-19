namespace OCCPort
{
    public enum SelectMgr_TypeOfBVHUpdate
    {
        /*Keeps track for BVH update state for each SelectMgr_Selection entity in a following way:


	Add : 2nd level BVH does not contain any of the selection's sensitive entities and they must be added;

	Remove : all sensitive entities of the selection must be removed from 2nd level BVH;
		Renew : 2nd level BVH already contains sensitives of the selection, but the its complete update and removal is required.Therefore, sensitives of the selection with this type of update must be removed from 2nd level BVH and added after recomputation.
	Invalidate : the 2nd level BVH needs to be rebuilt;
		None : entities of the selection are up to date.

Enumerator*/
        SelectMgr_TBU_Add,
        SelectMgr_TBU_Remove,
        SelectMgr_TBU_Renew,
        SelectMgr_TBU_Invalidate,
        SelectMgr_TBU_None
    }
}