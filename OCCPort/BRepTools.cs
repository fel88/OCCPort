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

        //=======================================================================
        //function : UVBounds
        //purpose  : 
        //=======================================================================
        public static void UVBounds(TopoDS_Face F,
                         ref double UMin, ref double UMax,
                         ref double VMin, ref double VMax)
        {
            Bnd_Box2d B = new Bnd_Box2d();
            AddUVBounds(F, B);
            if (!B.IsVoid())
            {
                B.Get(ref UMin, ref VMin, ref UMax, ref VMax);
            }
            else
            {
                UMin = UMax = VMin = VMax = 0.0;
            }
        }


        public static void AddUVBounds(TopoDS_Face FF, Bnd_Box2d B)
        {
            TopoDS_Face F = FF;
            F.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
            TopExp_Explorer ex = new TopExp_Explorer(F, TopAbs_ShapeEnum.TopAbs_EDGE);

            // fill box for the given face
            Bnd_Box2d aBox = new Bnd_Box2d();
            for (; ex.More(); ex.Next())
            {
                BRepTools.AddUVBounds(F, TopoDS.Edge(ex.Current()), aBox);
            }

            // if the box is empty (face without edges or without pcurves),
            // get natural bounds
            if (aBox.IsVoid())
            {
                double UMin = 0, UMax = 0, VMin = 0, VMax = 0;
                TopLoc_Location L = new TopLoc_Location();
                Geom_Surface aSurf = BRep_Tool.Surface(F, ref L);
                if (aSurf == null)
                {
                    return;
                }

                aSurf.Bounds(ref UMin, ref UMax, ref VMin, ref VMax);
                aBox.Update(UMin, VMin, UMax, VMax);
            }

            // add face box to result
            B.Add(aBox);
        }

        //=======================================================================
        //function : AddUVBounds
        //purpose  : 
        //=======================================================================
        static void AddUVBounds(TopoDS_Face aF,
                             TopoDS_Edge aE,
                            Bnd_Box2d aB)
        {
            double aT1, aT2, aXmin = 0.0, aYmin = 0.0, aXmax = 0.0, aYmax = 0.0;
            double aUmin, aUmax, aVmin, aVmax;
            Bnd_Box2d aBoxC, aBoxS;
            TopLoc_Location aLoc;
            /*Geom2d_Curve aC2D = BRep_Tool.CurveOnSurface(aE, aF, aT1, aT2);
            if (aC2D == null)
            {
                return;
            }*/
        }

        //=======================================================================
        //function : AddUVBounds
        //purpose  : s
        //=======================================================================
        static void AddUVBounds(TopoDS_Face F,
                                TopoDS_Wire W,
                               Bnd_Box2d B)
        {
            TopExp_Explorer ex = new TopExp_Explorer();
            for (ex.Init(W, TopAbs_ShapeEnum.TopAbs_EDGE); ex.More(); ex.Next())
            {
                BRepTools.AddUVBounds(F, TopoDS.Edge(ex.Current()), B);
            }
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