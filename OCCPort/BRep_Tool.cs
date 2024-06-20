using OCCPort;
using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace OCCPort
{
    internal class BRep_Tool
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

		internal static Poly_Triangulation Triangulation(TopoDS_Face aFace, TopLoc_Location aLocation)
		{
			throw new NotImplementedException();
		}

        internal static Geom_Surface Surface(TopoDS_Face aFace, TopLoc_Location aDummyLoc)
        {
            throw new NotImplementedException();
        }
    }
}