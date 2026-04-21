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

        public BRepMesh_Vertex(gp_XY gp_XY, int theLocation3d, BRepMesh_DegreeOfFreedom theMovability)
        {
        }
    }
}