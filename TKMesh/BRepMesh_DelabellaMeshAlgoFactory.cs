using OCCPort;
using OCCPort.Common;
using System;
using TKBRep;
using TKernel;
using TKG2d;
using TKG3d;
using TKMath;

namespace TKMesh
{
    internal class BRepMesh_DelabellaMeshAlgoFactory : IMeshTools_MeshAlgoFactory
    {
        public IMeshTools_MeshAlgo GetAlgo(GeomAbs_SurfaceType theSurfaceType,
            ref IMeshTools_Parameters theParameters)
        {
            //=======================================================================
            // Function: GetAlgo
            // Purpose :
            //=======================================================================
            {
                var algo1 = new BRepMesh_DelaunayNodeInsertionMeshAlgo<BRepMesh_DefaultRangeSplitter>();
                var algo2 = new BRepMesh_DelabellaBaseMeshAlgo();
                switch (theSurfaceType)
                {

                    /*  struct BaseMeshAlgo
  {
    typedef BRepMesh_DelabellaBaseMeshAlgo Type;
  };*/
                    /*
  template<class RangeSplitter>
  struct NodeInsertionMeshAlgo
  {
    typedef BRepMesh_DelaunayNodeInsertionMeshAlgo<RangeSplitter, BRepMesh_CustomDelaunayBaseMeshAlgo<BRepMesh_DelabellaBaseMeshAlgo> > Type;
  };*/

                    case GeomAbs_SurfaceType.GeomAbs_Plane:
                        return theParameters.InternalVerticesMode ?
                          //new NodeInsertionMeshAlgo<BRepMesh_DefaultRangeSplitter>::Type :
                          algo1 :
                          //new BaseMeshAlgo::Type;
                          algo2;
                        break;

                    //case GeomAbs_Sphere:
                    //    {
                    //        NodeInsertionMeshAlgo<BRepMesh_SphereRangeSplitter>::Type* aMeshAlgo =
                    //          new NodeInsertionMeshAlgo<BRepMesh_SphereRangeSplitter>::Type;
                    //        aMeshAlgo->SetPreProcessSurfaceNodes(Standard_True);
                    //        return aMeshAlgo;
                    //    }
                    //    break;

                    //case GeomAbs_Cylinder:
                    //    return theParameters.InternalVerticesMode ?
                    //      new DefaultNodeInsertionMeshAlgo<BRepMesh_CylinderRangeSplitter>::Type :
                    //      new DefaultBaseMeshAlgo::Type;
                    //    break;

                    //case GeomAbs_Cone:
                    //    {
                    //        NodeInsertionMeshAlgo<BRepMesh_ConeRangeSplitter>::Type* aMeshAlgo =
                    //          new NodeInsertionMeshAlgo<BRepMesh_ConeRangeSplitter>::Type;
                    //        aMeshAlgo->SetPreProcessSurfaceNodes(Standard_True);
                    //        return aMeshAlgo;
                    //    }
                    //    break;

                    //case GeomAbs_Torus:
                    //    {
                    //        NodeInsertionMeshAlgo<BRepMesh_TorusRangeSplitter>::Type* aMeshAlgo =
                    //          new NodeInsertionMeshAlgo<BRepMesh_TorusRangeSplitter>::Type;
                    //        aMeshAlgo->SetPreProcessSurfaceNodes(Standard_True);
                    //        return aMeshAlgo;
                    //    }
                    //    break;

                    //case GeomAbs_SurfaceOfRevolution:
                    //    {
                    //        DeflectionControlMeshAlgo<BRepMesh_BoundaryParamsRangeSplitter>::Type* aMeshAlgo =
                    //          new DeflectionControlMeshAlgo<BRepMesh_BoundaryParamsRangeSplitter>::Type;
                    //        aMeshAlgo->SetPreProcessSurfaceNodes(Standard_True);
                    //        return aMeshAlgo;
                    //    }
                    //    break;

                    default:
                        {
                            /*DeflectionControlMeshAlgo<BRepMesh_NURBSRangeSplitter>::Type* aMeshAlgo =
                              new DeflectionControlMeshAlgo<BRepMesh_NURBSRangeSplitter>::Type;
                            aMeshAlgo->SetPreProcessSurfaceNodes(Standard_True);
                            return aMeshAlgo;*/
                            return null;
                        }
                }
            }
        }
    }

    //! Extends base Delaunay meshing algo in order to enable possibility 
    //! of addition of free vertices and internal nodes into the mesh.

    public class BRepMesh_DelaunayNodeInsertionMeshAlgo<RangeSplitter> : BRepMesh_NodeInsertionMeshAlgo<RangeSplitter> where RangeSplitter : AbstractRangeSplitter, new()
    {
        public BRepMesh_DelaunayNodeInsertionMeshAlgo()
        {

        }
        //! Returns size of cell to be used by acceleration circles grid structure.
        public override (int, int) getCellsCount(int theVerticesNb)
        {
            return BRepMesh_GeomTool.CellsCount(getDFace().GetSurface(), theVerticesNb,
                                                  getDFace().GetDeflection(),
                                                  getRangeSplitter());
        }

        //! Sets PreProcessSurfaceNodes flag.
        //! If TRUE, registers surface nodes before generation of base mesh.
        //! If FALSE, inserts surface nodes after generation of base mesh. 
        public void SetPreProcessSurfaceNodes(bool isPreProcessSurfaceNodes)
        {
            myIsPreProcessSurfaceNodes = isPreProcessSurfaceNodes;
        }

        bool myIsPreProcessSurfaceNodes;

        //! Performs processing of generated mesh. Generates surface nodes and inserts them into structure.
        public override void postProcessMesh(BRepMesh_Delaun theMesher,
                                Message_ProgressRange theRange)
        {
            if (!theRange.More())
            {
                return;
            }
            base.postProcessMesh(theMesher, new Message_ProgressRange()); // shouldn't be range passed here?

            if (!myIsPreProcessSurfaceNodes)
            {
                ListOfPnt2d aSurfaceNodes =
                  this.getRangeSplitter().GenerateSurfaceNodes(getParameters());

                insertNodes(aSurfaceNodes, theMesher, theRange);
            }
        }

        //! Performs initialization of data structure using existing model data.
        protected override bool initDataStructure()
        {
            if (!base.initDataStructure())
            {
                return false;
            }

            if (myIsPreProcessSurfaceNodes)
            {
                ListOfPnt2d aSurfaceNodes =
                 this.getRangeSplitter().GenerateSurfaceNodes(this.getParameters());

                registerSurfaceNodes(aSurfaceNodes);
            }

            return true;
        }
        //! Registers surface nodes in data structure.
        bool registerSurfaceNodes(ListOfPnt2d theNodes)
        {
            if (theNodes == null || theNodes.IsEmpty())
            {
                return false;
            }

            bool isAdded = false;
            //ListOfPnt2d::Iterator aNodesIt(*theNodes);
            foreach (var aNodesIt in theNodes)
            {
                gp_Pnt2d aPnt2d = aNodesIt;
                if (this.getClassifier().Perform(aPnt2d) == TopAbs_State.TopAbs_IN)
                {
                    isAdded = true;
                    this.registerNode(this.getRangeSplitter().Point(aPnt2d),
                                       aPnt2d, BRepMesh_DegreeOfFreedom.BRepMesh_Free, false);
                }
            }

            return isAdded;
        }

        //! Inserts nodes into mesh.
        bool insertNodes(
                    ListOfPnt2d theNodes,
                    BRepMesh_Delaun theMesher,
                    Message_ProgressRange theRange)
        {
            if (theNodes == null || theNodes.IsEmpty())
            {
                return false;
            }

            VectorOfInteger aVertexIndexes = new VectorOfInteger(theNodes.Size());
            foreach (var aNodesIt in theNodes)
            {
                gp_Pnt2d aPnt2d = aNodesIt;
                if (this.getClassifier().Perform(aPnt2d) == TopAbs_State.TopAbs_IN)
                {
                    aVertexIndexes.Append(this.registerNode(this.getRangeSplitter().Point(aPnt2d),
                        aPnt2d, BRepMesh_DegreeOfFreedom.BRepMesh_Free,
                      false));
                }
            }

            theMesher.AddVertices(aVertexIndexes, theRange);
            if (!theRange.More())
            {
                return false;
            }
            return !aVertexIndexes.IsEmpty();
        }

    }

    //! Extends base meshing algo in order to enable possibility 
    //! of addition of free vertices into the mesh.

    public class BRepMesh_NodeInsertionMeshAlgo<RangeSplitter> : BRepMesh_DelaunayBaseMeshAlgo where RangeSplitter : AbstractRangeSplitter, new() //: BaseAlgo
    {
        //! Returns range splitter.
        public RangeSplitter getRangeSplitter()
        {
            return myRangeSplitter;
        }

        //! Adds the given 2d point to mesh data structure.
        //! Returns index of node in the structure.
        public override int addNodeToStructure(
    gp_Pnt2d thePoint,
    int theLocation3d,
    BRepMesh_DegreeOfFreedom theMovability,
    bool isForceAdd)
        {
            return base.addNodeToStructure(
              myRangeSplitter.Scale(thePoint, true),
              theLocation3d, theMovability, isForceAdd);
        }


        //! Performs initialization of data structure using existing model data.
        protected override bool initDataStructure()
        {
            var aDFace = this.getDFace();
            NCollection_Array1<SequenceOfPnt2d> aWires = new NCollection_Array1<SequenceOfPnt2d>(0, aDFace.WiresNb() - 1);
            for (int aWireIt = 0; aWireIt < aDFace.WiresNb(); ++aWireIt)
            {
                var aDWire = aDFace.GetWire(aWireIt);
                if (aDWire.IsSet(IMeshData_Status.IMeshData_SelfIntersectingWire) ||
                   (aDWire.IsSet(IMeshData_Status.IMeshData_OpenWire) && aWireIt != 0))
                {
                    continue;
                }

                aWires[aWireIt] = collectWirePoints(aDWire);// todo: ??
            }

            myRangeSplitter.AdjustRange();
            if (!myRangeSplitter.IsValid())
            {
                aDFace.SetStatus(IMeshData_Status.IMeshData_Failure);
                return false;
            }

            var aDelta = myRangeSplitter.GetDelta();
            var aTolUV = myRangeSplitter.GetToleranceUV();
            double uCellSize = 14.0 * aTolUV.Item1;
            double vCellSize = 14.0 * aTolUV.Item2;

            this.getStructure().Data().SetCellSize(uCellSize / aDelta.Item1, vCellSize / aDelta.Item2);
            this.getStructure().Data().SetTolerance(aTolUV.Item1 / aDelta.Item1, aTolUV.Item2 / aDelta.Item2);

            for (int aWireIt = 0; aWireIt < aDFace.WiresNb(); ++aWireIt)
            {
                SequenceOfPnt2d aWire = aWires[aWireIt];
                if (aWire != null && !aWire.IsEmpty())
                {
                    myClassifier.RegisterWire(aWire, aTolUV,
                                               myRangeSplitter.GetRangeU(),
                                               myRangeSplitter.GetRangeV());
                }
            }

            if (this.getParameters().InternalVerticesMode)
            {
                insertInternalVertices();
            }

            return base.initDataStructure();
        }

        public class SequenceOfPnt2d : NCollection_Sequence<gp_Pnt2d>
        {

        }
        //! Iterates over internal vertices of a face and 
        //! creates corresponding nodes in data structure.
        void insertInternalVertices()
        {
            TopExp_Explorer aExplorer = new TopExp_Explorer(this.getDFace().GetFace(), TopAbs_ShapeEnum.TopAbs_VERTEX, TopAbs_ShapeEnum.TopAbs_EDGE);
            for (; aExplorer.More(); aExplorer.Next())
            {
                TopoDS_Vertex aVertex = TopoDS.Vertex(aExplorer.Current());
                if (aVertex.Orientation() != TopAbs_Orientation.TopAbs_INTERNAL)
                {
                    continue;
                }

                insertInternalVertex(aVertex);
            }
        }


        //! Inserts the given vertex into mesh.
        void insertInternalVertex(TopoDS_Vertex theVertex)
        {
            try
            {
                //OCC_CATCH_SIGNALS

                gp_Pnt2d aPnt2d = BRep_Tool.Parameters(theVertex, this.getDFace().GetFace());
                // check UV values for internal vertices
                if (myClassifier.Perform(aPnt2d) != TopAbs_State.TopAbs_IN)
                    return;

                this.registerNode(BRep_Tool.Pnt(theVertex), aPnt2d,
                                   BRepMesh_DegreeOfFreedom.BRepMesh_Fixed, false);
            }
            catch (Standard_Failure ex)
            {
            }
        }

        //! Creates collection of points representing discrete wire.
        SequenceOfPnt2d collectWirePoints(
    IWireHandle theDWire
    )
        {
            SequenceOfPnt2d aWirePoints = new SequenceOfPnt2d();
            for (int aEdgeIt = 0; aEdgeIt < theDWire.EdgesNb(); ++aEdgeIt)
            {
                var aDEdge = theDWire.GetEdge(aEdgeIt);
                var aPCurve = aDEdge.GetPCurve(
                  this.getDFace(), theDWire.GetEdgeOrientation(aEdgeIt));

                int aPointIt, aEndIndex, aInc;
                if (aPCurve.IsForward())
                {
                    // For an infinite cylinder (for example)
                    // aPCurve->ParametersNb() == 0

                    aEndIndex = aPCurve.ParametersNb() - 1;
                    aPointIt = Math.Min(0, aEndIndex);
                    aInc = 1;
                }
                else
                {
                    // For an infinite cylinder (for example)
                    // aPCurve->ParametersNb() == 0

                    aPointIt = aPCurve.ParametersNb() - 1;
                    aEndIndex = Math.Min(0, aPointIt);
                    aInc = -1;
                }

                // For an infinite cylinder (for example)
                // this cycle will not be executed.
                for (; aPointIt != aEndIndex; aPointIt += aInc)
                {
                    var aPnt2d = aPCurve.GetPoint(aPointIt);
                    aWirePoints.Append(aPnt2d);
                    myRangeSplitter.AddPoint(aPnt2d);
                }
            }

            return aWirePoints;
        }
        //! Performs processing of the given face.
        public override void Perform(
    IMeshData_Face theDFace,
     IMeshTools_Parameters theParameters,
     Message_ProgressRange theRange)
        {
            myRangeSplitter.Reset(theDFace, theParameters);
            myClassifier = new BRepMesh_Classifier();
            if (!theRange.More())
            {
                return;
            }
            base.Perform(theDFace, theParameters, theRange);
            myClassifier = null;
        }


        //! Returns classifier.
        public BRepMesh_Classifier getClassifier()
        {
            return myClassifier;
        }
        BRepMesh_Classifier myClassifier;

        RangeSplitter myRangeSplitter = new RangeSplitter();
    }

    //! Auxiliary class intended for classification of points
    //! regarding internals of discrete face.
    public class BRepMesh_Classifier
    {
        public void RegisterWire(
  NCollection_Sequence<gp_Pnt2d> theWire,
   (double, double) theTolUV,
   (double, double) theRangeU,
   (double, double) theRangeV)
        {
            int aNbPnts = theWire.Length();
            if (aNbPnts < 2)
            {
                return;
            }

            // Accumulate angle
            TColgp_Array1OfPnt2d aPClass = new TColgp_Array1OfPnt2d(1, aNbPnts);
            double anAngle = 0.0;
            gp_Pnt2d p1 = theWire[1], p2 = theWire[2], p3;
            aPClass[1] = p1;
            aPClass[2] = p2;

            double aAngTol = Precision.Angular();
            double aSqConfusion =
             Precision.PConfusion() * Precision.PConfusion();

            for (int i = 1; i <= aNbPnts; i++)
            {
                int ii = i + 2;
                if (ii > aNbPnts)
                {
                    p3 = aPClass[ii - aNbPnts];
                }
                else
                {
                    p3 = theWire.Value(ii);
                    aPClass[ii] = p3;
                }

                gp_Vec2d A = new gp_Vec2d(p1, p2), B = new gp_Vec2d(p2, p3);
                if (A.SquareMagnitude() > aSqConfusion &&
                    B.SquareMagnitude() > aSqConfusion)
                {
                    double aCurAngle = A.Angle(B);
                    double aCurAngleAbs = Math.Abs(aCurAngle);
                    // Check if vectors are opposite
                    if (aCurAngleAbs > aAngTol && (Math.PI - aCurAngleAbs) > aAngTol)
                    {
                        anAngle += aCurAngle;
                        p1 = p2;
                    }
                }
                p2 = p3;
            }
            // Check for zero angle - treat self intersecting wire as outer
            if (Math.Abs(anAngle) < aAngTol)
                anAngle = 0.0;

            myTabClass.Append(new CSLib_Class2d(
                              aPClass, theTolUV.Item1, theTolUV.Item2,
                              theRangeU.Item1, theRangeV.Item1,
                              theRangeU.Item2, theRangeV.Item2));

            myTabOrient.Append(!(anAngle < 0.0));
        }

        //! Performs classification of the given point regarding to face internals.
        //! @param thePoint Point in parametric space to be classified.
        //! @return TopAbs_IN if point lies within face boundaries and TopAbs_OUT elsewhere.
        public TopAbs_State Perform(gp_Pnt2d thePoint)
        {
            bool isOut = false;
            int aNb = myTabClass.Length();

            for (int i = 0; i < aNb; i++)
            {
                int aCur = myTabClass[i].SiDans(thePoint);
                if (aCur == 0)
                {
                    // Point is ON, but mark it as OUT
                    isOut = true;
                }
                else
                {
                    isOut = myTabOrient[i] ? (aCur == -1) : (aCur == 1);
                }

                if (isOut)
                {
                    return TopAbs_State.TopAbs_OUT;
                }
            }

            return TopAbs_State.TopAbs_IN;
        }
        NCollection_Vector<CSLib_Class2d> myTabClass = new NCollection_Vector<CSLib_Class2d>();
        VectorOfBoolean myTabOrient = new VectorOfBoolean();


    }

    //! Class provides base functionality to build face triangulation using Dealunay approach.
    //! Performs generation of mesh using raw data from model.
    public class BRepMesh_DelaunayBaseMeshAlgo : BRepMesh_ConstrainedBaseMeshAlgo

    {
        public override void generateMesh(Message_ProgressRange theRange)
        {
            BRepMesh_DataStructureOfDelaun aStructure = getStructure();
            VectorOfPnt aNodesMap = getNodesMap();

            VectorOfInteger aVerticesOrder = new VectorOfInteger(aNodesMap.Size());
            for (int i = 1; i <= aNodesMap.Size(); ++i)
            {
                aVerticesOrder.Append(i);
            }

            var aCellsCount = getCellsCount(aVerticesOrder.Size());
            BRepMesh_Delaun aMesher = new BRepMesh_Delaun(aStructure, aVerticesOrder, aCellsCount.Item1, aCellsCount.Item2);
            BRepMesh_MeshTool aCleaner = new BRepMesh_MeshTool(aStructure);
            aCleaner.EraseFreeLinks();

            if (!theRange.More())
            {
                return;
            }
            postProcessMesh(aMesher, theRange);
        }
    }

    public class VectorOfBoolean : NCollection_Vector<bool>
    {
    }
}

