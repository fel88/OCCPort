using OCCPort.Interfaces;
using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace OCCPort
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
                        aCurve.D0(aParameter,ref aPnt);
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
    }


}