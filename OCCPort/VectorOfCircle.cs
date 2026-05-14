using System;

namespace OCCPort
{
    public class VectorOfCircle : NCollection_Vector<BRepMesh_Circle>
    {
        internal void SetValue(int theIndex, BRepMesh_Circle theCircle)
        {
            //expandV
            while (Count <= theIndex)
            {
                Add(null);
            }

            base[theIndex] = theCircle;
        }
    }
}