namespace OCCPort
{
    //! Interface class for shape visitor.
    public class IMeshTools_ShapeVisitor
    {
        public void Visit(TopoDS_Edge theEdge)
        {
            if (!myDEdgeMap.IsBound(theEdge))
            {
                myModel.AddEdge(theEdge);
                myDEdgeMap.Bind(theEdge, myModel.EdgesNb() - 1);
            }
        }
        IMeshData_Model myModel;
        IMeshData.DMapOfShapeInteger myDEdgeMap;

    }
}