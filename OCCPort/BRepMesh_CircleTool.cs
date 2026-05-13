using System;

namespace OCCPort
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

        double myTolerance;
        //Handle(NCollection_IncAllocator)  myAllocator;
        CircleCellFilter myCellFilter;
        BRepMesh_CircleInspector mySelector;
        gp_XY myFaceMax;
        gp_XY myFaceMin;

    }
}