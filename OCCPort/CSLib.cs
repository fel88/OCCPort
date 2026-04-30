using System;

namespace OCCPort
{
    //! This package implements functions for basis geometric
    //! computation on curves and surfaces.
    //! The tolerance criterions used in this package are
    //! Resolution from package gp and RealEpsilon from class
    //! Real of package Standard.
    public class CSLib
    {

        //! If there is a singularity on the surface  the previous method
        //! cannot compute the local normal.
        //! This method computes an approached normal direction of a surface.
        //! It does a limited development and needs the second derivatives
        //! on the surface as input data.
        //! It computes the normal as follow :
        //! N(u, v) = D1U ^ D1V
        //! N(u0+du,v0+dv) = N0 + DN/du(u0,v0) * du + DN/dv(u0,v0) * dv + Eps
        //! with Eps->0 so we can have the equivalence N ~ dN/du + dN/dv.
        //! DNu = ||DN/du|| and DNv = ||DN/dv||
        //!
        //! . if DNu IsNull (DNu <= Resolution from gp) the answer Done = True
        //! the normal direction is given by DN/dv
        //! . if DNv IsNull (DNv <= Resolution from gp) the answer Done = True
        //! the normal direction is given by DN/du
        //! . if the two directions DN/du and DN/dv are parallel Done = True
        //! the normal direction is given either by DN/du or DN/dv.
        //! To check that the two directions are colinear the sinus of the
        //! angle between these directions is computed and compared with
        //! SinTol.
        //! . if DNu/DNv or DNv/DNu is lower or equal than Real Epsilon
        //! Done = False, the normal is undefined
        //! . if DNu IsNull and DNv is Null Done = False, there is an
        //! indetermination and we should do a limited development at
        //! order 2 (it means that we cannot omit Eps).
        //! . if DNu Is not Null and DNv Is not Null Done = False, there are
        //! an infinity of normals at the considered point on the surface.
        public static void Normal(gp_Vec D1U,
            gp_Vec D1V,
            gp_Vec D2U,
            gp_Vec D2V,
            gp_Vec DUV,
            double SinTol,ref bool Done, out CSLib_NormalStatus theStatus, out gp_Dir Normal)
        {

            //  Calculation of an approximate normale in case of a null normal.
            //  Use limited development of the normal of order 1:
            //     N(u0+du,v0+dv) = N0 + dN/du(u0,v0) * du + dN/dv(u0,v0) * dv + epsilon
            //  -> N ~ dN/du + dN/dv.

            Normal = new gp_Dir ();

            gp_Vec D1Nu = D2U.Crossed(D1V);
            D1Nu.Add(D1U.Crossed(DUV));

            gp_Vec D1Nv = DUV.Crossed(D1V);
            D1Nv.Add(D1U.Crossed(D2V));

            double LD1Nu = D1Nu.SquareMagnitude();
            double LD1Nv = D1Nv.SquareMagnitude();

            if (LD1Nu <= Standard_Real.RealEpsilon() && LD1Nv <= Standard_Real.RealEpsilon())
            {
                theStatus = CSLib_NormalStatus.CSLib_D1NIsNull;
                Done = false;
            }
            else if (LD1Nu < Standard_Real.RealEpsilon())
            {
                theStatus = CSLib_NormalStatus.CSLib_D1NuIsNull;
                Done = true;
                Normal = new gp_Dir(D1Nv);
            }
            else if (LD1Nv < Standard_Real.RealEpsilon())
            {
                theStatus = CSLib_NormalStatus.CSLib_D1NvIsNull;
                Done = true;
                Normal = new gp_Dir(D1Nu);
            }
            else if ((LD1Nv / LD1Nu) <= Standard_Real.RealEpsilon())
            {
                theStatus = CSLib_NormalStatus.CSLib_D1NvNuRatioIsNull;
                Done = false;
            }
            else if ((LD1Nu / LD1Nv) <= Standard_Real.RealEpsilon())
            {
                theStatus = CSLib_NormalStatus.CSLib_D1NuNvRatioIsNull;
                Done = false;
            }
            else
            {
                gp_Vec D1NCross = D1Nu.Crossed(D1Nv);
                double Sin2 = D1NCross.SquareMagnitude() / (LD1Nu * LD1Nv);

                if (Sin2 < (SinTol * SinTol))
                {
                    theStatus = CSLib_NormalStatus.CSLib_D1NuIsParallelD1Nv;
                    Done = true;
                    Normal = new gp_Dir(D1Nu);
                }
                else
                {
                    theStatus = CSLib_NormalStatus.CSLib_InfinityOfSolutions;
                    Done = false;
                }
            }

        }


    }
}