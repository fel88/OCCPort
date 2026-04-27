using OCCPort.Enums;
using System;
using System.Linq;

namespace OCCPort
{
    //! Class intended for fast searching of the coincidence points.
    internal class BRepMesh_VertexInspector : NCollection_CellFilter_InspectorXY, IInspector
    {
        //  typedef Standard_Integer Target;

        VectorOfVertex myVertices;
        //! Returns index of point coinciding with regerence one.
        public int GetCoincidentPoint()
        {
            return myIndex;
        } 
        
        //! Returns vertex with the given index.
        public BRepMesh_Vertex GetVertex(int theIndex)
        {
            return myVertices[(theIndex - 1)];
        }

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
        ListOfInteger myDelNodes = new ListOfInteger();

        //! Registers the given vertex.
        //! @param theVertex vertex to be registered.
        public int Add(BRepMesh_Vertex theVertex)
        {
            if (myDelNodes.IsEmpty())
            {
                myVertices.Append(theVertex);
                return myVertices.Length();
            }

            int aNodeIndex = myDelNodes.First();
            myVertices.ChangeValue(aNodeIndex - 1, theVertex);
            myDelNodes.RemoveFirst();
            return aNodeIndex;
        }

        //ListOfInteger myDelNodes;
        gp_XY myPoint;
        //! Returns number of registered vertices.
        public int NbVertices()
        {
            return myVertices.Length();
        }

        //! Set reference point to be checked.
        public void SetPoint(gp_XY thePoint)
        {
            myIndex = 0;
            myMinSqDist = Standard_Real.RealLast();
            myPoint = thePoint;
        }

        public NCollection_CellFilter_Action Inspect(int theTarget)
        {
            BRepMesh_Vertex aVertex = myVertices.Value(theTarget - 1);
            if (aVertex.Movability() == BRepMesh_DegreeOfFreedom.BRepMesh_Deleted)
            {
                myDelNodes.Append(theTarget);
                return NCollection_CellFilter_Action.CellFilter_Purge;
            }

            gp_XY aVec = (myPoint - aVertex.Coord());
            bool inTol;
            if (Math.Abs(myTolerance[1]) < Precision.Confusion())
            {
                inTol = aVec.SquareModulus() < myTolerance[0];
            }
            else
            {
                inTol = ((aVec.X() * aVec.X()) < myTolerance[0]) &&
                        ((aVec.Y() * aVec.Y()) < myTolerance[1]);
            }

            if (inTol)
            {
                double aSqDist = aVec.SquareModulus();
                if (aSqDist < myMinSqDist)
                {
                    myMinSqDist = aSqDist;
                    myIndex = theTarget;
                }
            }

            return NCollection_CellFilter_Action.CellFilter_Keep;
        }
    }
}