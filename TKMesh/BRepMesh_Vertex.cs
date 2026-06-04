using TKMath;

namespace TKMesh
{
    //! Light weighted structure representing vertex 
    //! of the mesh in parametric space. Vertex could be 
    //! associated with 3d point stored in external map.
    public class BRepMesh_Vertex
    {
        //! Checks for equality with another vertex.
        //! @param theOther vertex to be checked against this one.
        //! @return TRUE if equal, FALSE if not.
        public bool IsEqual(BRepMesh_Vertex theOther)
        {
            if (myMovability == BRepMesh_DegreeOfFreedom.BRepMesh_Deleted ||
                theOther.myMovability == BRepMesh_DegreeOfFreedom.BRepMesh_Deleted)
            {
                return false;
            }

            return (myUV.IsEqual(theOther.myUV, Precision.PConfusion()));
        }

        public override bool Equals(object obj)
        {
            return IsEqual(obj as BRepMesh_Vertex);
        }

        public override int GetHashCode()
        {
            int theUpperBound = int.MaxValue;
            //return HashCode(Floor(1e5 * myUV.X()) * Floor(1e5 * myUV.Y()), theUpperBound);
            return (int)(((Math.Floor(1e5 * myUV.X()) * Math.Floor(1e5 * myUV.Y())) % theUpperBound));
        }

        //! Returns position of the vertex in parametric space.
        public gp_XY Coord()
        {
            return myUV;
        }

        BRepMesh_DegreeOfFreedom myMovability;
        //! Sets movability of the vertex.
        public void SetMovability(BRepMesh_DegreeOfFreedom theMovability)
        {
            myMovability = theMovability;
        }

        //! Returns index of 3d point associated with the vertex.
        public int Location3d()
        {
            return myLocation3d;
        }
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

