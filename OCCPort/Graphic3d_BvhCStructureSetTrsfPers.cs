using System;

namespace OCCPort
{
    //! Set of transformation persistent OpenGl_Structure for building BVH tree.
    //! Provides built-in mechanism to invalidate tree when world view projection state changes.
    //! Due to frequent invalidation of BVH tree the choice of BVH tree builder is made
    //! in favor of BVH linear builder (quick rebuild).
    internal class Graphic3d_BvhCStructureSetTrsfPers : BVH_Set<double>//3
	{
		private Select3D_BVHBuilder3d theBuilder;

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
		//! Indexed map of structures.
		Graphic3d_IndexedMapOfStructure myStructs = new Graphic3d_IndexedMapOfStructure();

	}
}