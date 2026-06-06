using OCCPort;
using OCCPort.Common;
using TKBRep;
using TKMath;

namespace TKPrim
{
    public class BRepPrim_OneAxis
    {
        public gp_Ax2 Axes()
        {
            return myAxes;
        }

        public void Angle(double A)
        {
            BRepPrim_OneAxis_Check(VerticesBuilt, EdgesBuilt, WiresBuilt, FacesBuilt);
            myAngle = A;
        }

        const int NBVERTICES = 6;
        const int NBEDGES = 9;
        const int NBWIRES = 9;
        const int NBFACES = 5;

        static void BRepPrim_OneAxis_Check(bool[] V,
                      bool[] E,
                      bool[] W,
                      bool[] F)
        {
            int i;
            for (i = 0; i < NBVERTICES; i++)
                if (V[i]) throw new Standard_DomainError();
            for (i = 0; i < NBEDGES; i++)
                if (E[i]) throw new Standard_DomainError();
            for (i = 0; i < NBWIRES; i++)
                if (W[i]) throw new Standard_DomainError();
            for (i = 0; i < NBFACES; i++)
                if (F[i]) throw new Standard_DomainError();
        }

        gp_Ax2 myAxes;
        double myAngle;
        double myVMin;
        double myVMax;
        double myMeridianOffset;
        TopoDS_Shell myShell;
        bool ShellBuilt;
        TopoDS_Vertex[] myVertices = new TopoDS_Vertex[6];
        bool[] VerticesBuilt = new bool[6];
        TopoDS_Edge[] myEdges = new TopoDS_Edge[9];
        bool[] EdgesBuilt = new bool[9];
        TopoDS_Wire[] myWires = new TopoDS_Wire[9];
        bool[] WiresBuilt = new bool[9];
        TopoDS_Face[] myFaces = new TopoDS_Face[5];
        bool[] FacesBuilt = new bool[5];

    }
}
