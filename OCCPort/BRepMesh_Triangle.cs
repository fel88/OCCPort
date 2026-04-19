using OCCPort.Enums;

namespace OCCPort
{
    //! Light weighted structure representing triangle 
    //! of mesh consisting of oriented links.

    public class BRepMesh_Triangle
    {
        public int []myEdges=new  int[3];
        public bool []myOrientations=new bool[3];
        public BRepMesh_DegreeOfFreedom myMovability;

        public BRepMesh_Triangle(int[] aEdges, bool[] aOrientations, BRepMesh_DegreeOfFreedom bRepMesh_Free)
        {
        }
    }
}