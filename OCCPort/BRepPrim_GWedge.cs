using System;
using System.Data;
using System.Security.Cryptography;

namespace OCCPort
{
    internal class BRepPrim_GWedge
    {
        static int[] num = { 0, 1, 2, 3, 4, 5 };
        protected BRepPrim_Builder myBuilder;
        protected gp_Ax2 myAxes;
        protected double XMin;
        protected double XMax;
        protected double YMin;
        protected double YMax;
        protected double ZMin;
        protected double ZMax;
        protected double Z2Min;
        protected double Z2Max;
        protected double X2Min;
        protected double X2Max;
        protected TopoDS_Shell myShell;
        bool ShellBuilt;
        TopoDS_Vertex[] myVertices = new TopoDS_Vertex[8];
        bool[] VerticesBuilt = new bool[8];
        TopoDS_Edge[] myEdges = new TopoDS_Edge[12];
        bool[] EdgesBuilt = new bool[12];
        TopoDS_Wire[] myWires = new TopoDS_Wire[6];
        bool[] WiresBuilt = new bool[6];
        TopoDS_Face[] myFaces = new TopoDS_Face[6];
        bool[] FacesBuilt = new bool[6];
        bool[] myInfinite = new bool[6];
        
        public TopoDS_Shell Shell()
        {
            if (IsDegeneratedShape())
                throw new Standard_DomainError();

            if (!ShellBuilt)
            {
                myBuilder.MakeShell(myShell);

                if (HasFace(BRepPrim_Direction.BRepPrim_XMin))
                    myBuilder.AddShellFace(myShell, Face(BRepPrim_Direction.BRepPrim_XMin));
                if (HasFace(BRepPrim_Direction.BRepPrim_XMax))
                    myBuilder.AddShellFace(myShell, Face(BRepPrim_Direction.BRepPrim_XMax));
                if (HasFace(BRepPrim_Direction.BRepPrim_YMin))
                    myBuilder.AddShellFace(myShell, Face(BRepPrim_Direction.BRepPrim_YMin));
                if (HasFace(BRepPrim_Direction.BRepPrim_YMax))
                    myBuilder.AddShellFace(myShell, Face(BRepPrim_Direction.BRepPrim_YMax));
                if (HasFace(BRepPrim_Direction.BRepPrim_ZMin))
                    myBuilder.AddShellFace(myShell, Face(BRepPrim_Direction.BRepPrim_ZMin));
                if (HasFace(BRepPrim_Direction.BRepPrim_ZMax))
                    myBuilder.AddShellFace(myShell, Face(BRepPrim_Direction.BRepPrim_ZMax));

                myShell.Closed(BRep_Tool.IsClosed(myShell));
                myBuilder.CompleteShell(myShell);
                ShellBuilt = true;
            }
            return myShell;
        }

        private bool IsDegeneratedShape()
        {
            throw new NotImplementedException();
        }



        //=======================================================================
        //function : HasFace 
        //purpose  : true if the face exist in one direction
        //=======================================================================

        public bool HasFace(BRepPrim_Direction d1)
        {
            bool state = !myInfinite[BRepPrim_Wedge_NumDir1(d1)];
            if (d1 == BRepPrim_Direction.BRepPrim_YMax) state = state && (Z2Max != Z2Min)
                                                       && (X2Max != X2Min);
            return state;
        }

        //=======================================================================
        //function : BRepPrim_Wedge_NumDir2
        //purpose  : when giving two directions return the range of the edge
        //=======================================================================

        static int BRepPrim_Wedge_NumDir2
                      (BRepPrim_Direction d1,
                BRepPrim_Direction d2)
        {
            int i1 = BRepPrim_Wedge_NumDir1(d1);
            int i2 = BRepPrim_Wedge_NumDir1(d2);
            if (i1 / 2 == i2 / 2) throw new Exception();
            return tab[i1][i2];
        }
        static int[][] tab = new[]{new []{-1,-1, 0, 1, 8, 9},
                       new[]{-1,-1, 2, 3,10,11},
new[]                      { 0, 2,-1,-1, 4, 5},
                       new[]{ 1, 3,-1,-1, 6, 7},
new[]                      { 8,10, 4, 6,-1,-1},
                       new[]{ 9,11, 5, 7,-1,-1}};


        static int BRepPrim_Wedge_NumDir1
              (BRepPrim_Direction d1)
        { return num[(int)d1]; }

        public TopoDS_Face Face(BRepPrim_Direction d1)
        {
            int i = BRepPrim_Wedge_NumDir1(d1);

            if (!FacesBuilt[i])
            {
                gp_Pln P = Plane(d1);
                myBuilder.MakeFace(myFaces[i], P);
                if (HasWire(d1)) myBuilder.AddFaceWire(myFaces[i], Wire(d1));
                if (i % 2 == 0) myBuilder.ReverseFace(myFaces[i]);

                // pcurves

                BRepPrim_Direction dd1 = BRepPrim_Direction.BRepPrim_ZMin, dd2 = BRepPrim_Direction.BRepPrim_YMax,
                dd3 = BRepPrim_Direction.BRepPrim_ZMax, dd4 = BRepPrim_Direction.BRepPrim_YMin;

                switch (i / 2)
                {

                    case 0:
                        // XMin XMax
                        dd1 = BRepPrim_Direction.BRepPrim_ZMin;
                        dd2 = BRepPrim_Direction.BRepPrim_YMax;
                        dd3 = BRepPrim_Direction.BRepPrim_ZMax;
                        dd4 = BRepPrim_Direction.BRepPrim_YMin;
                        break;

                    case 1:
                        // YMin YMax
                        dd1 = BRepPrim_Direction.BRepPrim_XMin;
                        dd2 = BRepPrim_Direction.BRepPrim_ZMax;
                        dd3 = BRepPrim_Direction.BRepPrim_XMax;
                        dd4 = BRepPrim_Direction.BRepPrim_ZMin;
                        break;

                    case 2:
                        // ZMin ZMax
                        dd1 = BRepPrim_Direction.BRepPrim_YMin;
                        dd2 = BRepPrim_Direction.BRepPrim_XMax;
                        dd3 = BRepPrim_Direction.BRepPrim_YMax;
                        dd4 = BRepPrim_Direction.BRepPrim_XMin;
                        break;

                };

                gp_Lin L;
                gp_Dir DX = P.XAxis().Direction();
                gp_Dir DY = P.YAxis().Direction();
                double U, V, DU, DV;
                //if (HasEdge(d1, dd4))
                //{
                //	L = Line(d1, dd4);
                //	ElSLib.Parameters(P, L.Location(), U, V);
                //	DU = L.Direction() * DX;
                //	DV = L.Direction() * DY;
                //	myBuilder.SetPCurve(myEdges[BRepPrim_Wedge_NumDir2(d1, dd4)],
                //			myFaces[i],
                //			gp_Lin2d(gp_Pnt2d(U, V), gp_Dir2d(DU, DV)));
                //}
                //if (HasEdge(d1, dd3))
                //{
                //	L = Line(d1, dd3);
                //	ElSLib.Parameters(P, L.Location(), U, V);
                //	DU = L.Direction() * DX;
                //	DV = L.Direction() * DY;
                //	myBuilder.SetPCurve(myEdges[BRepPrim_Wedge_NumDir2(d1, dd3)],
                //			myFaces[i],
                //			gp_Lin2d(gp_Pnt2d(U, V), gp_Dir2d(DU, DV)));
                //}

                //if (HasEdge(d1, dd2))
                //{
                //	L = Line(d1, dd2);
                //	ElSLib.Parameters(P, L.Location(), U, V);
                //	DU = L.Direction() * DX;
                //	DV = L.Direction() * DY;
                //	myBuilder.SetPCurve(myEdges[BRepPrim_Wedge_NumDir2(d1, dd2)],
                //			myFaces[i],
                //			gp_Lin2d(gp_Pnt2d(U, V), gp_Dir2d(DU, DV)));
                //}

                //if (HasEdge(d1, dd1))
                //{
                //	L = Line(d1, dd1);
                //	ElSLib.Parameters(P, L.Location(), U, V);
                //	DU = L.Direction() * DX;
                //	DV = L.Direction() * DY;
                //	myBuilder.SetPCurve(myEdges[BRepPrim_Wedge_NumDir2(d1, dd1)],
                //			myFaces[i],
                //			gp_Lin2d(gp_Pnt2d(U, V), gp_Dir2d(DU, DV)));
                //}


                //myBuilder.CompleteFace(myFaces[i]);
                FacesBuilt[i] = true;
            }

            return myFaces[i];


        }

        private object Wire(BRepPrim_Direction d1)
        {
            throw new NotImplementedException();
        }

        private bool HasWire(BRepPrim_Direction d1)
        {
            throw new NotImplementedException();
        }

        private gp_Pln Plane(BRepPrim_Direction d1)
        {
            int i = BRepPrim_Wedge_NumDir1(d1);

            gp_Dir D = new gp_Dir();
            gp_Vec VX = new gp_Vec(myAxes.XDirection());
            gp_Vec VY = new gp_Vec(myAxes.YDirection());
            gp_Vec VZ = new gp_Vec(myAxes.Direction());

            //switch (i / 2)
            //{

            //	case 0:
            //		D = myAxes.XDirection();
            //		break;

            //	case 1:
            //		D = myAxes.YDirection();
            //		break;

            //	case 2:
            //		D = myAxes.Direction();
            //		break;

            //};
            //double X = 0.0, Y = 0.0, Z = 0.0;

            //switch (i)
            //{

            //	case 0:
            //		// XMin
            //		X = XMin;
            //		Y = YMin;
            //		Z = ZMin;
            //		if (X2Min != XMin) D = new gp_Dir((YMax - YMin) * VX + (XMin - X2Min) * VY);
            //		break;

            //	case 1:
            //		// XMax
            //		X = XMax;
            //		Y = YMin;
            //		Z = ZMin;
            //		if (X2Max != XMax) D = new gp_Dir((YMax - YMin) * VX + (XMax - X2Max) * VY);
            //		break;

            //	case 2:
            //		// YMin
            //		X = XMin;
            //		Y = YMin;
            //		Z = ZMin;
            //		break;

            //	case 3:
            //		// YMax
            //		X = XMin;
            //		Y = YMax;
            //		Z = ZMin;
            //		break;

            //	case 4:
            //		// ZMin
            //		X = XMin;
            //		Y = YMin;
            //		Z = ZMin;
            //		if (Z2Min != ZMin) D = new gp_Dir((YMax - YMin) * VZ + (ZMin - Z2Min) * VY);
            //		break;

            //	case 5:
            //		// ZMax
            //		X = XMin;
            //		Y = YMin;
            //		Z = ZMax;
            //		if (Z2Max != ZMax) D = new gp_Dir((YMax - YMin) * VZ + (ZMax - Z2Max) * VY);
            //		break;

            //};

            gp_Pnt P = myAxes.Location();
            //P.Translate(X * new gp_Vec(myAxes.XDirection()));
            //P.Translate(Y * new gp_Vec(myAxes.YDirection()));
            //P.Translate(Z * new gp_Vec(myAxes.Direction()));

            gp_Pln plane = new gp_Pln(P, D);
            return plane;

        }

        private gp_Lin Line(BRepPrim_Direction d1, BRepPrim_Direction d2)
        {
            if (!HasEdge(d1, d2)) throw new Exception();

            int i = BRepPrim_Wedge_NumDir2(d1, d2);

            double X = 0.0, Y = 0.0, Z = 0.0;

            gp_Dir D;
            gp_Vec VX = new gp_Vec(myAxes.XDirection());
            gp_Vec VY = new gp_Vec(myAxes.YDirection());
            gp_Vec VZ = new gp_Vec(myAxes.Direction());

            //switch (i / 4)
            //{

            //	case 0:
            //		D = myAxes.Direction();
            //		break;

            //	case 1:
            //		D = myAxes.XDirection();
            //		break;

            //	case 2:
            //		D = myAxes.YDirection();
            //		break;

            //};

            //switch (i)
            //{

            //	case 0:
            //		// XMin YMin
            //		X = XMin;
            //		Y = YMin;
            //		Z = ZMin;
            //		break;

            //	case 1:
            //		// XMin YMax
            //		X = X2Min;
            //		Y = YMax;
            //		Z = Z2Min;
            //		break;

            //	case 2:
            //		// XMax YMin
            //		X = XMax;
            //		Y = YMin;
            //		Z = ZMin;
            //		break;

            //	case 3:
            //		// XMax YMax
            //		X = X2Max;
            //		Y = YMax;
            //		Z = Z2Min;
            //		break;

            //	case 4:
            //		// YMin ZMin
            //		X = XMin;
            //		Y = YMin;
            //		Z = ZMin;
            //		break;

            //	case 5:
            //		// YMin ZMax
            //		X = XMin;
            //		Y = YMin;
            //		Z = ZMax;
            //		break;

            //	case 6:
            //		// YMax ZMin
            //		X = X2Min;
            //		Y = YMax;
            //		Z = Z2Min;
            //		break;

            //	case 7:
            //		// YMax ZMax
            //		X = X2Min;
            //		Y = YMax;
            //		Z = Z2Max;
            //		break;

            //	case 8:
            //		// ZMin XMin
            //		X = XMin;
            //		Y = YMin;
            //		Z = ZMin;
            //		if ((XMin != X2Min) || (ZMin != Z2Min))
            //		{
            //			D = new gp_Vec((X2Min - XMin) * VX + (YMax - YMin) * VY + (Z2Min - ZMin) * VZ);
            //		}
            //		break;

            //	case 9:
            //		// ZMax XMin
            //		X = XMin;
            //		Y = YMin;
            //		Z = ZMax;
            //		if ((XMin != X2Min) || (ZMax != Z2Max))
            //		{
            //			D = new gp_Vec((X2Min - XMin) * VX + (YMax - YMin) * VY + (Z2Max - ZMax) * VZ);
            //		}
            //		break;

            //	case 10:
            //		// ZMin XMax
            //		X = XMax;
            //		Y = YMin;
            //		Z = ZMin;
            //		if ((XMax != X2Max) || (ZMin != Z2Min))
            //		{
            //			D = new gp_Vec((X2Max - XMax) * VX + (YMax - YMin) * VY + (Z2Min - ZMin) * VZ);
            //		}
            //		break;

            //	case 11:
            //		// ZMax XMax
            //		X = XMax;
            //		Y = YMin;
            //		Z = ZMax;
            //		if ((XMax != X2Max) || (ZMax != Z2Max))
            //		{
            //			D = new gp_Vec((X2Max - XMax) * VX + (YMax - YMin) * VY + (Z2Max - ZMax) * VZ);
            //		}
            //		break;

            //}

            //gp_Pnt P = myAxes.Location();
            //P.Translate(X * new gp_Vec(myAxes.XDirection()));
            //P.Translate(Y * new gp_Vec(myAxes.YDirection()));
            //P.Translate(Z * new gp_Vec(myAxes.Direction()));
            //return new gp_Lin(new gp_Ax1(P, D));
            return null;
        }

        private bool HasEdge(BRepPrim_Direction d1, BRepPrim_Direction dd4)
        {
            throw new NotImplementedException();
        }

        public BRepPrim_GWedge(BRepPrim_Builder B, gp_Ax2 Axes, double dx, double dy, double dz)
        {

            myBuilder = (B);
            myAxes = Axes;
            XMin = (0);
            XMax = (dx);
            YMin = (0);
            YMax = (dy);
            ZMin = (0);
            ZMax = (dz);
            Z2Min = (0);
            Z2Max = (dz);
            X2Min = (0);
            X2Max = dx;

            for (int i = 0; i < NBFACES; i++) { myInfinite[i] = false; }

            BRepPrim_Wedge_Init(ref ShellBuilt, VerticesBuilt, EdgesBuilt,
                WiresBuilt, FacesBuilt);
        }


      const  int NBFACES = 6;
          const int NBWIRES = 6;
        const int NBEDGES = 12;
        const int NBVERTICES = 8;
        //=======================================================================
        //function : BRepPrim_Wedge_Init
        //purpose  : Set arrays to Standard_False
        //=======================================================================

        static void BRepPrim_Wedge_Init(ref bool S,
                  bool[] V,
                  bool[] E,
                  bool[] W,
                  bool[] F)
        {
            int i;
            S = false;
            for (i = 0; i < NBVERTICES; i++)
                V[i] = false;
            for (i = 0; i < NBEDGES; i++)
                E[i] = false;
            for (i = 0; i < NBWIRES; i++)
                W[i] = false;
            for (i = 0; i < NBFACES; i++)
                F[i] = false;
        }

        

    }

    public enum BRepPrim_Direction
    {
        BRepPrim_XMin,
        BRepPrim_XMax,
        BRepPrim_YMin,
        BRepPrim_YMax,
        BRepPrim_ZMin,
        BRepPrim_ZMax
    }


}