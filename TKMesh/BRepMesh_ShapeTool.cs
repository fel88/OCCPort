using OCCPort;
using OCCPort.Common;
using TKBRep;
using TKG3d;
using TKMath;
using TKShHealing;

namespace TKMesh
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

        public static double MaxFaceTolerance(TopoDS_Face theFace)
        {
            double aMaxTolerance = BRep_Tool.Tolerance(theFace);

            double aTolerance = Math.Max(
              MaxTolerance(TopAbs_ShapeEnum.TopAbs_EDGE, theFace, new EdgeTolerance()),
              MaxTolerance(TopAbs_ShapeEnum.TopAbs_VERTEX, theFace, new VertexTolerance()));

            return Math.Max(aMaxTolerance, aTolerance);
        }

        //! Auxiliary struct to take a tolerance of vertex.
        public class VertexTolerance : IToleranceExtractor
        {
            public double Get(TopoDS_Shape theVertex)
            {
                return BRep_Tool.Tolerance(TopoDS.Vertex(theVertex));
            }
        }

        //! Auxiliary struct to take a tolerance of edge.
        public class EdgeTolerance : IToleranceExtractor
        {
            public double Get(TopoDS_Shape theEdge)
            {
                return BRep_Tool.Tolerance(TopoDS.Edge(theEdge));
            }
        }

        public interface IToleranceExtractor
        {
            double Get(TopoDS_Shape topoDS_Shape);
        }

        //! Returns maximum tolerance of face element of the specified type.

      public   static double MaxTolerance(TopAbs_ShapeEnum ShapeType, TopoDS_Face theFace, IToleranceExtractor toleranceExtractor)
        {
            double aMaxTolerance = Standard_Real.RealFirst();
            TopExp_Explorer aExplorer = new TopExp_Explorer(theFace, ShapeType);
            for (; aExplorer.More(); aExplorer.Next())
            {
                double aTolerance = toleranceExtractor.Get(aExplorer.Current());
                if (aTolerance > aMaxTolerance)
                    aMaxTolerance = aTolerance;
            }

            return aMaxTolerance;
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

        public static bool Range(
   TopoDS_Edge theEdge,
 out Geom_Curve theCurve,
ref double theFirstParam,
ref double theLastParam,
   bool isConsiderOrientation = false)
        {

            ShapeAnalysis_Edge aEdge = new ShapeAnalysis_Edge();
            return aEdge.Curve3d(theEdge, out theCurve,
            ref theFirstParam, ref theLastParam,
              isConsiderOrientation);
        }

        internal static void CheckAndUpdateFlags(IMeshData_Edge theEdge, IMeshData_PCurve thePCurve)
        {
            if (!theEdge.GetSameParam() &&
       !theEdge.GetSameRange() &&
        theEdge.GetDegenerated())
            {
                // Nothing to do worse.
                return;
            }

            TopoDS_Edge aEdge = theEdge.GetEdge();
            TopoDS_Face aFace = thePCurve.GetFace().GetFace();

            Geom_Curve aCurve;
            double aFirstParam = 0, aLastParam = 0;
            Range(aEdge, out aCurve, ref aFirstParam, ref aLastParam);
            if (aCurve == null)
            {
                theEdge.SetDegenerated(true);
                return;
            }

            BRepAdaptor_Curve aCurveOnSurf = new BRepAdaptor_Curve(aEdge, aFace);
            if (theEdge.GetSameParam() || theEdge.GetSameRange())
            {
                if (theEdge.GetSameRange())
                {
                    double aDiffFirst = aCurveOnSurf.FirstParameter() - aFirstParam;
                    double aDiffLast = aCurveOnSurf.LastParameter() - aLastParam;
                    theEdge.SetSameRange(
                     Math.Abs(aDiffFirst) < Precision.PConfusion() &&
                     Math.Abs(aDiffLast) < Precision.PConfusion());

                    if (!theEdge.GetSameRange())
                    {
                        theEdge.SetSameParam(false);
                    }
                }
            }

            if (!theEdge.GetDegenerated()/* || theEdge->GetSameParam ()*/)
            {
                TopoDS_Vertex aStartVertex = new TopoDS_Vertex(), aEndVertex = new TopoDS_Vertex();
                TopExp.Vertices(aEdge, ref aStartVertex, ref aEndVertex);
                if (aStartVertex.IsNull() || aEndVertex.IsNull())
                {
                    theEdge.SetDegenerated(true);
                    return;
                }

                if (aStartVertex.IsSame(aEndVertex))
                {
                    int aPointsNb = 20;
                    double aVertexTolerance = BRep_Tool.Tolerance(aStartVertex);
                    double aDu = (aLastParam - aFirstParam) / aPointsNb;
                    //const Standard_Real    aEdgeTolerance     = BRep_Tool::Tolerance (aEdge);
                    //const Standard_Real    aSqEdgeTolerance   = aEdgeTolerance * aEdgeTolerance;

                    gp_Pnt aPrevPnt = new gp_Pnt();
                    aCurve.D0(aFirstParam, ref aPrevPnt);

                    double aLength = 0.0;
                    for (int i = 1; i <= aPointsNb; ++i)
                    {
                        double aParameter = aFirstParam + i * aDu;
                        // Calculation of the length of the edge in 3D
                        // in order to check degenerativity
                        gp_Pnt aPnt = new gp_Pnt();
                        aCurve.D0(aParameter, ref aPnt);
                        aLength += aPrevPnt.Distance(aPnt);

                        //if (theEdge->GetSameParam ())
                        //{
                        //  // Check that points taken at the 3d and pcurve using 
                        //  // same parameter are within tolerance of an edge.
                        //  gp_Pnt aPntOnSurf;
                        //  aCurveOnSurf.D0 (aParameter, aPntOnSurf);
                        //  theEdge->SetSameParam (aPnt.SquareDistance (aPntOnSurf) < aSqEdgeTolerance);
                        //}

                        if (aLength > aVertexTolerance /*&& !theEdge->GetSameParam()*/)
                        {
                            break;
                        }

                        aPrevPnt = aPnt;
                    }

                    theEdge.SetDegenerated(aLength < aVertexTolerance);
                }
            }
        }
        public static void NullifyEdge(
   TopoDS_Edge theEdge,
   ref TopLoc_Location theLocation)
        {
            BRep_Builder aBuilder = new BRep_Builder();
            //aBuilder.UpdateEdge(theEdge, null, ref theLocation);
        }

        internal static void NullifyEdge(TopoDS_Edge theEdge, Poly_Triangulation theTriangulation, ref TopLoc_Location theLocation)
        {

            /*  UpdateEdge(theEdge, null,
                theTriangulation, ref theLocation);*/

        }
        //! Updates the given seam edge by the given tessellated representations.
        //! @param theEdge edge to be updated.
        //! @param thePolygon1 tessellated representation corresponding to
        //! forward direction of the seam edge.
        //! @param thePolygon2 tessellated representation corresponding to
        //! reversed direction of the seam edge.
        //! @param theTriangulation triangulation the given edge is associated to.
        //! @param theLocation face location.
        public static void UpdateEdge(
    TopoDS_Edge theEdge,
    Poly_PolygonOnTriangulation thePolygon1,
    Poly_PolygonOnTriangulation thePolygon2,
    Poly_Triangulation theTriangulation,
    ref TopLoc_Location theLocation)
        {
            BRep_Builder aBuilder = new BRep_Builder();
            aBuilder.UpdateEdge(theEdge, thePolygon1, thePolygon2,
              theTriangulation, theLocation);
        }

        //! Updates the given edge by the given tessellated representation.
        //! @param theEdge edge to be updated.
        //! @param thePolygon tessellated representation of the edge to be stored.
        public static void UpdateEdge(
  TopoDS_Edge theEdge,
  Poly_Polygon3D thePolygon)
        {
            BRep_Builder aBuilder = new BRep_Builder();
            aBuilder.UpdateEdge(theEdge, thePolygon);
        }

        public static void UpdateEdge(
      TopoDS_Edge theEdge,
      Poly_PolygonOnTriangulation thePolygon,
      Poly_Triangulation theTriangulation,
    ref TopLoc_Location theLocation)
        {
            BRep_Builder aBuilder = new BRep_Builder();
            //   aBuilder.UpdateEdge(theEdge, thePolygon, theTriangulation, theLocation);
        }

        internal static void NullifyFace(TopoDS_Face theFace)
        {

            BRep_Builder aBuilder = new BRep_Builder();
            aBuilder.UpdateFace(theFace, null);

        }
    }
}


