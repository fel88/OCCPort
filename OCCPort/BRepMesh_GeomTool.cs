using OCCPort;
using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Transactions;

namespace OCCPort
{
    public class BRepMesh_GeomTool
    {

        //=============================================================================
        //function : classifyPoint
        //purpose  : 
        //=============================================================================
      static  int classifyPoint(
  gp_XY thePoint1,
  gp_XY thePoint2,
  gp_XY thePointToCheck)
        {
            gp_XY aP1 = thePoint2 - thePoint1;
            gp_XY aP2 = thePointToCheck - thePoint1;

            double aPrec = Precision.PConfusion();
            double aSqPrec = aPrec * aPrec;
            double aDist = Math.Abs(aP1 ^ aP2);
            if (aDist > aPrec)
            {
                aDist = (aDist * aDist) / aP1.SquareModulus();
                if (aDist > aSqPrec)
                    return 0; //out
            }

            gp_XY aMult = aP1.Multiplied(aP2);
            if (aMult.X() < 0.0 || aMult.Y() < 0.0)
                return 0; //out

            if (aP1.SquareModulus() < aP2.SquareModulus())
                return 0; //out

            if (thePointToCheck.IsEqual(thePoint1, aPrec) ||
                thePointToCheck.IsEqual(thePoint2, aPrec))
            {
                return -1; //coincides with an end point
            }

            return 1;
        }

        // For better meshing performance we try to estimate the acceleration circles grid structure sizes:
        // For each parametric direction (U, V) we estimate firstly an approximate distance between the future points -
        // this estimation takes into account the required face deflection and the complexity of the face.
        // Particularly, the complexity of the faces based on BSpline curves and surfaces requires much more points.
        // At the same time, for planar faces and linear parts of the arbitrary surfaces usually no intermediate points
        // are necessary.
        // The general idea for each parametric direction:
        // cells_count = 2 ^ log10 ( estimated_points_count )
        // For linear parametric direction we fall back to the initial vertex count:
        // cells_count = 2 ^ log10 ( initial_vertex_count )
        public static (int, int) CellsCount(
    Adaptor3d_Surface theSurface,
    int theVerticesNb,
    double theDeflection,
    AbstractRangeSplitter theRangeSplitter)
        {
            if (theRangeSplitter == null)
                return (-1, -1);

            GeomAbs_SurfaceType aType = theSurface._GetType();

            double anErrFactorU = 0, anErrFactorV = 0;
            ComputeErrFactors(theDeflection, theSurface, ref anErrFactorU, ref anErrFactorV);

            (double, double) aRangeU = theRangeSplitter.GetRangeU();
            (double, double) aRangeV = theRangeSplitter.GetRangeV();
            (double, double) aDelta = theRangeSplitter.GetDelta();

            int aCellsCountU = 0, aCellsCountV = 0;
            //if (aType == GeomAbs_SurfaceType.GeomAbs_Torus)
            //{
            //    aCellsCountU = (Standard_Integer)Ceiling(Pow(2, Log10(
            //      (aRangeU.second - aRangeU.first) / aDelta.first)));
            //    aCellsCountV = (Standard_Integer)Ceiling(Pow(2, Log10(
            //      (aRangeV.second - aRangeV.first) / aDelta.second)));
            //}
            //else if (aType == GeomAbs_Cylinder)
            //{
            //    aCellsCountU = (Standard_Integer)Ceiling(Pow(2, Log10(
            //      (aRangeU.second - aRangeU.first) / aDelta.first /
            //      (aRangeV.second - aRangeV.first))));
            //    aCellsCountV = (Standard_Integer)Ceiling(Pow(2, Log10(
            //      (aRangeV.second - aRangeV.first) / anErrFactorV)));
            //}
            //else
            {
                aCellsCountU = (int)Math.Ceiling(Math.Pow(2, Math.Log10(
                  (aRangeU.Item2 - aRangeU.Item1) / aDelta.Item1 / anErrFactorU)));
                aCellsCountV = (int)Math.Ceiling(Math.Pow(2, Math.Log10(
                  (aRangeV.Item2 - aRangeV.Item1) / aDelta.Item2 / anErrFactorV)));
            }

            AdjustCellsCounts(theSurface, theVerticesNb, ref aCellsCountU, ref aCellsCountV);
            return (aCellsCountU, aCellsCountV);
        }
        public static IntFlag IntSegSeg(
  gp_XY theStartPnt1,
  gp_XY theEndPnt1,
  gp_XY theStartPnt2,
  gp_XY theEndPnt2,
  bool isConsiderEndPointTouch,
  bool isConsiderPointOnSegment,
 ref  gp_Pnt2d theIntPnt)
        {
            int[] aPointHash = {
    classifyPoint(theStartPnt1, theEndPnt1, theStartPnt2),
    classifyPoint(theStartPnt1, theEndPnt1, theEndPnt2  ),
    classifyPoint(theStartPnt2, theEndPnt2, theStartPnt1),
    classifyPoint(theStartPnt2, theEndPnt2, theEndPnt1  )
  };

            int aPosHash =
              aPointHash[0] + aPointHash[1] + aPointHash[2] + aPointHash[3];

            // Consider case when edges have shared vertex
            if (aPointHash[0] < 0 || aPointHash[1] < 0)
            {
                if (aPosHash == -1)
                {
                    // -1 means, that 2 points are equal, and 1 point is on another curve
                    return IntFlag.Glued;
                }
                else
                {
                    if (isConsiderEndPointTouch)
                        return IntFlag.EndPointTouch;

                    return IntFlag.NoIntersection;
                }
            }

            /*=========================================*/
            /*  1) hash code == 1:

                              0+
                              /
                     0      1/         0
                     +======+==========+

                2) hash code == 2:

                     0    1        1   0
                  a) +----+========+---+

                     0       1   1     0
                  b) +-------+===+=====+

                                                       */
            /*=========================================*/
            if (aPosHash == 1)
            {
                if (isConsiderPointOnSegment)
                {
                    if (aPointHash[0] == 1)
                        theIntPnt = new gp_Pnt2d(theStartPnt1);
                    else if (aPointHash[1] == 1)
                        theIntPnt = theEndPnt1.To_gp_Pnt2d();
                    else if (aPointHash[2] == 1)
                        theIntPnt = theStartPnt2.To_gp_Pnt2d();
                    else
                        theIntPnt = theEndPnt2.To_gp_Pnt2d();

                    return IntFlag.PointOnSegment;
                }

                return IntFlag.NoIntersection;
            }
            else if (aPosHash == 2)
                return IntFlag.Glued;

            double[] aParam = new double[2];
            IntFlag aIntFlag = IntLinLin(theStartPnt1, theEndPnt1,
              theStartPnt2, theEndPnt2, ref theIntPnt.coord, aParam);

            if (aIntFlag == IntFlag.NoIntersection)
                return IntFlag.NoIntersection;

            if (aIntFlag == IntFlag.Same)
            {
                if (aPosHash < -2)
                    return IntFlag.Same;
                else if (aPosHash == -1)
                    return IntFlag.Glued;

                return IntFlag.NoIntersection;
            }

            // Cross
            // Intersection is out of segments ranges
            double aPrec = Precision.PConfusion();
            double aEndPrec = 1 - aPrec;
            for (int i = 0; i < 2; ++i)
            {
                if (aParam[i] < aPrec || aParam[i] > aEndPrec)
                    return IntFlag.NoIntersection;
            }

            return IntFlag.Cross;
        }
        public static IntFlag IntLinLin(
  gp_XY theStartPnt1,
  gp_XY theEndPnt1,
  gp_XY theStartPnt2,
  gp_XY theEndPnt2,
  ref gp_XY theIntPnt,
  double[] theParamOnSegment)
        {
            gp_XY aVec1 = theEndPnt1 - theStartPnt1;
            gp_XY aVec2 = theEndPnt2 - theStartPnt2;
            gp_XY aVecO1O2 = theStartPnt2 - theStartPnt1;

            double aCrossD1D2 = aVec1 ^ aVec2;
            double aCrossD1D3 = aVecO1O2 ^ aVec2;

            double aPrec = gp.Resolution();
            // Are edgegs codirectional
            if (Math.Abs(aCrossD1D2) < aPrec)
            {
                // Just a parallel case?
                if (Math.Abs(aCrossD1D3) < aPrec)
                    return IntFlag.Same;
                else
                    return IntFlag.NoIntersection;
            }

            theParamOnSegment[0] = aCrossD1D3 / aCrossD1D2;
            theIntPnt = theStartPnt1 +  aVec1* theParamOnSegment[0] ;

            double aCrossD2D3 = aVecO1O2 ^ aVec1;
            theParamOnSegment[1] = aCrossD2D3 / aCrossD1D2;

            return IntFlag.Cross;
        }
        private static void ComputeErrFactors(double theDeflection,
                    Adaptor3d_Surface theFace, ref double theErrFactorU, ref double theErrFactorV)
        {
            theErrFactorU = theDeflection * 10.0;
            theErrFactorV = theDeflection * 10.0;

            switch (theFace._GetType())
            {
                case GeomAbs_SurfaceType.GeomAbs_Cylinder:
                case GeomAbs_SurfaceType.GeomAbs_Cone:
                case GeomAbs_SurfaceType.GeomAbs_Sphere:
                case GeomAbs_SurfaceType.GeomAbs_Torus:
                    break;

                case GeomAbs_SurfaceType.GeomAbs_SurfaceOfExtrusion:
                case GeomAbs_SurfaceType.GeomAbs_SurfaceOfRevolution:
                    {
                        //Handle(Adaptor3d_Curve) aCurve = theFace->BasisCurve();
                        //if (aCurve->GetType() == GeomAbs_BSplineCurve && aCurve->Degree() > 2)
                        //{
                        //    theErrFactorV /= (aCurve->Degree() * aCurve->NbKnots());
                        //}
                        break;
                    }
                case GeomAbs_SurfaceType.GeomAbs_BezierSurface:
                    {
                        //if (theFace->UDegree() > 2)
                        //{
                        //    theErrFactorU /= (theFace->UDegree());
                        //}
                        //if (theFace->VDegree() > 2)
                        //{
                        //    theErrFactorV /= (theFace->VDegree());
                        //}
                        break;
                    }
                case GeomAbs_SurfaceType.GeomAbs_BSplineSurface:
                    {
                        //if (theFace.UDegree() > 2)
                        //{
                        //    theErrFactorU /= (theFace.UDegree() * theFace.NbUKnots());
                        //}
                        //if (theFace.VDegree() > 2)
                        //{
                        //    theErrFactorV /= (theFace.VDegree() * theFace.NbVKnots());
                        //}
                        break;
                    }

                case GeomAbs_SurfaceType.GeomAbs_Plane:
                default:
                    theErrFactorU = theErrFactorV = 1.0;
                    break;
            }
        }

        static void AdjustCellsCounts(Adaptor3d_Surface theFace,
                             int theNbVertices,
                          ref int theCellsCountU,
                            ref int theCellsCountV)
        {
            GeomAbs_SurfaceType aType = theFace._GetType();
            if (aType == GeomAbs_SurfaceType.GeomAbs_OtherSurface)
            {
                // fallback to the default behavior
                theCellsCountU = theCellsCountV = -1;
                return;
            }

            double aSqNbVert = theNbVertices;
            if (aType == GeomAbs_SurfaceType.GeomAbs_Plane)
            {
                theCellsCountU = theCellsCountV = (int)Math.Ceiling(Math.Pow(2, Math.Log10(aSqNbVert)));
            }
            //else if (aType == GeomAbs_SurfaceType.GeomAbs_Cylinder || aType == GeomAbs_SurfaceType.GeomAbs_Cone)
            //{
            //    theCellsCountV = (Standard_Integer)Ceiling(Pow(2, Log10(aSqNbVert)));
            //}
            //else if (aType == GeomAbs_SurfaceType.GeomAbs_SurfaceOfExtrusion || aType == GeomAbs_SurfaceType.GeomAbs_SurfaceOfRevolution)
            //{
            //    Handle(Adaptor3d_Curve) aCurve = theFace->BasisCurve();
            //    if (aCurve->GetType() == GeomAbs_Line ||
            //       (aCurve->GetType() == GeomAbs_BSplineCurve && aCurve->Degree() < 2))
            //    {
            //        // planar, cylindrical, conical cases
            //        if (aType == GeomAbs_SurfaceType.GeomAbs_SurfaceOfExtrusion)
            //            theCellsCountU = (Standard_Integer)Ceiling(Pow(2, Log10(aSqNbVert)));
            //        else
            //            theCellsCountV = (Standard_Integer)Ceiling(Pow(2, Log10(aSqNbVert)));
            //    }
            //    if (aType == GeomAbs_SurfaceType.GeomAbs_SurfaceOfExtrusion)
            //    {
            //        // V is always a line
            //        theCellsCountV = (Standard_Integer)Ceiling(Pow(2, Log10(aSqNbVert)));
            //    }
            //}
            //else if (aType == GeomAbs_SurfaceType.GeomAbs_BezierSurface || aType == GeomAbs_SurfaceType.GeomAbs_BSplineSurface)
            //{
            //    if (theFace.UDegree() < 2)
            //    {
            //        theCellsCountU = (Standard_Integer)Ceiling(Pow(2, Log10(aSqNbVert)));
            //    }
            //    if (theFace.VDegree() < 2)
            //    {
            //        theCellsCountV = (Standard_Integer)Ceiling(Pow(2, Log10(aSqNbVert)));
            //    }
            //}

            theCellsCountU = Math.Max(theCellsCountU, 2);
            theCellsCountV = Math.Max(theCellsCountV, 2);
        }
    }
}  //! Enumerates states of segments intersection check.

