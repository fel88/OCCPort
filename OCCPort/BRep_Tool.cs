using System;

namespace OCCPort
{
    internal class BRep_Tool
    {
        internal static bool IsClosed(TopoDS_Shell myShell)
        {
            throw new NotImplementedException();
        }

        internal static bool IsClosed(TopoDS_Wire w)
        {
            throw new NotImplementedException();
        }

        internal static double Surface(double f, TopLoc_Location l)
        {
            throw new NotImplementedException();
        }

        internal static Geom_Surface Surface(TopoDS_Face F,
           ref TopLoc_Location L)
        {
            BRep_TFace TF = (BRep_TFace)(F.TShape());
            L = F.Location() * TF.Location();
            return TF.Surface();

        }
    }
}