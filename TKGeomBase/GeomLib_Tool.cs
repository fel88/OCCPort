using OCCPort.Common;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using TKG3d;
using TKMath;

namespace TKGeomBase
{
    //! Provides various methods with Geom2d and Geom curves and surfaces.
    //! The methods of this class compute the parameter(s) of a given point on a
    //! curve or a surface. To get the valid result the point must be located rather close
    //! to the curve (surface) or at least to allow getting unambiguous result
    //! (do not put point at center of circle...),
    //! but choice of "trust" distance between curve/surface and point is
    //! responsibility of user (parameter MaxDist).
    //! Return FALSE if the point is beyond the MaxDist
    //! limit or if computation fails.
    public class GeomLib_Tool
    {
        //=======================================================================
        //function : Parameter
        //purpose  : Get parameter on curve of given point
        //           return FALSE if point is far from curve than MaxDist
        //           or computation fails
        //=======================================================================

        public static bool Parameter(Geom_Curve Curve,
                                          gp_Pnt Point,
                                          double MaxDist,
                                         ref double U)
        {
            if (Curve == null) return false;
            //
            U = 0.0;
            double aTol = MaxDist * MaxDist;
            //
            GeomAdaptor_Curve aGAC = new(Curve);
            //Extrema_ExtPC extrema = new(Point, aGAC);
            IExtrema_ExtPC extrema = new Extrema_GExtPC(Point, aGAC);
            //
            if (!extrema.IsDone()) return false;
            //
            int n = extrema.NbExt();
            if (n <= 0) return false;
            //
            int i = 0, iMin = 0;
            double Dist2Min = Standard_Real.RealLast();
            for (i = 1; i <= n; i++)
            {
                if (extrema.SquareDistance(i) < Dist2Min)
                {
                    iMin = i;
                    Dist2Min = extrema.SquareDistance(i);
                }
            }
            if (iMin != 0 && Dist2Min <= aTol)
            {
                U = (extrema.Point(iMin)).Parameter();
            }
            else
            {
                return false;
            }

            return true;

        }
    }
}
