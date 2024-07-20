namespace OCCPort
{
	internal class BVH_Object : IBVH_Object
	{  //! Marks object state as outdated (needs BVH rebuilding).
		public virtual void MarkDirty() { myIsDirty = true; }
		bool myIsDirty;

	}
}