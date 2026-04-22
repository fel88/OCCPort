using OCCPort.Interfaces;

namespace OCCPort
{
    //! Builds discrete model of a shape by adding faces and free edges.
    //! Computes deflection for corresponded shape and checks whether it
    //! fits existing polygonal representation. If not, cleans shape from
    //! outdated info.
    public class BRepMesh_ShapeVisitor : MeshTools_ShapeVisitor, IMeshTools_ShapeVisitor
    {
        IMeshData_Model myModel;
        IMeshData.DMapOfShapeInteger myDEdgeMap;
        //IMeshData::DMapOfShapeInteger myDEdgeMap;
        //=======================================================================
        // Function: Constructor
        // Purpose : 
        //=======================================================================
        public BRepMesh_ShapeVisitor(IMeshData_Model theModel)

        {
            myModel = (theModel);
            //myDEdgeMap(1, new NCollection_IncAllocator(IMeshData::MEMORY_BLOCK_SIZE_HUGE))
        }

        //=======================================================================
        // Function: addWire
        // Purpose : 
        //=======================================================================

        public override bool addWire(TopoDS_Wire theWire,
IMeshData_Face theDFace)
        {
            if (theWire.IsNull())
            {
                return false;
            }

            ShapeExtend_WireData aWireData = new ShapeExtend_WireData(theWire, true, false);
            ShapeAnalysis_Wire aWireTool = new ShapeAnalysis_Wire(aWireData, theDFace.GetFace(), Precision.Confusion());

            ShapeAnalysis_WireOrder aOrderTool = new ShapeAnalysis_WireOrder();
            aWireTool.CheckOrder(aOrderTool, true, false);
            if (aWireTool.LastCheckStatus(ShapeExtend_Status.ShapeExtend_FAIL))
            {
                return false;
            }

            if (aWireTool.LastCheckStatus(ShapeExtend_Status.ShapeExtend_DONE3))
            {
                theDFace.SetStatus(IMeshData_Status.IMeshData_UnorientedWire);
            }

            int aEdgesNb = aOrderTool.NbEdges();
            if (aEdgesNb != aWireData.NbEdges())
            {
                return false;
            }

            IWireHandle aDWire = theDFace.AddWire(theWire, aEdgesNb);
            for (int i = 1; i <= aEdgesNb; ++i)
            {
              /*  int  aEdgeIndex = aOrderTool.Ordered(i);
                 TopoDS_Edge aEdge = aWireData.Edge(aEdgeIndex);
                if (aEdge.Orientation() != TopAbs_Orientation.TopAbs_EXTERNAL)
                {
                    IEdgeHandle aDEdge = myModel.GetEdge(myDEdgeMap.Find(aEdge));

                    aDEdge.AddPCurve(theDFace, aEdge.Orientation());
                    aDWire.AddEdge(aDEdge, aEdge.Orientation());
                }*/
            }

            return true;
        }

        public override void Visit(TopoDS_Face theFace)
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

            TopExp_Explorer aWireIt = new TopExp_Explorer(theFace, TopAbs_ShapeEnum.TopAbs_WIRE);
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
        public override void Visit(TopoDS_Edge theEdge)
        {
            if (!myDEdgeMap.IsBound(theEdge))
            {
                myModel.AddEdge(theEdge);
                myDEdgeMap.Bind(theEdge, myModel.EdgesNb() - 1);
            }
        }

    }
}
