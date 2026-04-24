namespace OCCPort
{
    //=======================================================================
    //class : Approx_CurveOnSurface_Eval3d
    //purpose: evaluator class for approximation of 3d curve
    //=======================================================================

    public class Approx_CurveOnSurface_Eval3d : AdvApprox_EvaluatorFunction
    {
        public Approx_CurveOnSurface_Eval3d(Adaptor3d_Curve theFunc,
                                  double First, double Last)
        {
            fonct = (theFunc);
            StartEndSav[0] = First; StartEndSav[1] = Last;
        }

        Adaptor3d_Curve fonct;
        double[] StartEndSav=new double[2];

    }
}