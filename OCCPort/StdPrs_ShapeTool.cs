using OCCPort;
using System.Reflection.Metadata;
using System.Security.AccessControl;

namespace OCCPort.Tester
{
    //! Describes the behaviour requested for a wireframe shape presentation.
    public class StdPrs_ShapeTool
    {
        public  bool IsPlanarFace()
        {
            TopoDS_Face aFace = TopoDS.Face(myFaceExplorer.Current());
            return IsPlanarFace(aFace);
        }

        public static bool IsPlanarFace(TopoDS_Face theFace)
        {
            TopLoc_Location l;
            Geom_Surface S = BRep_Tool.Surface(theFace, out l);
            if (S == null)
            {
                return false;
            }

            var TheType = S.DynamicType();

            if (TheType == typeof(Geom_RectangularTrimmedSurface))
            {
                Geom_RectangularTrimmedSurface RTS = (Geom_RectangularTrimmedSurface)S;
                TheType = RTS.BasisSurface().DynamicType();
            }
            return (TheType == typeof(Geom_Plane));
        }

         TopExp_Explorer myFaceExplorer;
    }
}