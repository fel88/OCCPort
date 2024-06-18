namespace OCCPort
{
	public enum SelectMgr_PickingStrategy
    {

		//Enumeration defines picking strategy - which entities detected by picking line will be accepted, considering selection filters.

		SelectMgr_PickingStrategy_FirstAcceptable,

		//the first detected entity passing selection filter is accepted(e.g.any)
		SelectMgr_PickingStrategy_OnlyTopmost

		//only topmost detected entity passing selection filter is accepted
    }
}