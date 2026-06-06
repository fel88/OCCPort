using TKG3d;

namespace TKShHealing
{
    //! Complements standard tool Geom_Surface by providing additional
    //! functionality for detection surface singularities, checking
    //! spatial surface closure and computing projections of 3D points
    //! onto a surface.
    //!
    //! * The singularities
    //! Each singularity stores the precision with which corresponding
    //! surface iso-line is considered as degenerated.
    //! The number of singularities is determined by specifying precision
    //! and always not greater than 4.
    //!
    //! * The spatial closure
    //! The check for spatial closure is performed with given precision
    //! (default value is Precision::Confusion).
    //! If Geom_Surface says that the surface is closed, this class
    //! also says this. Otherwise additional analysis is performed.
    //!
    //! * The parameters of 3D point on the surface
    //! The projection of the point is performed with given precision.
    //! This class tries to find a solution taking into account possible
    //! singularities.
    //! Additional method for searching the solution from already built
    //! one is also provided.
    //!
    //! This tool is optimised: computes most information only once
    public class ShapeAnalysis_Surface
    {
        public ShapeAnalysis_Surface(Geom_Surface S)
        {
            mySurf = S;
            myExtOK = false; //:30
            myNbDeg = (-1);
            myIsos = (false);
            myIsoBoxes = (false);
            myGap = 0.0;
            myUDelt = (0.01);
            myVDelt = (0.01);
            myUCloseVal = (-1);
            myVCloseVal = (-1);

            mySurf.Bounds(out myUF, out myUL, out myVF, out myVL);
            myAdSur = new GeomAdaptor_Surface(mySurf);
        }
        bool myExtOK;
        bool myIsos;
        int myNbDeg;
        bool myIsoBoxes;

        double myGap;
        double myUDelt;
        double myVDelt;
        double myUCloseVal;
        double myVCloseVal;
        double myUF;
        double myUL;
        double myVF;
        double myVL;
        Geom_Surface mySurf;
        GeomAdaptor_Surface myAdSur;
    }
}
