namespace OCCPort.Interfaces
{
    public abstract class MeshTools_ShapeVisitor : IMeshTools_ShapeVisitor
    {
        public abstract bool addWire(TopoDS_Wire theWire,
  IMeshData_Face theDFace);

        public void Visit(TopoDS_Face theFace)
        {
            BRepTools.Update(theFace);
            IMeshData_Face aDFace = myModel.AddFace(theFace);

            // Outer wire should always be the first in the model. 
            TopoDS_Wire aOuterWire = ShapeAnalysis.OuterWire(theFace);
            if (!addWire(aOuterWire, aDFace))
            {
                aDFace.SetStatus(IMeshData_Status.IMeshData_Failure);
                return;
            }

            TopExp_Explorer aWireIt=new TopExp_Explorer (theFace, TopAbs_ShapeEnum.TopAbs_WIRE);
            for (; aWireIt.More(); aWireIt.Next())
            {
                TopoDS_Wire aWire = TopoDS.Wire(aWireIt.Current());
                if (aWire.IsSame(aOuterWire))
                {
                    continue;
                }

                if (!addWire(aWire, aDFace))
                {
                    // If there is a failure on internal wire, just skip it.
                    // The most significant is an outer wire.
                    aDFace.SetStatus(IMeshData_Status.IMeshData_UnorientedWire);
                }
            }
        }
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