using OCCPort.Common;
using TKMath;

namespace TKGeomBase
{
    //! The BndLib package provides functions to add a geometric primitive to a bounding box.
    //! Note: these functions work with gp objects, optionally
    //! limited by parameter values. If the curves and surfaces
    //! provided by the gp package are not explicitly
    //! parameterized, they still have an implicit parameterization,
    //! similar to that which they infer for the equivalent Geom or Geom2d objects.
    //! Add : Package to compute the bounding boxes for elementary
    //! objects from gp in 2d and 3d .
    //!
    //! AddCurve2d : A class to compute the bounding box for a curve
    //! in 2d dimensions ;the curve is defined by a tool
    //!
    //! AddCurve : A class to compute the bounding box for a curve
    //! in 3d dimensions ;the curve is defined by a tool
    //!
    //! AddSurface : A class to compute the bounding box for a surface.
    //! The surface is defined by a tool for the geometry and another
    //! tool for the topology (only the edges in 2d dimensions)
    public class BndLib
    {
        public static void OpenMinMax(gp_Dir V, Bnd_Box B)
        {
            gp_Dir OX = new gp_Dir(1.0, 0.0, 0.0);
            gp_Dir OY = new gp_Dir(0.0, 1.0, 0.0);
            gp_Dir OZ = new gp_Dir(0.0, 0.0, 1.0);
            if (V.IsParallel(OX, Precision.Angular()))
            {
                B.OpenXmax(); B.OpenXmin();
            }
            else if (V.IsParallel(OY, Precision.Angular()))
            {
                B.OpenYmax(); B.OpenYmin();
            }
            else if (V.IsParallel(OZ, Precision.Angular()))
            {
                B.OpenZmax(); B.OpenZmin();
            }
            else
            {
                B.OpenXmin(); B.OpenYmin(); B.OpenZmin();
                B.OpenXmax(); B.OpenYmax(); B.OpenZmax();
            }
        }

        public static void Add(gp_Lin L, double P1,
                 double P2,
                 double Tol, Bnd_Box B)
        {

            if (Precision.IsNegativeInfinite(P1))
            {
                if (Precision.IsNegativeInfinite(P2))
                {
                    throw new Standard_Failure("BndLib::bad parameter");
                }
                else if (Precision.IsPositiveInfinite(P2))
                {
                    OpenMinMax(L.Direction(), B);
                    B.Add(ElCLib.Value(0.0, L));
                }

                else
                {
                    OpenMin(L.Direction(), B);
                    B.Add(ElCLib.Value(P2, L));
                }
            }
            else if (Precision.IsPositiveInfinite(P1))
            {
                if (Precision.IsNegativeInfinite(P2))
                {
                    OpenMinMax(L.Direction(), B);
                    B.Add(ElCLib.Value(0.0, L));
                }
                else if (Precision.IsPositiveInfinite(P2))
                {
                    throw new Standard_Failure("BndLib::bad parameter");
                }
                else
                {
                    OpenMax(L.Direction(), B);
                    B.Add(ElCLib.Value(P2, L));
                }
            }
            else
            {
                B.Add(ElCLib.Value(P1, L));
                if (Precision.IsNegativeInfinite(P2))
                {
                    OpenMin(L.Direction(), B);
                }
                else if (Precision.IsPositiveInfinite(P2))
                {
                    OpenMax(L.Direction(), B);
                }
                else
                {
                    B.Add(ElCLib.Value(P2, L));
                }
            }
            B.Enlarge(Tol);
        }

        public static void OpenMax(gp_Dir V, Bnd_Box B)
        {
            gp_Dir OX = new gp_Dir(1.0, 0.0, 0.0);
            gp_Dir OY = new gp_Dir(0.0, 1.0, 0.0);
            gp_Dir OZ = new gp_Dir(0.0, 0.0, 1.0);
            if (V.IsParallel(OX, Precision.Angular()))
                B.OpenXmax();
            else if (V.IsParallel(OY, Precision.Angular()))
                B.OpenYmax();
            else if (V.IsParallel(OZ, Precision.Angular()))
                B.OpenZmax();
            else
            {
                B.OpenXmax(); B.OpenYmax(); B.OpenZmax();
            }
        }
        public static void OpenMin(gp_Dir V, Bnd_Box B)
        {
            gp_Dir OX = new gp_Dir(1.0, 0.0, 0.0);
            gp_Dir OY = new(0.0, 1.0, 0.0);
            gp_Dir OZ = new gp_Dir(0.0, 0.0, 1.0);
            if (V.IsParallel(OX, Precision.Angular()))
                B.OpenXmin();
            else if (V.IsParallel(OY, Precision.Angular()))
                B.OpenYmin();
            else if (V.IsParallel(OZ, Precision.Angular()))
                B.OpenZmin();
            else
            {
                B.OpenXmin(); B.OpenYmin(); B.OpenZmin();
            }
        }
    }
}
