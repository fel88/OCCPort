using System;

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