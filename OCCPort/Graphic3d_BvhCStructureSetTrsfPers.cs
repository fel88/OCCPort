using System;

namespace OCCPort
{
	internal class Graphic3d_BvhCStructureSetTrsfPers
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