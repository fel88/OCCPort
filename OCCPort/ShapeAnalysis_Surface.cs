using System.Reflection.Metadata;

namespace OCCPort
{
    internal class ShapeAnalysis_Surface
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