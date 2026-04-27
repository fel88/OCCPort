using OCCPort.Enums;
using System;

namespace OCCPort
{
    //! Light weighted structure representing vertex 
    //! of the mesh in parametric space. Vertex could be 
    //! associated with 3d point stored in external map.
    public class BRepMesh_Vertex
    {

        //! Returns position of the vertex in parametric space.
        public gp_XY Coord()
        {
            return myUV;
        }
        BRepMesh_DegreeOfFreedom myMovability;


        //! Returns movability of the vertex.
        public BRepMesh_DegreeOfFreedom Movability()
        {
            return myMovability;
        }

        gp_XY myUV;
        int myLocation3d;

        //! Creates vertex associated with point in 3d space.
        //! @param theUV position of vertex in parametric space.
        //! @param theLocation3d index of 3d point to be associated with vertex.
        //! @param theMovability movability of the vertex.
        public BRepMesh_Vertex(gp_XY theUV, int theLocation3d, BRepMesh_DegreeOfFreedom theMovability)
        {
            Initialize(theUV, theLocation3d, theMovability);

        }
        //! Initializes vertex associated with point in 3d space.
        //! @param theUV position of vertex in parametric space.
        //! @param theLocation3d index of 3d point to be associated with vertex.
        //! @param theMovability movability of the vertex.
        void Initialize(gp_XY theUV,
                   int theLocation3d,
                   BRepMesh_DegreeOfFreedom theMovability)
        {
            myUV = theUV;
            myLocation3d = theLocation3d;
            myMovability = theMovability;
        }

        //! Creates vertex without association with point in 3d space.
        //! @param theU U position of vertex in parametric space.
        //! @param theV V position of vertex in parametric space.
        //! @param theMovability movability of the vertex.
        public BRepMesh_Vertex(double theU,
                   double theV,
                   BRepMesh_DegreeOfFreedom theMovability)
        {
            myUV = new gp_XY(theU, theV);
            myLocation3d = (0);
            myMovability = (theMovability);
        }
    }
}