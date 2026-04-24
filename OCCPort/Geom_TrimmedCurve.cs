using System;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;

namespace OCCPort
{
    //! Describes a portion of a curve (termed the "basis
    //! curve") limited by two parameter values inside the
    //! parametric domain of the basis curve.
    //! The trimmed curve is defined by:
    //! - the basis curve, and
    //! - the two parameter values which limit it.
    //! The trimmed curve can either have the same
    //! orientation as the basis curve or the opposite orientation.
    public class Geom_TrimmedCurve : Geom_BoundedCurve

    {//=======================================================================
     //function : BasisCurve
     //purpose  : 
     //=======================================================================

        public Geom_Curve BasisCurve()
        {
            return basisCurve;
        }

        double uTrim1;
        double uTrim2;
        Geom_Curve basisCurve;

        public Geom_TrimmedCurve(Geom_Curve C,
                                      double U1,
                                      double U2,
                                      bool Sense,
                                      bool theAdjustPeriodic)

        {
            uTrim1 = (U1);
            uTrim2 = (U2);
            // kill trimmed basis curves
            Geom_TrimmedCurve T = C as Geom_TrimmedCurve;
            if (T != null)
                basisCurve = (Geom_Curve)(T.BasisCurve().Copy());
            else
                basisCurve = (Geom_Curve)(C.Copy());

            SetTrim(U1, U2, Sense, theAdjustPeriodic);
        }

        public void SetTrim(double U1,
                                 double U2,
                                 bool Sense,
                                 bool theAdjustPeriodic)
        {
            bool sameSense = true;
            if (U1 == U2)
                throw new Standard_ConstructionError("Geom_TrimmedCurve::U1 == U2");

            double Udeb = basisCurve.FirstParameter();
            double Ufin = basisCurve.LastParameter();

            if (basisCurve.IsPeriodic())
            {
                sameSense = Sense;

                // set uTrim1 in the range Udeb , Ufin
                // set uTrim2 in the range uTrim1 , uTrim1 + Period()
                uTrim1 = U1;
                uTrim2 = U2;
                if (theAdjustPeriodic)
                    ElCLib.AdjustPeriodic(Udeb, Ufin,
                                          Math.Min(Math.Abs(uTrim2 - uTrim1) / 2, Precision.PConfusion()),
                                          ref uTrim1, ref uTrim2);
            }

            else
            {
                if (U1 < U2)
                {
                    sameSense = Sense;
                    uTrim1 = U1;
                    uTrim2 = U2;
                }
                else
                {
                    sameSense = !Sense;
                    uTrim1 = U2;
                    uTrim2 = U1;
                }

                if ((Udeb - uTrim1 > Precision.PConfusion()) ||
                (uTrim2 - Ufin > Precision.PConfusion()))
                    throw new Standard_ConstructionError("Geom_TrimmedCurve::parameters out of range");


            }
            if (!sameSense)
            {
                Reverse();
            }
        }
        public override void Reverse()
        {
            double U1 = basisCurve.ReversedParameter(uTrim2);
            double U2 = basisCurve.ReversedParameter(uTrim1);
            basisCurve.Reverse();
            SetTrim(U1, U2, true, false);
        }

        public override double LastParameter()
        {
            return uTrim2;
        }

        public override double FirstParameter()
        {
            return uTrim1;
        }

        public override void D0(double U, ref gp_Pnt P)
        {
            throw new NotImplementedException();
        }

        public override void Transform(gp_Trsf t)
        {
            throw new NotImplementedException();
        }

        public override Geom_Geometry Copy()
        {
            throw new NotImplementedException();
        }

        public override bool IsPeriodic()
        {
            throw new NotImplementedException();
        }



        public override double ReversedParameter(double U)
        {
            throw new NotImplementedException();
        }
    }

}