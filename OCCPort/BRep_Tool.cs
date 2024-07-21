using OCCPort;
using OCCPort.Tester;
using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace OCCPort
{
    public class BRep_Tool
    {

        public static bool IsClosed(TopoDS_Shape theShape)
        {
            if (theShape.ShapeType() == TopAbs_ShapeEnum.TopAbs_SHELL)
            {
                //Dictionary<TopoDS_Shape, TopTools_ShapeMapHasher> aMap(101, new NCollection_IncAllocator);
                NCollection_Map aMap = new NCollection_Map();
                TopExp_Explorer exp = new TopExp_Explorer(theShape.Oriented(TopAbs_Orientation.TopAbs_FORWARD), TopAbs_ShapeEnum.TopAbs_EDGE);
                bool hasBound = false;
                for (; exp.More(); exp.Next())
                {
                    TopoDS_Edge E = TopoDS.Edge(exp.Current());
                    if (BRep_Tool.Degenerated(E) || E.Orientation() == TopAbs_Orientation.TopAbs_INTERNAL || E.Orientation() == TopAbs_Orientation.TopAbs_EXTERNAL)
                        continue;
                    hasBound = true;
                    if (!aMap.Add(E))
                        aMap.Remove(E);
                }
                return hasBound && aMap.IsEmpty();
            }
            else if (theShape.ShapeType() == TopAbs_ShapeEnum.TopAbs_WIRE)
            {
                //NCollection_Map<TopoDS_Shape, TopTools_ShapeMapHasher> aMap(101, new NCollection_IncAllocator);
                NCollection_Map aMap = new NCollection_Map();
                TopExp_Explorer exp = new TopExp_Explorer(theShape.Oriented(TopAbs_Orientation.TopAbs_FORWARD), TopAbs_ShapeEnum.TopAbs_VERTEX);
                bool hasBound = false;
                for (; exp.More(); exp.Next())
                {
                    TopoDS_Shape V = exp.Current();
                    if (V.Orientation() == TopAbs_Orientation.TopAbs_INTERNAL || V.Orientation() == TopAbs_Orientation.TopAbs_EXTERNAL)
                        continue;
                    hasBound = true;
                    if (!aMap.Add(V))
                        aMap.Remove(V);
                }
                return hasBound && aMap.IsEmpty();
            }
            else if (theShape.ShapeType() == TopAbs_ShapeEnum.TopAbs_EDGE)
            {
                TopoDS_Vertex aVFirst, aVLast;
                TopExp.Vertices(TopoDS.Edge(theShape), out aVFirst, out aVLast);
                return !aVFirst.IsNull() && aVFirst.IsSame(aVLast);
            }
            return theShape.Closed();
        }

        private static bool Degenerated(TopoDS_Edge e)
        {
            BRep_TEdge TE = e.TShape() as BRep_TEdge;

            //const BRep_TEdge* TE = static_cast <const BRep_TEdge*> (E.TShape().get());
            return TE.Degenerated();
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

        public static Poly_Triangulation Triangulation(TopoDS_Face theFace,
            ref TopLoc_Location theLocation,
            Poly_MeshPurpose theMeshPurpose = Poly_MeshPurpose.Poly_MeshPurpose_NONE)
        {
            theLocation = theFace.Location();
            var aTFace = theFace.TShape() as BRep_TFace;
            //const BRep_TFace* aTFace = static_cast <const BRep_TFace*> (theFace.TShape().get());
            return aTFace.Triangulation(theMeshPurpose);
        }

        internal static Geom_Surface Surface(TopoDS_Face aFace, TopLoc_Location aDummyLoc)
        {
            throw new NotImplementedException();
        }

        internal static Poly_PolygonOnTriangulation PolygonOnTriangulation(TopoDS_Edge anEdge, Poly_Triangulation aTriangulation, TopLoc_Location aTrsf)
        {
            throw new NotImplementedException();
        }

        internal static GeomAbs_Shape MaxContinuity(TopoDS_Edge theEdge)
        {
            GeomAbs_Shape aMaxCont = GeomAbs_Shape.GeomAbs_C0;
            var curves = ((BRep_TEdge)theEdge.TShape()).ChangeCurves();
            //for (BRep_ListIteratorOfListOfCurveRepresentation aReprIter ((*((Handle(BRep_TEdge) *) & theEdge.TShape()))->ChangeCurves());
            //  aReprIter.More(); aReprIter.Next())
            foreach (var aReprIter in curves.list)
            {
                BRep_CurveRepresentation aRepr = aReprIter;
                if (aRepr.IsRegularity())
                {
                    GeomAbs_Shape aCont = aRepr.Continuity();
                    if ((int)aCont > (int)aMaxCont)
                    {
                        aMaxCont = aCont;
                    }
                }
            }
            return aMaxCont;
        }
    }
}