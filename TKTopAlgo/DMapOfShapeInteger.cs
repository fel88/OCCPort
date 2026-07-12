using OCCPort;
using OCCPort.Common;
using TKBRep;
using TKernel;

namespace TKTopAlgo
{
  //typedef NCollection_Shared<NCollection_DataMap<TopoDS_Shape, Standard_Integer, TopTools_ShapeMapHasher> >     DMapOfShapeInteger;
    
    public class DMapOfShapeInteger: NCollection_DataMap<TopoDS_Shape, int, TopTools_ShapeMapHasher >
    {
        //public List<DMapOfShapeIntegerItem> Items = new List<DMapOfShapeIntegerItem>();

        //public void Bind(TopoDS_Edge theEdge, int v)
        //{
        //    Items.Add(new DMapOfShapeIntegerItem() { Int = v, Shape = theEdge });
        //}

        //public int Find(TopoDS_Edge aEdge)
        //{
        //    return Items.First(z => z.Shape == aEdge).Int;
        //}

        //public bool IsBound(TopoDS_Edge theEdge)
        //{
        //    return Items.Any(z => z.Shape == theEdge);
        //}

        //public class DMapOfShapeIntegerItem
        //{
        //    public TopoDS_Shape Shape;
        //    public int Int;

        //}
    }
}