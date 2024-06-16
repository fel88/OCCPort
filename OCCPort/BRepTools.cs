using System;

namespace OCCPort
{
    internal class BRepTools
    {
        internal static void Update(TopoDS_Edge e)
        {

        }



        public static void Update(TopoDS_Face F)
        {
            if (!F.Checked())
            {
                UpdateFaceUVPoints(F);
                F.TShape().Checked(true);
            }
        }

        private static void UpdateFaceUVPoints(TopoDS_Face f)
        {
            throw new NotImplementedException();
        }

        internal static void Update(TopoDS_Shell s)
        {
            throw new NotImplementedException();
        }

        internal static void Update(TopoDS_Wire w)
        {
            throw new NotImplementedException();
        }
    }
}