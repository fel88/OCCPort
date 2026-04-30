using System;

namespace OCCPort
{
    //! Geom    Library.    This   package   provides   an
    //! implementation of  functions for basic computation
    //! on geometric entity from packages Geom and Geom2d.
    public class GeomLib
    {//! Estimate surface normal at the given (U, V) point.
     //! @param[in]  theSurf input surface
     //! @param[in]  theUV   (U, V) point coordinates on the surface
     //! @param[in]  theTol  estimation tolerance
     //! @param[out] theNorm computed normal
     //! @return 0 if normal estimated from D1,
     //!         1 if estimated from D2 (quasysingular),
     //!       >=2 in case of failure (undefined or infinite solutions)
        public static int NormEstim(Geom_Surface theSurf,
                                                     gp_Pnt2d theUV,
                                                     double theTol,
                                                     gp_Dir theNorm)
        {
            double aTol2 = Math.Pow(theTol, 2);

            gp_Vec DU, DV;
            gp_Pnt aDummyPnt;
            theSurf.D1(theUV.X(), theUV.Y(), out aDummyPnt, out DU, out DV);

            double MDU = DU.SquareMagnitude(), MDV = DV.SquareMagnitude();
            if (MDU >= aTol2
             && MDV >= aTol2)
            {
                gp_Vec aNorm = DU ^ DV;
                double aMagn = aNorm.SquareMagnitude();
                if (aMagn < aTol2)
                {
                    return 3;
                }

                theNorm.SetXYZ(aNorm.XYZ());
                return 0;
            }

            gp_Vec D2U, D2V, D2UV;
            bool isDone = false;
            CSLib_NormalStatus aStatus;
            gp_Dir aNormal;

            theSurf.D2(theUV.X(), theUV.Y(), out aDummyPnt, out DU, out DV, out D2U, out D2V, out D2UV);
            CSLib.Normal(DU, DV, D2U, D2V, D2UV, theTol, ref isDone, out aStatus, out aNormal);
            if (!isDone)
            {
                // computation is impossible
                return aStatus == CSLib_NormalStatus.CSLib_D1NIsNull ? 2 : 3;
            }

            double Umin, Umax, Vmin, Vmax;
            double step = 1.0e-5;
            double eps = 1.0e-16;
            double sign = -1.0;
            theSurf.Bounds(out Umin, out Umax, out Vmin, out Vmax);

            // check for cone apex singularity point
            if ((theUV.Y() > Vmin + step)
             && (theUV.Y() < Vmax - step))
            {
                gp_Dir aNormal1, aNormal2;
                double aConeSingularityAngleEps = 1.0e-4;
                theSurf.D1(theUV.X(), theUV.Y() - sign * step, out aDummyPnt, out DU, out DV);
                if ((DU.XYZ().SquareModulus() > eps) && (DV.XYZ().SquareModulus() > eps))
                {
                    aNormal1 = (DU ^ DV).To_gp_Dir();
                    theSurf.D1(theUV.X(), theUV.Y() + sign * step, out aDummyPnt, out DU, out DV);
                    if ((DU.XYZ().SquareModulus() > eps)
                     && (DV.XYZ().SquareModulus() > eps))
                    {
                        aNormal2 = (DU ^ DV).To_gp_Dir();
                        if (aNormal1.IsOpposite(aNormal2.to_gp_Vec(), aConeSingularityAngleEps))
                        {
                            return 2;
                        }
                    }
                }
            }

            // Along V
            if (MDU < aTol2
             && MDV >= aTol2)
            {
                if ((Vmax - theUV.Y()) > (theUV.Y() - Vmin))
                {
                    sign = 1.0;
                }

                theSurf.D1(theUV.X(), theUV.Y() + sign * step, out aDummyPnt, out DU, out DV);
                gp_Vec Norm = DU ^ DV;
                if (Norm.SquareMagnitude() < eps)
                {
                    double sign1 = -1.0;
                    if ((Umax - theUV.X()) > (theUV.X() - Umin))
                    {
                        sign1 = 1.0;
                    }
                    theSurf.D1(theUV.X() + sign1 * step, theUV.Y() + sign * step, out aDummyPnt, out DU, out DV);
                    Norm = DU ^ DV;
                }
                if (Norm.SquareMagnitude() >= eps
                 && Norm.Dot(aNormal.to_gp_Vec()) < 0.0)
                {
                    aNormal.Reverse();
                }
            }

            // Along U
            if (MDV < aTol2
             && MDU >= aTol2)
            {
                if ((Umax - theUV.X()) > (theUV.X() - Umin))
                {
                    sign = 1.0;
                }

                theSurf.D1(theUV.X() + sign * step, theUV.Y(), out aDummyPnt, out DU, out DV);
                gp_Vec Norm = DU ^ DV;
                if (Norm.SquareMagnitude() < eps)
                {
                    double sign1 = -1.0;
                    if ((Vmax - theUV.Y()) > (theUV.Y() - Vmin))
                    {
                        sign1 = 1.0;
                    }

                    theSurf.D1(theUV.X() + sign * step, theUV.Y() + sign1 * step, out aDummyPnt, out DU, out DV);
                    Norm = DU ^ DV;
                }
                if (Norm.SquareMagnitude() >= eps
                 && Norm.Dot(aNormal.to_gp_Vec()) < 0.0)
                {
                    aNormal.Reverse();
                }
            }

            // quasysingular
            if (aStatus == CSLib_NormalStatus.CSLib_D1NuIsNull
             || aStatus == CSLib_NormalStatus.CSLib_D1NvIsNull
             || aStatus == CSLib_NormalStatus.CSLib_D1NuIsParallelD1Nv)
            {
                theNorm.SetXYZ(aNormal.XYZ());
                return 1;
            }

            return aStatus == CSLib_NormalStatus.CSLib_InfinityOfSolutions ? 2 : 3;
        }

    }
}