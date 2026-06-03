using System;
using TKernel;

namespace OCCPort
{
    //! Describes data structure intended to keep mesh nodes 
    //! defined in UV space and implements functionality 
    //! providing their uniqueness regarding their position.
    public class BRepMesh_VertexTool
    {
        BRepMesh_VertexInspector mySelector;

        public BRepMesh_VertexTool(NCollection_IncAllocator myAllocator)
        {
            mySelector = new BRepMesh_VertexInspector(myAllocator);
            myCellFilter = new VertexCellFilter(0.0);
        }//! Returns vertex by the given index.
        public BRepMesh_Vertex FindKey(int theIndex)
        {
            return mySelector.GetVertex(theIndex);
        }

        //! Sets the tolerance to be used for identification of 
        //! coincident vertices.
        //! @param theToleranceX tolerance for X dimension.
        //! @param theToleranceY tolerance for Y dimension.
        public void SetTolerance(double theToleranceX,
                      double theToleranceY)
        {
            mySelector.SetTolerance(theToleranceX, theToleranceY);
            myTolerance[0] = theToleranceX;
            myTolerance[1] = theToleranceY;
        }

        //! Sets new size of cell for cellfilter.
        //! @param theSizeX size for X dimension.
        //! @param theSizeY size for Y dimension.
        public void SetCellSize(double theSizeX,
                    double theSizeY)
        {
            double[] aCellSizeC = { theSizeX, theSizeY };
            NCollection_Array1<double> aCellSize = new NCollection_Array1<double>(aCellSizeC, 1, 2);
            myCellFilter.Reset(aCellSize);
            mySelector.Clear();
        }

        //! Gets the tolerance to be used for identification of 
        //! coincident vertices.
        //! @param theToleranceX tolerance for X dimension.
        //! @param theToleranceY tolerance for Y dimension.
        public void GetTolerance(ref double theToleranceX,
                             ref double theToleranceY)
        {
            theToleranceX = myTolerance[0];
            theToleranceY = myTolerance[1];
        }
        public void DeleteVertex(int theIndex)
        {
            BRepMesh_Vertex aV = mySelector.GetVertex(theIndex);

            gp_XY aMinPnt = new gp_XY(), aMaxPnt = new gp_XY();
            expandPoint(aV.Coord(), ref aMinPnt, ref aMaxPnt);

            myCellFilter.Remove(theIndex, aMinPnt, aMaxPnt);
            mySelector.Delete(theIndex);
        }

        //! Returns a number of vertices.
        public int Extent()
        {
            return mySelector.NbVertices();
        }
        //! Returns index of the given vertex.
        int FindIndex(BRepMesh_Vertex theVertex)
        {
            mySelector.SetPoint(theVertex.Coord());
            myCellFilter.Inspect(theVertex.Coord(), mySelector);
            return mySelector.GetCoincidentPoint();
        }


        //! Adds vertex with empty data to the tool.
        //! @param theVertex node to be added to the mesh.
        //! @param isForceAdd adds the given node to structure without 
        //! checking on coincidence with other nodes.
        //! @return index of the node in the structure.
        internal int Add(BRepMesh_Vertex theVertex, bool isForceAdd)
        {
            int aIndex = isForceAdd ? 0 : FindIndex(theVertex);
            if (aIndex == 0)
            {
                aIndex = mySelector.Add(theVertex);

                gp_XY aMinPnt = new gp_XY(), aMaxPnt = new gp_XY();
                expandPoint(theVertex.Coord(), ref aMinPnt, ref aMaxPnt);
                myCellFilter.Add(aIndex, aMinPnt, aMaxPnt);
            }
            return aIndex;
        }

        VertexCellFilter myCellFilter;

        //! Expands the given point according to specified tolerance.
        //! @param thePoint point to be expanded.
        //! @param[out] theMinPoint bottom left corner of area defined by expanded point.
        //! @param[out] theMaxPoint top right corner of area defined by expanded point.
        void expandPoint(gp_XY thePoint,
                  ref gp_XY theMinPoint,
                  ref gp_XY theMaxPoint)
        {
            theMinPoint.SetX(thePoint.X() - myTolerance[0]);
            theMinPoint.SetY(thePoint.Y() - myTolerance[1]);
            theMaxPoint.SetX(thePoint.X() + myTolerance[0]);
            theMaxPoint.SetY(thePoint.Y() + myTolerance[1]);
        }

        double[] myTolerance = new double[2];
    }
}