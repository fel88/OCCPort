namespace TKMath
{
    public interface IBVH_Object
    {
        //! Marks object state as outdated (needs BVH rebuilding).
        void MarkDirty();

    }
}