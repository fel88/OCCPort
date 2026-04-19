using OCCPort;
using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace OCCPort
{
    public class BRepMesh_GeomTool
    {
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

            GeomAbs_SurfaceType aType = theSurface.GetType();

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

        private static void ComputeErrFactors(double theDeflection, Adaptor3d_Surface theSurface, ref double anErrFactorU, ref double anErrFactorV)
        {
            throw new NotImplementedException();
        }

        static void AdjustCellsCounts(Adaptor3d_Surface theFace,
                             int theNbVertices,
                          ref int theCellsCountU,
                            ref int theCellsCountV)
        {
            GeomAbs_SurfaceType aType = theFace.GetType();
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

