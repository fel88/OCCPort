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

        public static gp_Pnt UseLocation(
   gp_Pnt thePnt,
   TopLoc_Location theLoc)
        {
            if (theLoc.IsIdentity())
            {
                return thePnt;
            }

            return thePnt.Transformed(theLoc.Transformation());
        }

        //! Stores the given triangulation into the given face.
        //! @param theFace face to be updated by triangulation.
        //! @param theTriangulation triangulation to be stored into the face.
        public static void AddInFace(TopoDS_Face theFace, Poly_Triangulation theTriangulation)
        {
            TopLoc_Location aLoc = theFace.Location();
            if (!aLoc.IsIdentity())
            {
                gp_Trsf aTrsf = aLoc.Transformation();
                aTrsf.Invert();
                for (int aNodeIter = 1; aNodeIter <= theTriangulation.NbNodes(); ++aNodeIter)
                {
                    gp_Pnt aNode = theTriangulation.Node(aNodeIter);
                    aNode.Transform(aTrsf);
                    theTriangulation.SetNode(aNodeIter, aNode);
                }
            }

            BRep_Builder aBuilder = new BRep_Builder();
            aBuilder.UpdateFace(theFace, theTriangulation);
        }
    }


}