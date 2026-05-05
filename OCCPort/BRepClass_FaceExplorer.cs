namespace OCCPort
{
    internal class BRepClass_FaceExplorer
    {
        //! Sets the maximum tolerance at 
        //! which to start checking in the intersector
        public void SetMaxTolerance(double theValue)
        {
            myMaxTolerance = theValue;
        }

        //! Sets the status of whether we are
        //! using boxes or not
        public void SetUseBndBox(bool theValue)
        {
            myUseBndBox = theValue;
        }

        const double Probing_Start = 0.123;
        const double Probing_End = 0.7;
        const double Probing_Step = 0.2111;
        public BRepClass_FaceExplorer(TopoDS_Face F)
        {
            myFace = (F);
            myCurEdgeInd = (1);
            myCurEdgePar = (Probing_Start);
            myMaxTolerance = (0.1);
            myUseBndBox = (false);
            myUMin = (Precision.Infinite());
            myUMax = (-Precision.Infinite());
            myVMin = (Precision.Infinite());
            myVMax = (-Precision.Infinite());

            myFace.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
        }


        TopoDS_Face myFace;
        TopExp_Explorer myWExplorer;
        TopExp_Explorer myEExplorer;
        int myCurEdgeInd;
        double myCurEdgePar;
        double myMaxTolerance;
        bool myUseBndBox;
        TopTools_IndexedDataMapOfShapeListOfShape myMapVE;

        double myUMin;
        double myUMax;
        double myVMin;
        double myVMax;
    }
}