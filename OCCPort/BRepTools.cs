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

        private static void UpdateFaceUVPoints(TopoDS_Face theF)
        {
            // For each edge of the face <F> reset the UV points to the bounding
            // points of the parametric curve of the edge on the face.

            // Get surface of the face
            TopLoc_Location aLoc = new TopLoc_Location();
            Geom_Surface aSurf = BRep_Tool.Surface(theF, ref aLoc);
            // Iterate on edges and reset UV points
            TopExp_Explorer anExpE = new TopExp_Explorer(theF, TopAbs_ShapeEnum.TopAbs_EDGE);
            for (; anExpE.More(); anExpE.Next())
            {
                TopoDS_Edge aE = TopoDS.Edge(anExpE.Current());

                BRep_TEdge TE = (BRep_TEdge)aE.TShape();
                if (TE.Locked())
                    return;

                TopLoc_Location aELoc = aLoc.Predivided(aE.Location()).Clone();
                // Edge representations
                BRep_ListOfCurveRepresentation aLCR = TE.ChangeCurves();
                BRep_ListIteratorOfListOfCurveRepresentation itLCR = new BRep_ListIteratorOfListOfCurveRepresentation(aLCR);
                for (; itLCR.More(); itLCR.Next())
                {
                    BRep_GCurve GC = (itLCR.Value()) as BRep_GCurve;

                    if (GC != null && GC.IsCurveOnSurface(aSurf, aELoc))
                    {
                        // Update UV points
                        GC.Update();
                        break;
                    }
                }
            }

        }

        internal static void Update(TopoDS_Shell s)
        {

        }

        internal static void Update(TopoDS_Wire w)
        {

        }
    }
}