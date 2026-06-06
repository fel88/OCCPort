using OCCPort;
using TKBRep;
using TKernel;
using TKG3d;
using TKMath;
using TKShHealing;
using TKTopAlgo;

namespace TKMesh
{
    internal class BRepMesh_ModelBuilder : IMeshTools_ModelBuilder
    {
        //=======================================================================
        // Function: Perform
        // Purpose : 
        //=======================================================================
        public override IMeshData_Model performInternal(
  TopoDS_Shape theShape,
  IMeshTools_Parameters theParameters)
        {
            BRepMeshData_Model aModel = null;

            Bnd_Box aBox = new Bnd_Box();
            BRepBndLib.Add(theShape, aBox, false);

            if (!aBox.IsVoid())
            {
                // Build data model for further processing.
                aModel = new BRepMeshData_Model(theShape);

                if (theParameters.Relative)
                {
                    double aMaxSize = 0;
                    BRepMesh_ShapeTool.BoxMaxDimension(aBox, ref aMaxSize);
                    aModel.SetMaxSize(aMaxSize);
                }
                else
                {
                    aModel.SetMaxSize(Math.Max(theParameters.Deflection,
                                           theParameters.DeflectionInterior));
                }

                IMeshTools_ShapeVisitor aVisitor = new BRepMesh_ShapeVisitor(aModel);

                IMeshTools_ShapeExplorer aExplorer = new MeshTools_ShapeExplorer(theShape);
                aExplorer.Accept(aVisitor);
                SetStatus(Message_Status.Message_Done1);
            }
            else
            {
                SetStatus(Message_Status.Message_Fail1);
            }

            return aModel;
        }

        //! Sets maximum size of shape's bounding box.
        public void SetMaxSize(double theValue)
        {
            myMaxSize = theValue;
        }

        double myMaxSize;
        /*Handle(NCollection_IncAllocator) myAllocator;
  IMeshData::VectorOfIFaceHandles myDFaces;
		IMeshData::VectorOfIEdgeHandles myDEdges;*/
    }

    //! Builds discrete model of a shape by adding faces and free edges.
    //! Computes deflection for corresponded shape and checks whether it
    //! fits existing polygonal representation. If not, cleans shape from
    //! outdated info.
    public class BRepMesh_ShapeVisitor : MeshTools_ShapeVisitor, IMeshTools_ShapeVisitor
    {
        IMeshData_Model myModel;
        DMapOfShapeInteger myDEdgeMap;
        //IMeshData::DMapOfShapeInteger myDEdgeMap;
        //=======================================================================
        // Function: Constructor
        // Purpose : 
        //=======================================================================
        public BRepMesh_ShapeVisitor(IMeshData_Model theModel)

        {
            myModel = (theModel);
            myDEdgeMap = new DMapOfShapeInteger(/*1, new NCollection_IncAllocator(IMeshData::MEMORY_BLOCK_SIZE_HUGE)*/);
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
                int aEdgeIndex = aOrderTool.Ordered(i);
                TopoDS_Edge aEdge = aWireData.Edge(aEdgeIndex);
                if (aEdge.Orientation() != TopAbs_Orientation.TopAbs_EXTERNAL)
                {
                    IMeshData_Edge aDEdge = myModel.GetEdge(myDEdgeMap.Find(aEdge));

                    aDEdge.AddPCurve(theDFace, aEdge.Orientation());
                    aDWire.AddEdge(aDEdge, aEdge.Orientation());
                }
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


    //! Explores TopoDS_Shape for parts to be meshed - faces and free edges.
    public class MeshTools_ShapeExplorer : AbstractMeshData_Shape, IMeshTools_ShapeExplorer
    {//=======================================================================
     // Function: Constructor
     // Purpose : 
     //=======================================================================
        public MeshTools_ShapeExplorer(TopoDS_Shape theShape)
            : base(theShape)
        {
        }

        //=======================================================================
        // Function: visitEdges
        // Purpose : Explodes the given shape on edges according to the specified
        //           criteria and visits each one in order to add it to data model.
        //=======================================================================
        void visitEdges(IMeshTools_ShapeVisitor theVisitor,
                   TopoDS_Shape theShape,
                   bool isResetLocation,
                   TopAbs_ShapeEnum theToFind,
                   TopAbs_ShapeEnum theToAvoid = TopAbs_ShapeEnum.TopAbs_SHAPE)
        {

            TopExp_Explorer aEdgesIt = new TopExp_Explorer(theShape, theToFind, theToAvoid);
            for (; aEdgesIt.More(); aEdgesIt.Next())
            {
                TopoDS_Edge aEdge = TopoDS.Edge(aEdgesIt.Current());
                if (!BRep_Tool.IsGeometric(aEdge))
                {
                    continue;
                }

                theVisitor.Visit(isResetLocation ?
                  TopoDS.Edge(aEdge.Located(new TopLoc_Location())) :
                  aEdge);
            }
        }

        //=======================================================================
        // Function: Accept
        // Purpose : 
        //=======================================================================
        public void Accept(IMeshTools_ShapeVisitor theVisitor)
        {
            // Explore all free edges in shape.
            visitEdges(theVisitor, GetShape(), true, TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE);

            // Explore all related to some face edges in shape.
            // make array of faces suitable for processing (excluding faces without surface)
            TopTools_ListOfShape aFaceList = new TopTools_ListOfShape();
            BRepLib.ReverseSortFaces(GetShape(), aFaceList);
            TopTools_MapOfShape aFaceMap = new TopTools_MapOfShape();

            TopLoc_Location aEmptyLoc = new TopLoc_Location();
            //TopTools_ListIteratorOfListOfShape aFaceIter(aFaceList);

            foreach (var aFaceIter in aFaceList)
            {
                TopoDS_Shape aFaceNoLoc = aFaceIter;
                aFaceNoLoc.Location(aEmptyLoc);
                if (!aFaceMap.Add(aFaceNoLoc))
                {
                    continue; // already processed
                }

                TopoDS_Face aFace = TopoDS.Face(aFaceIter);
                if (!BRep_Tool.IsGeometric(aFace))
                {
                    continue;
                }

                //	// Explore all edges in face.
                visitEdges(theVisitor, aFace, false, TopAbs_ShapeEnum.TopAbs_EDGE);

                //	// Store only forward faces in order to prevent inverse issue.
                theVisitor.Visit(TopoDS.Face(aFace.Oriented(TopAbs_Orientation.TopAbs_FORWARD)));
            }
        }
    }


}


