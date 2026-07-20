using OCCPort;
using OCCPort.Common;
using TKBRep;
using TKG2d;
using TKG3d;
using TKMath;
using TKTopAlgo;

namespace TKPrim
{
    //! implements the abstract Builder with the BRep Builder
    public class BRepPrim_Builder
    {
        public BRepPrim_Builder()
        {
            myBuilder = new BRep_Builder();
        }
        public void MakeShell(TopoDS_Shell S)
        {
            myBuilder.MakeShell(S);
            S.Closed(true);
        }
        public void SetParameters(TopoDS_Edge E,
                          TopoDS_Vertex _,
                          double P1,
                          double P2)
        {
            myBuilder.Range(E, P1, P2);
        }

        public BRep_Builder Builder()
        {
            return myBuilder;
        }

        public void MakeDegeneratedEdge(TopoDS_Edge E)
        {
            myBuilder.MakeEdge(E);
            myBuilder.Degenerated(E, true);
        }

        internal void AddFaceWire(TopoDS_Face F, TopoDS_Wire W)
        {
            myBuilder.Add(F, W);
        }

        BRep_Builder myBuilder;

        public void MakeFace(TopoDS_Face F, gp_Pln P)
        {
            myBuilder.MakeFace(F, new Geom_Plane(P), Precision.Confusion());
        }

        internal void ReverseFace(TopoDS_Face F)
        {
            F.Reverse();
        }


        public void CompleteShell(TopoDS_Shell S)
        {
            S.Closed(BRep_Tool.IsClosed(S));
            BRepTools.Update(S);
        }

        //! Adds the Face <F>  in the Shell <Sh>.
        public void AddShellFace(TopoDS_Shell S, TopoDS_Face F)
        {
            myBuilder.Add(S, F);
        }

        public void SetPCurve(TopoDS_Edge E,

                   TopoDS_Face F,
                   gp_Circ2d C)
        {
            myBuilder.UpdateEdge(E, new Geom2d_Circle(C), F, Precision.Confusion());
        }

        public void SetPCurve(TopoDS_Edge E,

                  TopoDS_Face F,
                  gp_Lin2d L1,
                  gp_Lin2d L2)
        {
            TopoDS_Shape aLocalShape = E.Oriented(TopAbs_Orientation.TopAbs_FORWARD);
            myBuilder.UpdateEdge(TopoDS.Edge(aLocalShape),
                   new Geom2d_Line(L1),
                   new Geom2d_Line(L2),
                   F, Precision.Confusion());
            //  myBuilder.UpdateEdge(TopoDS::Edge(E.Oriented(TopAbs_FORWARD)),
            //		       new Geom2d_Line(L1),
            //		       new Geom2d_Line(L2),
            //		       F,Precision::Confusion());
            myBuilder.Continuity(E, F, F, GeomAbs_Shape.GeomAbs_CN);
        }

        public void SetPCurve(TopoDS_Edge E,
            TopoDS_Face F, gp_Lin2d L)
        {
            myBuilder.UpdateEdge(E, new Geom2d_Line(L), F, Precision.Confusion());
        }

        public void CompleteFace(TopoDS_Face F)
        {
            BRepTools.Update(F);
        }

        //! This is called once a wire is  completed. It gives
        //! the opportunity to perform any post treatment.
        public void CompleteWire(TopoDS_Wire W)
        {
            W.Closed(BRep_Tool.IsClosed(W));
            BRepTools.Update(W);
        }

        public void AddWireEdge(TopoDS_Wire W, TopoDS_Edge E, bool direct)
        {
            TopoDS_Edge EE = new TopoDS_Edge(E);
            if (!direct)
                EE.Reverse();
            myBuilder.Add(W, EE);
        }

        public void MakeWire(TopoDS_Wire W)
        {
            myBuilder.MakeWire(W);
        }

        public void AddEdgeVertex(TopoDS_Edge E,

                          TopoDS_Vertex V,
                          double P1,

                          double P2)
        {
            TopoDS_Vertex VV = new TopoDS_Vertex(V);//not origin
            VV.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
            myBuilder.Add(E, VV);  
            
            VV = new TopoDS_Vertex(V);//not origin, but logical
            VV.Orientation(TopAbs_Orientation.TopAbs_REVERSED);
            myBuilder.Add(E, VV);
            myBuilder.Range(E, P1, P2);
        }

        public void AddEdgeVertex(TopoDS_Edge E,
                        TopoDS_Vertex V,
                        double P,
                        bool direct)
        {
            //TopoDS_Vertex VV = V;
            TopoDS_Vertex VV = new TopoDS_Vertex(V);
            if (!direct)
                VV.Reverse();

            myBuilder.Add(E, VV);
            myBuilder.UpdateVertex(VV, P, E, Precision.Confusion());
        }


        public void MakeEdge(TopoDS_Edge E, gp_Circ C)
        {
            myBuilder.MakeEdge(E, new Geom_Circle(C), Precision.Confusion());
        }
        public void MakeEdge(TopoDS_Edge E, gp_Lin L)
        {
            myBuilder.MakeEdge(E, new Geom_Line(L), Precision.Confusion());
        }

        public void CompleteEdge(TopoDS_Edge E)
        {
            BRepTools.Update(E);
        }

        public void MakeVertex(TopoDS_Vertex V,
            gp_Pnt P)
        {
            myBuilder.MakeVertex(V, P, Precision.Confusion());

        }
    }
}
