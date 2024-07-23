using OCCPort.Tester;
using OCCPort;
using System;
using System.Runtime.InteropServices;

namespace OCCPort
{
    internal class BRepTools
    {
        internal static void Update(TopoDS_Edge e)
        {

        }

        public static bool Triangulation(TopoDS_Shape theShape,
                                            double theLinDefl,
                                            bool theToCheckFreeEdges)
        {
            TopExp_Explorer anEdgeIter;
            TopLoc_Location aDummyLoc = new TopLoc_Location();
            for (TopExp_Explorer aFaceIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceIter.More(); aFaceIter.Next())
            {
                TopoDS_Face aFace = TopoDS.Face(aFaceIter.Current());
                Poly_Triangulation aTri = BRep_Tool.Triangulation(aFace, ref aDummyLoc);
                if (aTri == null
                 || aTri.Deflection() > theLinDefl)
                {
                    return false;
                }

                //for (anEdgeIter.Init(aFace, TopAbs_ShapeEnum.TopAbs_EDGE); anEdgeIter.More(); anEdgeIter.Next())
                //{
                //    TopoDS_Edge anEdge = TopoDS.Edge(anEdgeIter.Current());
                //    Poly_PolygonOnTriangulation aPoly = BRep_Tool.PolygonOnTriangulation(anEdge, aTri, aDummyLoc);
                //    if (aPoly == null)
                //    {
                //        return false;
                //    }
                //}
            }
            if (!theToCheckFreeEdges)
            {
                return true;
            }

            Poly_Triangulation anEdgeTri = null;
            //for (anEdgeIter.Init(theShape, TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE); anEdgeIter.More(); anEdgeIter.Next())
            //{
            //    TopoDS_Edge anEdge = TopoDS.Edge(anEdgeIter.Current());
            //    Poly_Polygon3D aPolygon = BRep_Tool.Polygon3D(anEdge, aDummyLoc);
            //    if (aPolygon != null)
            //    {
            //        if (aPolygon.Deflection() > theLinDefl)
            //        {
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        Poly_PolygonOnTriangulation aPoly = BRep_Tool.PolygonOnTriangulation(anEdge, anEdgeTri, aDummyLoc);
            //        if (aPoly == null
            //         || anEdgeTri == null
            //         || anEdgeTri.Deflection() > theLinDefl)
            //        {
            //            return false;
            //        }
            //    }
            //}

            return true;
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