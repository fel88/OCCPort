using System;

namespace OCCPort
{
    internal class Graphic3d_BvhCStructureSet : BVH_PrimitiveSet3d
    {
        internal bool Add(Graphic3d_CStructure theStruct)
        {

            int aSize = myStructs.Size();

            if (myStructs.Add(theStruct) > aSize) // new structure?
            {
                MarkDirty();

                return true;
            }

            return false;

        }
        public void Clear()
        {
            myStructs.Clear();
            MarkDirty();
        }

        Graphic3d_IndexedMapOfStructure myStructs = new Graphic3d_IndexedMapOfStructure();    //!< Indexed map of structures.

    }
}