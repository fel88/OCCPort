using System;

namespace OCCPort
{
    internal class BRepPrim_Builder
    {
        public BRepPrim_Builder()
        {
        }
        public void MakeShell(TopoDS_Shell S)
        {
            myBuilder.MakeShell(S);
            S.Closed(true);
        }

        internal void AddFaceWire(TopoDS_Face topoDS_Face, object value)
        {
            throw new NotImplementedException();
        }



        BRep_Builder myBuilder;

        public void MakeFace(TopoDS_Face F, gp_Pln P)
        {
            myBuilder.MakeFace(F, new Geom_Plane(P), Precision.Confusion());
        }

        internal void ReverseFace(TopoDS_Face topoDS_Face)
        {
            throw new NotImplementedException();
        }

        internal void CompleteShell(TopoDS_Shell myShell)
        {
            throw new NotImplementedException();
        }
        public void AddShellFace(TopoDS_Shell S,

                     TopoDS_Face F)
        {
            myBuilder.Add(S, F);
        }

    }
}