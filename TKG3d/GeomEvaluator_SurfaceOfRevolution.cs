using OCCPort.Common;
using TKMath;

namespace TKG3d
{
    //! Allows to calculate values and derivatives for surfaces of revolution
    public class GeomEvaluator_SurfaceOfRevolution : GeomEvaluator_Surface
    {

        //! Initialize evaluator by revolved curve, the axis of revolution and the location
        public GeomEvaluator_SurfaceOfRevolution(Geom_Curve theBase,
                                                     gp_Dir theRevolDir,
                                                     gp_Pnt theRevolLoc)
        {

            myBaseCurve = (theBase);
            myRotAxis = new(theRevolLoc, theRevolDir);
        }
        public void D0(
     double theU, double theV,
    out gp_Pnt theValue)
        {
            theValue = new gp_Pnt();//not origin code
            if (myBaseAdaptor != null)
                myBaseAdaptor.D0(theV, ref theValue);
            else
                myBaseCurve.D0(theV, ref theValue);

            gp_Trsf aRotation = new gp_Trsf();
            aRotation.SetRotation(myRotAxis, theU);
            theValue.Transform(aRotation);
        }

        Geom_Curve myBaseCurve;
        Adaptor3d_Curve myBaseAdaptor;
        gp_Ax1 myRotAxis;
    }
}
