using OCCPort.Common;
using TKernel;
using TKMath;

namespace TKMesh
{
    //! Create sort and destroy the circles used in triangulation. <br>
    internal class BRepMesh_CircleTool
    {
        public BRepMesh_CircleTool()
        {
            myTolerance = (Precision.PConfusion());
            myCellFilter = new CircleCellFilter(10.0);
            mySelector = new BRepMesh_CircleInspector(myTolerance, 64);
        }

        public BRepMesh_CircleTool(int theReservedSize)
        {
            myTolerance = (Precision.PConfusion());
            //myAllocator(theAllocator),
            myCellFilter = new CircleCellFilter(10.0);
            mySelector = new BRepMesh_CircleInspector(myTolerance, Math.Max(theReservedSize, 64));
        }

        //! Sets limits of inspection area.
        //! @param theMin bottom left corner of inspection area.
        //! @param theMax top right corner of inspection area.
        public void SetMinMaxSize(gp_XY theMin,
                     gp_XY theMax)
        {
            myFaceMin = theMin;
            myFaceMax = theMax;
        }
        //! Sets new size for cell filter.
        //! @param theSizeX cell size to be set for X dimension.
        //! @param theSizeY cell size to be set for Y dimension.
        public void SetCellSize(double theSizeX,
                     double theSizeY)
        {
            double[] aCellSizeC = { theSizeX, theSizeY };
            NCollection_Array1<double> aCellSize = new NCollection_Array1<double>(aCellSizeC, 1, 2);
            myCellFilter.Reset(aCellSize/*, myAllocator*/);
        }


        //! Deletes a circle from the tool.
        //! @param theIndex index of a circle to be removed.
        internal void Delete(int theIndex)
        {
            BRepMesh_Circle aCircle = mySelector.Circle(theIndex);
            if (aCircle.Radius() > 0.0)
                aCircle.SetRadius(-1);
        }

        //! Select the circles shot by the given point.
        //! @param thePoint bullet point.
        internal ListOfInteger Select(gp_XY thePoint)
        {
            mySelector.SetPoint(thePoint);
            myCellFilter.Inspect(thePoint, mySelector);
            return mySelector.GetShotCircles();
        }

        internal bool Bind(int theIndex, gp_XY thePoint1, gp_XY thePoint2, gp_XY thePoint3)
        {
            gp_XY aLocation = new gp_XY();
            double aRadius = 0;
            if (!MakeCircle(thePoint1, thePoint2, thePoint3, ref aLocation, ref aRadius))
                return false;

            bind(theIndex, aLocation, aRadius);
            return true;
        }

        bool MakeCircle(gp_XY thePoint1,
                                                  gp_XY thePoint2,
                                                  gp_XY thePoint3,
                                               ref gp_XY theLocation,
                                                 ref double theRadius)
        {
            double aPrecision = Precision.PConfusion();
            double aSqPrecision = aPrecision * aPrecision;

            gp_XY aLink1 = new gp_XY((thePoint3).ChangeCoord(1) - (thePoint2).ChangeCoord(1),
                (thePoint2).ChangeCoord(2) - (thePoint3).ChangeCoord(2));
            if (aLink1.SquareModulus() < aSqPrecision)
                return false;

            gp_XY aLink2 = new gp_XY((thePoint1).ChangeCoord(1) - (thePoint3).ChangeCoord(1),
                (thePoint3).ChangeCoord(2) - (thePoint1).ChangeCoord(2));
            if (aLink2.SquareModulus() < aSqPrecision)
                return false;

            gp_XY aLink3 = new gp_XY((thePoint2).ChangeCoord(1) - (thePoint1).ChangeCoord(1),
               (thePoint1).ChangeCoord(2) - (thePoint2).ChangeCoord(2));
            if (aLink3.SquareModulus() < aSqPrecision)
                return false;

            double aD = 2 * ((thePoint1).ChangeCoord(1) * aLink1.Y() +
                                           (thePoint2).ChangeCoord(1) * aLink2.Y() +
                                          (thePoint3).ChangeCoord(1) * aLink3.Y());

            if (Math.Abs(aD) < gp.Resolution())
                return false;

            double aInvD = 1.0 / aD;
            double aSqMod1 = thePoint1.SquareModulus();
            double aSqMod2 = thePoint2.SquareModulus();
            double aSqMod3 = thePoint3.SquareModulus();
            theLocation.ChangeCoord(1, (aSqMod1 * aLink1.Y() +
                                    aSqMod2 * aLink2.Y() +
                                    aSqMod3 * aLink3.Y()) * aInvD);

            theLocation.ChangeCoord(2, (aSqMod1 * aLink1.X() +
                                          aSqMod2 * aLink2.X() +
                                          aSqMod3 * aLink3.X()) * aInvD);

            theRadius = Math.Sqrt(Math.Max(Math.Max((thePoint1 - theLocation).SquareModulus(),
                                     (thePoint2 - theLocation).SquareModulus()),
                                     (thePoint3 - theLocation).SquareModulus())) + 2 * Standard_Real.RealEpsilon();

            return true;
        }

        void bind(int theIndex,
                                gp_XY theLocation,
                                double theRadius)
        {
            BRepMesh_Circle aCirle = new BRepMesh_Circle(theLocation, theRadius);

            //compute coords
            double aMaxX = Math.Min(theLocation.X() + theRadius, myFaceMax.X());
            double aMinX = Math.Max(theLocation.X() - theRadius, myFaceMin.X());
            double aMaxY = Math.Min(theLocation.Y() + theRadius, myFaceMax.Y());
            double aMinY = Math.Max(theLocation.Y() - theRadius, myFaceMin.Y());

            gp_XY aMinPnt = new gp_XY(aMinX, aMinY);
            gp_XY aMaxPnt = new gp_XY(aMaxX, aMaxY);

            myCellFilter.Add(theIndex, aMinPnt, aMaxPnt);
            mySelector.Bind(theIndex, aCirle);
        }

        double myTolerance;
        //Handle(NCollection_IncAllocator)  myAllocator;
        CircleCellFilter myCellFilter;
        BRepMesh_CircleInspector mySelector;
        gp_XY myFaceMax;
        gp_XY myFaceMin;

    }
    internal class CircleCellFilter : NCollection_CellFilter<BRepMesh_CircleInspector>
    {
        public CircleCellFilter(double v) : base(v)
        {
        }
    }
}

