using OCCPort.Tester;
using System;

namespace OCCPort
{
    internal class TopExp
    {  //! Returns the Vertex of orientation FORWARD in E. If
       //! there is none returns a Null Shape.
       //! CumOri = True : taking account the edge orientation
        public static TopoDS_Vertex FirstVertex(TopoDS_Edge E,
                   bool CumOri = false)
        {
            TopoDS_Iterator ite = new TopoDS_Iterator(E, CumOri);
            while (ite.More())
            {
                if (ite.Value().Orientation() == TopAbs_Orientation.TopAbs_FORWARD)
                    return TopoDS.Vertex(ite.Value());
                ite.Next();
            }
            return new TopoDS_Vertex();
        }
        //! Returns the Vertex of orientation REVERSED in E. If
        //! there is none returns a Null Shape.
        //! CumOri = True : taking account the edge orientation
        public static TopoDS_Vertex LastVertex(TopoDS_Edge E,
                  bool CumOri = false)
        {
            TopoDS_Iterator ite = new TopoDS_Iterator(E, CumOri);
            while (ite.More())
            {
                if (ite.Value().Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
                    return TopoDS.Vertex(ite.Value());
                ite.Next();
            }
            return new TopoDS_Vertex();
        }

        internal static void MapShapesAndAncestors(TopoDS_Shape S,
                    TopAbs_ShapeEnum TS,
                    TopAbs_ShapeEnum TA,
                   TopTools_IndexedDataMapOfShapeListOfShape M)
        {
            TopTools_ListOfShape empty = new TopTools_ListOfShape();

            // visit ancestors
            TopExp_Explorer exa = new TopExp_Explorer(S, TA);
            while (exa.More())
            {
                // visit shapes
                TopoDS_Shape anc = exa.Current();
                TopExp_Explorer exs = new TopExp_Explorer(anc, TS);
                while (exs.More())
                {
                    int index = M.FindIndex(exs.Current());
                    if (index == 0) index = M.Add(exs.Current(), empty);
                    M.Get(index).Append(anc);
                    exs.Next();
                }
                exa.Next();
            }

            // visit shapes not under ancestors
            TopExp_Explorer ex = new TopExp_Explorer(S, TS, TA);
            while (ex.More())
            {
                int index = M.FindIndex(ex.Current());
                if (index == 0) index = M.Add(ex.Current(), empty);
                ex.Next();
            }
        }

        internal static void Vertices(object v, out TopoDS_Vertex aVFirst, out TopoDS_Vertex aVLast)
        {
            throw new NotImplementedException();
        }
    }
}