using OCCPort.Enums;
using System.Linq;

namespace OCCPort
{
    //! Light weighted structure representing triangle 
    //! of mesh consisting of oriented links.

    public class BRepMesh_Triangle
    {
        //! Default constructor.
        public BRepMesh_Triangle()

        {
            myMovability = BRepMesh_DegreeOfFreedom.BRepMesh_Free;
            myEdges[0] = 0;
            myEdges[1] = 0;
            myEdges[2] = 0;
            myOrientations[0] = false;
            myOrientations[1] = false;
            myOrientations[2] = false;
        }

        public int[] myEdges = new int[3];
        public bool[] myOrientations = new bool[3];
        public BRepMesh_DegreeOfFreedom myMovability;

        public BRepMesh_Triangle(int[] theEdges, bool[] theOrientations, BRepMesh_DegreeOfFreedom theMovability)
        {
            Initialize(theEdges, theOrientations, theMovability);

        }
        //! Initializes the triangle by the given parameters.
        //! @param theEdges array of edges of triangle.
        //! @param theOrientations array of edge's orientations.
        //! @param theMovability movability of triangle.
        void Initialize(
    int[] theEdges,
    bool[] theOrientations,
    BRepMesh_DegreeOfFreedom theMovability)
        {
            myEdges = theEdges.ToArray();
            myOrientations = theOrientations.ToArray();
            myMovability = theMovability;
        }


    }
}