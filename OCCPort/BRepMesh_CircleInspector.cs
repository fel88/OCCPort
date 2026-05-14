using OpenTK.Audio.OpenAL;

namespace OCCPort
{
    //! Auxiliary class to find circles shot by the given point.
    internal class BRepMesh_CircleInspector : NCollection_CellFilter_InspectorXY, IInspector
    {
        //! Returns list of circles shot by the reference point.
        public ListOfInteger GetShotCircles()
        {
            return myResIndices;
        }


        //! Returns circle with the given index.
        //! @param theIndex index of circle.
        //! @return circle with the given index.
        public BRepMesh_Circle Circle(int theIndex)
        {
            return myCircles[theIndex];
        }

        //! Adds the circle to vector of circles at the given position.
        //! @param theIndex position of circle in the vector.
        //! @param theCircle circle to be added.
        public void Bind(int theIndex,
              BRepMesh_Circle theCircle)
        {
            myCircles.SetValue(theIndex, theCircle);
        }

        //! Set reference point to be checked.
        //! @param thePoint bullet point.
        public void SetPoint(gp_XY thePoint)
        {
            myResIndices.Clear();
            myPoint = thePoint;
        }

        //! Performs inspection of a circle with the given index.
        //! @param theTargetIndex index of a circle to be checked.
        //! @return status of the check.
        public NCollection_CellFilter_Action Inspect(int theTargetIndex)
        {
            BRepMesh_Circle aCircle = myCircles.Value(theTargetIndex);
            double aRadius = aCircle.Radius();
            if (aRadius < 0.0)
                return NCollection_CellFilter_Action.CellFilter_Purge;

            gp_XY aLoc = (gp_XY)(aCircle.Location());

            double aDX = myPoint.ChangeCoord(1) - aLoc.ChangeCoord(1);
            double aDY = myPoint.ChangeCoord(2) - aLoc.ChangeCoord(2);

            //This check is wrong. It is better to use 
            //  
            //  const Standard_Real aR = aRadius + aToler;
            //  if ((aDX * aDX + aDY * aDY) <= aR * aR)
            //  {
            //    ...
            //  }

            //where aToler = sqrt(mySqTolerance). Taking into account the fact
            //that the input parameter of the class (see constructor) is linear
            //(not quadratic) tolerance there is no point in square root computation.
            //Simply, we do not need to compute square of the input tolerance and to
            //assign it to mySqTolerance. The input linear tolerance is needed to be used.

            //However, this change leads to hangs the test case "perf mesh bug27119".
            //So, this correction is better to be implemented in the future.

            if ((aDX * aDX + aDY * aDY) - (aRadius * aRadius) <= mySqTolerance)
                myResIndices.Append(theTargetIndex);

            return NCollection_CellFilter_Action.CellFilter_Keep;
        }

        double mySqTolerance;
        ListOfInteger myResIndices;
        VectorOfCircle myCircles;
        gp_XY myPoint;


        //! Constructor.
        //! @param theTolerance tolerance to be used for identification of shot circles.
        //! @param theReservedSize size to be reserved for vector of circles.
        //! @param theAllocator memory allocator to be used by internal collections.
        public BRepMesh_CircleInspector(double theTolerance, int theReservedSize)
        {
            mySqTolerance = (theTolerance * theTolerance);
            myResIndices = new ListOfInteger();
            //myCircles = new VectorOfCircle(theReservedSize);
            myCircles = new VectorOfCircle();
        }
    }
}