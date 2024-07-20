namespace OCCPort
{
	internal interface IBVH_Object
	{
		//! Marks object state as outdated (needs BVH rebuilding).
		void MarkDirty();

	}

}