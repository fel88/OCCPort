using OCCPort;
using OCCPort.Common;
using TKBRep;
using TKG3d;

namespace TKTopAlgo
{
    [Aux]
    public static class AuxFunctions
    {
        [Aux]
        public static void Check1(TopoDS_Shape myShape)
        {
            Queue<TopoDS_Shape> q = new Queue<TopoDS_Shape>();
            q.Enqueue(myShape);
            while (q.Any())
            {
                var deq = q.Dequeue();
                if (deq.TShape() is TopoDS_TEdge)
                {
                    if (deq.TShape().InernalShapes.Length == 2)
                    {
                        var orients = deq.TShape().InernalShapes.Select(z => z.Orientation()).ToArray();
                        if (orients.Distinct().Count() == 1)
                        {
                            throw new Exception("wrong edge");
                        }
                    }
                }
                foreach (var item in deq.TShape().InernalShapes)
                {
                    q.Enqueue(item);
                }
            }
        }

        [Aux]
        public static TopAbs_Orientation[] Check2(TopoDS_Shape myShape)
        {
            Queue<TopoDS_Shape> q = new Queue<TopoDS_Shape>();
            q.Enqueue(myShape);
            List<TopAbs_Orientation> ret = new List<TopAbs_Orientation>();
            while (q.Any())
            {
                var deq = q.Dequeue();
                ret.Add(deq.Orientation());
                foreach (var item in deq.TShape().InernalShapes)
                {
                    q.Enqueue(item);
                }
            }
            return ret.ToArray();
        }
    }
}
