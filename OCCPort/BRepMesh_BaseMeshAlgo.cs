using OCCPort.Enums;
using OCCPort.Interfaces;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace OCCPort
{
    //! Class provides base functionality for algorithms building face triangulation.
    //! Performs initialization of BRepMesh_DataStructureOfDelaun and nodes map structures.
    public abstract class BRepMesh_BaseMeshAlgo : IMeshTools_MeshAlgo
    {
        //! Gets mesh structure.
        public BRepMesh_DataStructureOfDelaun getStructure()
        {
            return myStructure;
        }
        //! Gets discrete face.
        public IMeshData_Face getDFace()
        {
            return myDFace;
        }

        //! Registers the given point in vertex map and adds 2d point to mesh data structure.
        //! Returns index of node in the structure.
        public int registerNode(
  gp_Pnt thePoint,
  gp_Pnt2d thePoint2d,
  BRepMesh_DegreeOfFreedom theMovability,
  bool isForceAdd)
        {
            int aNodeIndex = addNodeToStructure(
              thePoint2d, myNodesMap.Size(), theMovability, isForceAdd);

            if (aNodeIndex > myNodesMap.Size())
            {
                myNodesMap.Append(thePoint);
            }

            return aNodeIndex;
        }
        VectorOfPnt myNodesMap;

        public int addNodeToStructure(
          gp_Pnt2d thePoint,
          int theLocation3d,
          BRepMesh_DegreeOfFreedom theMovability,
          bool isForceAdd)
        {
            BRepMesh_Vertex aNode = new BRepMesh_Vertex(thePoint.XY(), theLocation3d, theMovability);
            return myStructure.AddNode(aNode, isForceAdd);
        }

        Poly_Triangulation collectTriangles()
        {
            MapOfInteger aTriangles = myStructure.ElementsOfDomain();
            //if (aTriangles.IsEmpty())
            //{
            //    return new Poly_Triangulation();
            //}

            Poly_Triangulation aRes = new Poly_Triangulation();
            //aRes.ResizeTriangles(aTriangles.Extent(), false);
            IteratorOfMapOfInteger aTriIt = new IteratorOfMapOfInteger(aTriangles);
            for (int aTriangeId = 1; aTriIt.More(); aTriIt.Next(), ++aTriangeId)
            {
                //     BRepMesh_Triangle aCurElem = myStructure.GetElement(aTriIt.Key());

                int[] aNode = new int[3];
                //    myStructure.ElementNodes(aCurElem, aNode);

                //    for (int i = 0; i < 3; ++i)
                //    {
                //        if (!myUsedNodes.IsBound(aNode[i]))
                //        {
                //            myUsedNodes.Bind(aNode[i], myUsedNodes.Size() + 1);
                //        }

                //        aNode[i] = myUsedNodes.Find(aNode[i]);
                //    }

                aRes.SetTriangle(aTriangeId, new Poly_Triangle(aNode[0], aNode[1], aNode[2]));
            }
            //aRes.ResizeNodes(myUsedNodes.Extent(), false);
            //aRes.AddUVNodes();
            return aRes;
        }

        //! Commits generated triangulation to TopoDS face.
        private void commitSurfaceTriangulation()
        {
            Poly_Triangulation aTriangulation = collectTriangles();
            if (aTriangulation == null)
            {
                //myDFace.SetStatus(IMeshData_Failure);
                return;
            }

            collectNodes(aTriangulation);

            BRepMesh_ShapeTool.AddInFace(myDFace.GetFace(), aTriangulation);
        }
        //=======================================================================
        //function : collectNodes
        //purpose  :
        //=======================================================================
        public void collectNodes(Poly_Triangulation theTriangulation)
        {
            //for (int i = 1; i <= myNodesMap.Size(); ++i)
            {
                //if (myUsedNodes->IsBound(i))
                {
                    //BRepMesh_Vertex aVertex = myStructure.GetNode(i);

                    //const Standard_Integer aNodeIndex = myUsedNodes->Find(i);
                    //theTriangulation->SetNode(aNodeIndex, myNodesMap->Value(aVertex.Location3d()));
                    //theTriangulation->SetUVNode(aNodeIndex, getNodePoint2d(aVertex));
                }
            }
        }
        NCollection_IncAllocator myAllocator;

        //! Generates mesh for the contour stored in data structure.
        public abstract void generateMesh(Message_ProgressRange theRange);
        public void Perform(IMeshData_Face theDFace, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {
            try
            {
                myDFace = theDFace;
                myParameters = theParameters;
                myAllocator = new NCollection_IncAllocator(IMeshData.MEMORY_BLOCK_SIZE_HUGE);
                myStructure = new BRepMesh_DataStructureOfDelaun(myAllocator);
                //myNodesMap = new VectorOfPnt(256, myAllocator);
                //myUsedNodes = new DMapOfIntegerInteger(1, myAllocator);

                if (initDataStructure())
                {
                    // if (!theRange.More())
                    {
                        //   return;
                    }
                    generateMesh(theRange);
                    commitSurfaceTriangulation();
                }
            }
            catch (Standard_Failure  /*theException*/)
            {
            }

            //myDFace.Nullify(); // Do not hold link to face.
            myStructure = null;
            //myNodesMap.Nullify();
            //myUsedNodes.Nullify();
            //myAllocator.Nullify();
        }

        private bool initDataStructure()
        {

            for (int aWireIt = 0; aWireIt < myDFace.WiresNb(); ++aWireIt)
            {
                var aDWire = myDFace.GetWire(aWireIt);
                if (aDWire.IsSet(IMeshData_Status.IMeshData_SelfIntersectingWire))
                {
                    // TODO: here we can add points of self-intersecting wire as fixed points
                    // in order to keep consistency of nodes with adjacent faces.
                    continue;
                }

                for (int aEdgeIt = 0; aEdgeIt < aDWire.EdgesNb(); ++aEdgeIt)
                {
                    IMeshData_Edge aDEdge = aDWire.GetEdge(aEdgeIt);
                    ICurveHandle  aCurve = aDEdge.GetCurve();
                    IPCurveHandle aPCurve = aDEdge.GetPCurve(
                      myDFace, aDWire.GetEdgeOrientation(aEdgeIt));

                    int aPrevNodeIndex = -1;
                    int aLastPoint = aPCurve.ParametersNb() - 1;
                    for (int aPointIt = 0; aPointIt <= aLastPoint; ++aPointIt)
                    {
                        int aNodeIndex = registerNode(
                          aCurve.GetPoint(aPointIt),
                          aPCurve.GetPoint(aPointIt),
                          BRepMesh_DegreeOfFreedom.BRepMesh_Frontier, false/*aPointIt > 0 && aPointIt < aLastPoint*/);

                    }
                }

            }



            return true;
        }

        IMeshData_Face myDFace;
        IMeshTools_Parameters myParameters;
        // Handle(NCollection_IncAllocator)       myAllocator;

        //Handle(VectorOfPnt)                    myNodesMap;
        //Handle(DMapOfIntegerInteger)           myUsedNodes;

        BRepMesh_DataStructureOfDelaun myStructure;
    }

}