using System;

namespace OCCPort
{
    //! Auxiliary class providing functionality to compute,
    //! retrieve and store data to TopoDS and model shape.
    public class BRepMesh_ShapeTool
    {
        //! Gets the maximum dimension of the given bounding box.
        //! If the given bounding box is void leaves the resulting value unchanged.
        //! @param theBox bounding box to be processed.
        //! @param theMaxDimension maximum dimension of the given box.
        public static void BoxMaxDimension(Bnd_Box theBox,
                                             ref double theMaxDimension)
        {
            if (theBox.IsVoid())
                return;

            double aMinX, aMinY, aMinZ, aMaxX, aMaxY, aMaxZ;
            theBox.Get(out aMinX, out aMinY, out aMinZ, out aMaxX, out aMaxY, out aMaxZ);

            theMaxDimension = Math.Max(aMaxX - aMinX, Math.Max(aMaxY - aMinY, aMaxZ - aMinZ));
        }
    }


}