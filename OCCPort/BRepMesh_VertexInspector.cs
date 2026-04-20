namespace OCCPort
{
    //! Class intended for fast searching of the coincidence points.
    internal class BRepMesh_VertexInspector
    {
        VectorOfVertex myVertices;

        public BRepMesh_VertexInspector(NCollection_IncAllocator myAllocator)
        {
            myIndex = (0);
            //myMinSqDist = (RealLast());
            myVertices = new VectorOfVertex();
            //myDelNodes = (theAllocator);

            SetTolerance(Precision.Confusion());
        }
        //! Sets the tolerance to be used for identification of 
        //! coincident vertices equal for both dimensions.
        void SetTolerance(double theTolerance)
        {
            myTolerance[0] = theTolerance * theTolerance;
            myTolerance[1] = 0.0;
        }
        int myIndex;
        double myMinSqDist;
        double[] myTolerance = new double[2];

        //ListOfInteger myDelNodes;
        gp_XY myPoint;
        //! Returns number of registered vertices.
        public int NbVertices()
        {
            return myVertices.Length();
        }
    }
}