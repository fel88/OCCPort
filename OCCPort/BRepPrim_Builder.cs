using System;

namespace OCCPort.Tester
{
    internal class BRepPrim_Builder
    {
        public BRepPrim_Builder()
        {
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
    }
}