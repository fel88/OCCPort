using OCCPort;
using OCCPort.Tester;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace OCCPort
{
	public class BRep_Tool
	{

		public static Poly_Polygon3D nullPolygon3D = new Poly_Polygon3D();
		//=======================================================================
		//function : IsGeometric
		//purpose  : Returns True if <E> is a 3d curve or a curve on
		//           surface.
		//=======================================================================

		public static bool IsGeometric(TopoDS_Edge E)
		{
			// find the representation
			BRep_TEdge TE = (BRep_TEdge)(E.TShape());
			BRep_ListIteratorOfListOfCurveRepresentation itcr = new BRep_ListIteratorOfListOfCurveRepresentation(TE.Curves());

			while (itcr.More())
			{
				BRep_CurveRepresentation cr = itcr.Value();
				if (cr.IsCurve3D())
				{
					BRep_Curve3D GC = cr as BRep_Curve3D;
					if (GC != null && GC.Curve3D() != null)
						return true;
				}
				else if (cr.IsCurveOnSurface()) return true;
				itcr.Next();
			}
			return false;
		}

		public static Poly_Polygon3D Polygon3D(TopoDS_Edge E,
														  TopLoc_Location L)
		{
			// find the representation
			BRep_TEdge TE = (BRep_TEdge)E.TShape();
			//BRep_ListIteratorOfListOfCurveRepresentation itcr(TE->Curves());

			//while (itcr.More())
			//{
			//    const Handle(BRep_CurveRepresentation)&cr = itcr.Value();
			//    if (cr->IsPolygon3D())
			//    {
			//        const BRep_Polygon3D* GC = static_cast <const BRep_Polygon3D*> (cr.get());
			//        L = E.Location() * GC->Location();
			//        return GC->Polygon3D();
			//    }
			//    itcr.Next();
			//}
			//L.Identity();
			return nullPolygon3D;
		}
		//=======================================================================
		//function : Tolerance
		//purpose  : Returns the tolerance for <E>.
		//=======================================================================

		public static double Tolerance(TopoDS_Edge E)
		{
			BRep_TEdge TE = (BRep_TEdge)(E.TShape());
			double p = TE.Tolerance();
			double pMin = Precision.Confusion();
			if (p > pMin)
				return p;


			return pMin;
		}
		//=======================================================================
		//function : Tolerance
		//purpose  : Returns the tolerance of the face.
		//=======================================================================

		public static double Tolerance(TopoDS_Face F)
		{
			BRep_TFace TF = (BRep_TFace)(F.TShape());
			double p = TF.Tolerance();
			double pMin = Precision.Confusion();
			if (p > pMin)
				return p;

			return pMin;
		}

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