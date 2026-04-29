using System;

namespace OCCPort
{//! Provides a triangulation for a surface, a set of surfaces, or more generally a shape.
 //!
 //! A triangulation consists of an approximate representation of the actual shape,
 //! using a collection of points and triangles.
 //! The points are located on the surface.
 //! The edges of the triangles connect adjacent points with a straight line that approximates the true curve on the surface.
 //!
 //! A triangulation comprises:
 //! - A table of 3D nodes (3D points on the surface).
 //! - A table of triangles.
 //!   Each triangle (Poly_Triangle object) comprises a triplet of indices in the table of 3D nodes specific to the triangulation.
 //! - An optional table of 2D nodes (2D points), parallel to the table of 3D nodes.
 //!   2D point are the (u, v) parameters of the corresponding 3D point on the surface approximated by the triangulation.
 //! - An optional table of 3D vectors, parallel to the table of 3D nodes, defining normals to the surface at specified 3D point.
 //! - An optional deflection, which maximizes the distance from a point on the surface to the corresponding point on its approximate triangulation.
 //!
 //! In many cases, algorithms do not need to work with the exact representation of a surface.
 //! A triangular representation induces simpler and more robust adjusting, faster performances, and the results are as good.
    public class Poly_Triangulation
    {
        public Poly_Triangulation()
        {
            myCachedMinMax = null;
            myDeflection = (0);
            myPurpose = Poly_MeshPurpose.Poly_MeshPurpose_NONE;
            //
        }
        Bnd_Box myCachedMinMax;
        //! Returns triangle at the given index.
        //! @param[in] theIndex triangle index within [1, NbTriangles()] range
        //! @return triangle node indices, with each node defined within [1, NbNodes()] range
        public Poly_Triangle Triangle(int theIndex) { return myTriangles.Value(theIndex); }
        //! Returns mesh purpose bits.
        public Poly_MeshPurpose MeshPurpose() { return myPurpose; }
        //! Returns normal at the given index.
        //! @param[in] theIndex node index within [1, NbNodes()] range
        //! @return normalized 3D vector defining a surface normal
        public gp_Dir Normal(int theIndex)
        {
            gp_Vec3f aNorm = myNormals.Value(theIndex - 1);
            return new gp_Dir(aNorm.x(), aNorm.y(), aNorm.z());
        }
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

        Poly_ArrayOfNodes myNodes = new Poly_ArrayOfNodes();
        NCollection_Array1<gp_Vec3f> myNormals = new NCollection_Array1<gp_Vec3f>();

        Poly_MeshPurpose myPurpose;

        //! Returns the number of nodes for this triangulation.
        public int NbNodes() { return myNodes.Length(); }

        //! Returns the number of triangles for this triangulation.
        internal int NbTriangles()
        {
            return myTriangles.Length();
        }

        //! Returns TRUE if triangulation has some geometry.
        internal bool HasGeometry()
        {
            return !myNodes.IsEmpty() && !myTriangles.IsEmpty();
        }

        //! Sets a node coordinates.
        //! @param[in] theIndex node index within [1, NbNodes()] range
        //! @param[in] thePnt   3D point coordinates
        public void SetNode(int theIndex,
                   gp_Pnt thePnt)
        {
            myNodes.SetValue(theIndex - 1, thePnt);
        }

        Poly_ArrayOfUVNodes myUVNodes = new Poly_ArrayOfUVNodes();

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

        //! Returns Standard_True if 2D nodes are associated with 3D nodes for this triangulation.
        public bool HasUVNodes() { return !myUVNodes.IsEmpty(); }

        //! Returns UV-node at the given index.
        //! @param[in] theIndex node index within [1, NbNodes()] range
        //! @return 2D point defining UV coordinates
        public gp_Pnt2d UVNode(int theIndex) { return myUVNodes.Value(theIndex - 1); }

    }
}
