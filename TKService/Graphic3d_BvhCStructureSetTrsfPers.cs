namespace TKService
{
    //! Set of transformation persistent OpenGl_Structure for building BVH tree.
    //! Provides built-in mechanism to invalidate tree when world view projection state changes.
    //! Due to frequent invalidation of BVH tree the choice of BVH tree builder is made
    //! in favor of BVH linear builder (quick rebuild).
    public class Graphic3d_BvhCStructureSetTrsfPers : BVH_Set<double>//3
    {
        private Select3D_BVHBuilder3d theBuilder;

        //! Cleans the whole primitive set.
        public void Clear()
        {
            myStructs.Clear();
            MarkDirty();
        }
        //! Marks object state as outdated (needs BVH rebuilding).
        void MarkDirty()
        {
            myIsDirty = true;
        }
        //! Marks internal object state as outdated.
        bool myIsDirty;

        public Graphic3d_BvhCStructureSetTrsfPers(Select3D_BVHBuilder3d theBuilder)
        {
            this.theBuilder = theBuilder;
        }

        internal void Add(Graphic3d_CStructure theStruct)
        {
            throw new NotImplementedException();
        }

        internal int Size()
        {
            return myStructs.Size();
        }

        internal bool Remove(Graphic3d_CStructure theStruct)
        {
            //todo
            return true;
        }

        //! Indexed map of structures.
        Graphic3d_IndexedMapOfStructure myStructs = new Graphic3d_IndexedMapOfStructure();

    }
}


