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

        //! Returns Standard_True if nodal normals are defined.
        public bool HasNormals() { return !myNormals.IsEmpty(); }

        //! Return an internal array of normals.
        //! Normal()/SetNormal() should be used instead in portable code.
        public NCollection_Array1<gp_Vec3f> InternalNormals() { return myNormals; }


        //! Returns TRUE if triangulation has some geometry.
        internal bool HasGeometry()
        {
            return !myNodes.IsEmpty() && !myTriangles.IsEmpty();
        }

        //! Changes normal at the given index.
        //! @param[in] theIndex node index within [1, NbNodes()] range
        //! @param[in] theVec3  normalized 3D vector defining a surface normal
        public void SetNormal(int theIndex,
                   gp_Vec3f theNormal)
        {
            myNormals.SetValue(theIndex - 1, theNormal);
        }
        //! Changes normal at the given index.
        //! @param[in] theIndex  node index within [1, NbNodes()] range
        //! @param[in] theNormal normalized 3D vector defining a surface normal
        public void SetNormal(int theIndex,
                   gp_Dir theNormal)
        {
            SetNormal(theIndex, new gp_Vec3f((float)(theNormal.X()),
                                           (float)(theNormal.Y()),
                                           (float)(theNormal.Z())));
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
        //! If an array for normals is not allocated yet, do it now.
        public void AddNormals()
        {
            if (myNormals.IsEmpty() || myNormals.Size() != myNodes.Size())
            {
                myNormals.Resize(0, myNodes.Size() - 1, false);
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

        internal void ComputeNormals()
        {
            // zero values
            AddNormals();
            myNormals.Init(new gp_Vec3f(0.0f));

            int[] anElem = { 0, 0, 0 };
            foreach (var aTriIter in myTriangles.triangles)
            {
                aTriIter.Get(ref anElem[0], ref anElem[1], ref anElem[2]);
                gp_Pnt aNode0 = myNodes.Value(anElem[0] - 1);
                gp_Pnt aNode1 = myNodes.Value(anElem[1] - 1);
                gp_Pnt aNode2 = myNodes.Value(anElem[2] - 1);

                gp_XYZ aVec01 = aNode1.XYZ() - aNode0.XYZ();
                gp_XYZ aVec02 = aNode2.XYZ() - aNode0.XYZ();
                gp_XYZ aTriNorm = aVec01 ^ aVec02;
                gp_Vec3f aNorm3f = new gp_Vec3f((float)(aTriNorm.X()), (float)(aTriNorm.Y()), (float)(aTriNorm.Z()));
                for (int aNodeIter = 0; aNodeIter < 3; ++aNodeIter)
                {
                    myNormals[anElem[aNodeIter] - 1] += aNorm3f;
                }
            }

            // Normalize all vectors
            for (int i = 0; i < myNormals.list.Length; i++)
            {
                gp_Vec3f aNodeIter = myNormals.list[i];
                gp_Vec3f  aNorm3f = aNodeIter;
                float aMod = aNorm3f.Modulus();
                myNormals.list[i] = aMod == 0.0f ? new gp_Vec3f(0.0f, 0.0f, 1.0f) : (aNorm3f / aMod);
            }
        }


    }
}
