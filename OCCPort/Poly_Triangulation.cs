using System;

namespace OCCPort
{
    public class Poly_Triangulation
    {
        //! Returns mesh purpose bits.
        public Poly_MeshPurpose MeshPurpose() { return myPurpose; }

        //! Sets a triangle.
        //! @param[in] theIndex triangle index within [1, NbTriangles()] range
        //! @param[in] theTriangle triangle node indices, with each node defined within [1, NbNodes()] range
        public void SetTriangle(int theIndex,
                    Poly_Triangle theTriangle)
        {
            myTriangles.SetValue(theIndex, theTriangle);
        }
        public void ResizeTriangles(int theNbTriangles,
                                        bool theToCopyOld)
        {
            myTriangles.Resize(1, theNbTriangles, theToCopyOld);
        }


        Poly_Array1OfTriangle myTriangles = new Poly_Array1OfTriangle();

        //! Sets mesh purpose bits.
        public void SetMeshPurpose(Poly_MeshPurpose thePurpose) { myPurpose = thePurpose; }
        //! Returns a node at the given index.
        //! @param[in] theIndex node index within [1, NbNodes()] range
        //! @return 3D point coordinates
        public gp_Pnt Node(int theIndex) { return myNodes.Value(theIndex - 1); }

        double myDeflection;

        //! Returns the deflection of this triangulation.
        public double Deflection() { return myDeflection; }

        Poly_ArrayOfNodes myNodes;
        NCollection_Array1<gp_Vec3f> myNormals;

        Poly_MeshPurpose myPurpose;

        //! Returns the number of nodes for this triangulation.
        public int NbNodes() { return myNodes.Length(); }

        internal int NbTriangles()
        {
            throw new NotImplementedException();
        }

        internal bool HasGeometry()
        {
            throw new NotImplementedException();
        }

        internal void SetNode(int aNodeIter, gp_Pnt aNode)
        {
            throw new NotImplementedException();
        }
        Poly_ArrayOfUVNodes myUVNodes;

        internal void AddUVNodes()
        {

            if (myUVNodes.IsEmpty() || myUVNodes.Size() != myNodes.Size())
            {
                myUVNodes.Resize(myNodes.Size(), false);
            }


        }

        //! Method resizing internal arrays of nodes (synchronously for all attributes).
        //! @param theNbNodes   [in] new number of nodes
        //! @param theToCopyOld [in] copy old nodes into the new array
        internal void ResizeNodes(int theNbNodes, bool theToCopyOld)
        {
            myNodes.Resize(theNbNodes, theToCopyOld);
            if (!myUVNodes.IsEmpty())
            {
                myUVNodes.Resize(theNbNodes, theToCopyOld);
            }
            if (!myNormals.IsEmpty())
            {
                myNormals.Resize(0, theNbNodes - 1, theToCopyOld);
            }
        }
    }
}
