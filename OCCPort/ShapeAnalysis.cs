using System;
using System.Reflection.Metadata;

namespace OCCPort
{
    internal class ShapeAnalysis
    {

        public static double TotCross2D(ShapeExtend_WireData sewd,
                                        TopoDS_Face aFace)
        {
            int i, nbc = 0;
            gp_Pnt2d fuv=new gp_Pnt2d (), luv, uv0;
            double totcross = 0;
            for (i = 1; i <= sewd.NbEdges(); i++)
            {
                TopoDS_Edge edge = sewd.Edge(i);
                double f2d, l2d;
                //Geom2d_Curve c2d = BRep_Tool.CurveOnSurface(edge, aFace, f2d, l2d);
                //if (c2d!=null)
                //{
                //    nbc++;
                //    TColgp_SequenceOfPnt2d SeqPnt;
                //    ShapeAnalysis_Curve.GetSamplePoints(c2d, f2d, l2d, SeqPnt);
                //    if (edge.Orientation() == 1)
                //        ReverseSeq(SeqPnt);
                //    if (nbc == 1)
                //    {
                //        fuv = SeqPnt.Value(1);
                //        uv0 = fuv;
                //    }
                //    int j = 1;
                //    for (; j <= SeqPnt.Length(); j++)
                //    {
                //        luv = SeqPnt.Value(j);
                //        totcross += (fuv.X() - luv.X()) * (fuv.Y() + luv.Y()) / 2;
                //        fuv = luv;
                //    }
                //}
            }
            //totcross += (fuv.X() - uv0.X()) * (fuv.Y() + uv0.Y()) / 2;
            return totcross;
        }

        //! Returns positively oriented wire in the face.
        //! If there is no such wire - returns the last wire of the face.
        internal static TopoDS_Wire OuterWire(TopoDS_Face theFace)
        {
            TopoDS_Face aF = theFace;
            aF.Orientation(TopAbs_Orientation.TopAbs_FORWARD);

            TopExp_Explorer anIt = new TopExp_Explorer(aF, TopAbs_ShapeEnum.TopAbs_WIRE);
            while (anIt.More())
            {
                TopoDS_Wire aWire = TopoDS.Wire(anIt.Value());
                anIt.Next();

                // if current wire is the last one, return it without analysis
                if (!anIt.More())
                    return aWire;

                // Check if the wire has positive area
                ShapeExtend_WireData aSEWD = new ShapeExtend_WireData(aWire);
                double anArea2d = ShapeAnalysis.TotCross2D(aSEWD, aF);
                if (anArea2d >= 0.0)
                    return aWire;
            }
            return new TopoDS_Wire();
        }
    }
}