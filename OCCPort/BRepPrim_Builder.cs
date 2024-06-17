using System;

namespace OCCPort
{
    internal class BRepPrim_Builder
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

        public void AddShellFace(TopoDS_Shell S,

                         TopoDS_Face F)
        {
            myBuilder.Add(S, F);
        }

        internal void SetPCurve(TopoDS_Edge E, 
            TopoDS_Face F, gp_Lin2d L)
        {
            myBuilder.UpdateEdge(E, new Geom2d_Line(L), F, Precision.Confusion());
        }

        internal void CompleteFace(TopoDS_Face F)
        {
            BRepTools.Update(F);
        }

        internal void CompleteWire(TopoDS_Wire W)
        {
            W.Closed(BRep_Tool.IsClosed(W));
            BRepTools.Update(W);
        }

        internal void AddWireEdge(TopoDS_Wire W, TopoDS_Edge E, bool direct)
        {
            TopoDS_Edge EE = E;
            if (!direct)
                EE.Reverse();
            myBuilder.Add(W, EE);
        }

        internal void MakeWire(TopoDS_Wire W)
        {
            myBuilder.MakeWire(W);
        }
        public void AddEdgeVertex(TopoDS_Edge E,
                        TopoDS_Vertex V,
                        double P,
                        bool direct)
        {
            TopoDS_Vertex VV = V;
            if (!direct)
                VV.Reverse();

            myBuilder.Add(E, VV);
            //myBuilder.UpdateVertex(VV, P, E, Precision.Confusion());
        }


        internal void MakeEdge(TopoDS_Edge E, gp_Lin L)
        {
            myBuilder.MakeEdge(E, new Geom_Line(L), Precision.Confusion());
        }

        internal void CompleteEdge(TopoDS_Edge E)
        {
            BRepTools.Update(E);
        }

        internal void MakeVertex(TopoDS_Vertex topoDS_Vertex, object v)
        {
            throw new NotImplementedException();
        }
    }
}