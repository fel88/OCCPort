global using DMapOfIntegerInteger = TKernel.NCollection_DataMap<int, int>;

using OCCPort;
using OCCPort.Common;
using System;
using TKBRep;
using TKernel;
using TKG3d;
using TKMath;
using TKTopAlgo;
using static System.Net.Mime.MediaTypeNames;

namespace TKMesh
{
    //! Default implementation of IMeshTools_MeshAlgoFactory providing algorithms 
    //! of different complexity depending on type of target surface.
    public class BRepMesh_MeshAlgoFactory : IMeshTools_MeshAlgoFactory
    {
        public IMeshTools_MeshAlgo GetAlgo(GeomAbs_SurfaceType theSurfaceType, ref IMeshTools_Parameters theParameters)
        {
            var algo1 = new BRepMesh_DelaunayNodeInsertionMeshAlgo<BRepMesh_DefaultRangeSplitter>();
            var algo2 = new BRepMesh_DelabellaBaseMeshAlgo();
            var algo3 = new BRepMesh_DelaunayDeflectionControlMeshAlgo<BRepMesh_DefaultRangeSplitter>();
            switch (theSurfaceType)
            {
                /**
                 *   struct DeflectionControlMeshAlgo
  {
    typedef BRepMesh_DelaunayDeflectionControlMeshAlgo<RangeSplitter, BRepMesh_DelaunayBaseMeshAlgo> Type;
  };
                 */
                case GeomAbs_SurfaceType.GeomAbs_Plane:
                    return theParameters.EnableControlSurfaceDeflectionAllSurfaces ?
                        algo3 : theParameters.InternalVerticesMode ?
                        algo1 : algo2;
                    /* new DeflectionControlMeshAlgo<BRepMesh_DefaultRangeSplitter>::Type :
                       (theParameters.InternalVerticesMode ?
                        new NodeInsertionMeshAlgo<BRepMesh_DefaultRangeSplitter>::Type :
                        new BaseMeshAlgo::Type);*/

                    break;
            }
            return null;
        }
    }

    //! Class provides base functionality to build face triangulation using Delabella project.
    //! Performs generation of mesh using raw data from model.
    public class BRepMesh_DelabellaBaseMeshAlgo : BRepMesh_CustomBaseMeshAlgo
    {
        public override void buildBaseTriangulation()
        {
            BRepMesh_DataStructureOfDelaun aStructure = this.getStructure();

            Bnd_B2d aBox = new Bnd_B2d();
            int aNodesNb = aStructure.NbNodes();
            //List<Standard_Real> aPoints(2 * (aNodesNb + 4));
            double[] aPoints = new double[(2 * (aNodesNb + 4))];
            for (int aNodeIt = 0; aNodeIt < aNodesNb; ++aNodeIt)
            {
                BRepMesh_Vertex aVertex = aStructure.GetNode(aNodeIt + 1);

                int aBaseIdx = 2 * (aNodeIt);
                aPoints[aBaseIdx + 0] = aVertex.Coord().X();
                aPoints[aBaseIdx + 1] = aVertex.Coord().Y();

                aBox.Add(new gp_Pnt2d(aVertex.Coord()));
            }

            aBox.Enlarge(0.1 * (aBox.CornerMax() - aBox.CornerMin()).Modulus());
            gp_XY aMin = aBox.CornerMin();
            gp_XY aMax = aBox.CornerMax();

            aPoints[2 * aNodesNb + 0] = aMin.X();
            aPoints[2 * aNodesNb + 1] = aMin.Y();
            aStructure.AddNode(new BRepMesh_Vertex(
              aPoints[2 * aNodesNb + 0],
              aPoints[2 * aNodesNb + 1], BRepMesh_DegreeOfFreedom.BRepMesh_Free));

            aPoints[2 * aNodesNb + 2] = aMax.X();
            aPoints[2 * aNodesNb + 3] = aMin.Y();
            aStructure.AddNode(new BRepMesh_Vertex(
              aPoints[2 * aNodesNb + 2],
              aPoints[2 * aNodesNb + 3], BRepMesh_DegreeOfFreedom.BRepMesh_Free));

            aPoints[2 * aNodesNb + 4] = aMax.X();
            aPoints[2 * aNodesNb + 5] = aMax.Y();
            aStructure.AddNode(new BRepMesh_Vertex(
              aPoints[2 * aNodesNb + 4],
              aPoints[2 * aNodesNb + 5], BRepMesh_DegreeOfFreedom.BRepMesh_Free));

            aPoints[2 * aNodesNb + 6] = aMin.X();
            aPoints[2 * aNodesNb + 7] = aMax.Y();
            aStructure.AddNode(new BRepMesh_Vertex(
              aPoints[2 * aNodesNb + 6],
              aPoints[2 * aNodesNb + 7], BRepMesh_DegreeOfFreedom.BRepMesh_Free));

            double aDiffX = (aMax.X() - aMin.X());
            double aDiffY = (aMax.Y() - aMin.Y());
            for (int i = 0; i < aPoints.Length; i += 2)
            {
                aPoints[i + 0] = (aPoints[i + 0] - aMin.X()) / aDiffX - 0.5;
                aPoints[i + 1] = (aPoints[i + 1] - aMin.Y()) / aDiffY - 0.5;
            }

            //IDelaBella aTriangulator = new ShewchukTriangulator();
            IDelaBella aTriangulator = new CDelaBella();
            if (aTriangulator == null) // should never happen
            {
                throw new Standard_ProgramError("BRepMesh_DelabellaBaseMeshAlgo::buildBaseTriangulation: unable creating a triangulation algorithm");
            }

            //          aTriangulator->SetErrLog(logDelabella2Occ, NULL);
            //          try
            //          {
            int aVerticesNb = aTriangulator.Triangulate((int)(aPoints.Length / 2), aPoints);

            if (aVerticesNb > 0)
            {
                DelaBella_Triangle aTrianglePtr = aTriangulator.GetFirstDelaunayTriangle();
                while (aTrianglePtr != null)
                {
                    int[] aNodes = {
                    aTrianglePtr.v[0].i + 1,
                    aTrianglePtr.v[2].i + 1,
                    aTrianglePtr.v[1].i + 1
                  };

                    int[] aEdges = new int[3];
                    bool[] aOrientations = new bool[3];
                    for (int k = 0; k < 3; ++k)
                    {
                        BRepMesh_Edge aLink = new BRepMesh_Edge(aNodes[k], aNodes[(k + 1) % 3], BRepMesh_DegreeOfFreedom.BRepMesh_Free);

                        int aLinkInfo = aStructure.AddLink(aLink);
                        aEdges[k] = Math.Abs(aLinkInfo);
                        aOrientations[k] = aLinkInfo > 0;
                    }

                    BRepMesh_Triangle aTriangle = new BRepMesh_Triangle(aEdges, aOrientations, BRepMesh_DegreeOfFreedom.BRepMesh_Free);
                    aStructure.AddElement(aTriangle);

                    aTrianglePtr = aTrianglePtr.next;
                }
            }

            //              aTriangulator->Destroy();
            //              aTriangulator = NULL;
            //          }
            //          catch (Standard_Failure const&theException)
            //{
            //              if (aTriangulator != NULL)
            //              {
            //                  aTriangulator->Destroy();
            //                  aTriangulator = NULL;
            //              }

            //              throw Standard_Failure(theException);
            //          }
            //catch (...)
            //{
            //              if (aTriangulator != NULL)
            //              {
            //                  aTriangulator->Destroy();
            //                  aTriangulator = NULL;
            //              }

            //              throw Standard_Failure("BRepMesh_DelabellaBaseMeshAlgo::buildBaseTriangulation: exception in triangulation algorithm");
        }
    }

    //! Extends node insertion Delaunay meshing algo in order to control 
    //! deflection of generated trianges. Splits triangles failing the check.
    //template<class RangeSplitter, class BaseAlgo>
    class BRepMesh_DelaunayDeflectionControlMeshAlgo<RangeSplitter> : BRepMesh_DelaunayNodeInsertionMeshAlgo<RangeSplitter> where RangeSplitter : AbstractRangeSplitter, new()//<RangeSplitter, BaseAlgo>
    {
    }
    //! Class provides base functionality to build face triangulation using custom triangulation algorithm.
    //! Performs generation of mesh using raw data from model.
    public abstract class BRepMesh_CustomBaseMeshAlgo : BRepMesh_ConstrainedBaseMeshAlgo
    {

        //! Builds base triangulation using custom triangulation algorithm.
        public abstract void buildBaseTriangulation();

        //! Generates mesh for the contour stored in data structure.
        public override void generateMesh(Message_ProgressRange theRange)
        {
            BRepMesh_DataStructureOfDelaun aStructure = this.getStructure();
            int aNodesNb = aStructure.NbNodes();

            buildBaseTriangulation();

            (int, int) aCellsCount = getCellsCount(aStructure.NbNodes());
            BRepMesh_Delaun aMesher = new BRepMesh_Delaun(aStructure, aCellsCount.Item1, aCellsCount.Item2, false);

            int aNewNodesNb = aStructure.NbNodes();
            bool isRemoveAux = aNewNodesNb > aNodesNb;
            if (isRemoveAux)
            {
                VectorOfInteger aAuxVertices = new VectorOfInteger(aNewNodesNb - aNodesNb);
                for (int aExtNodesIt = aNodesNb + 1; aExtNodesIt <= aNewNodesNb; ++aExtNodesIt)
                {
                    aAuxVertices.Append(aExtNodesIt);
                }

                // Set aux vertices if there are some to clean up mesh correctly.
                aMesher.SetAuxVertices(aAuxVertices);
            }

            aMesher.ProcessConstraints();

            // Destruction of triangles containing aux vertices added (possibly) during base mesh computation.
            if (isRemoveAux)
            {
                aMesher.RemoveAuxElements();
            }

            BRepMesh_MeshTool aCleaner = new BRepMesh_MeshTool(aStructure);
            aCleaner.EraseFreeLinks();

            postProcessMesh(aMesher, theRange);
        }
    }

    //! Auxiliary tool providing API for manipulation with BRepMesh_DataStructureOfDelaun.
    internal class BRepMesh_MeshTool
    {
        BRepMesh_DataStructureOfDelaun myStructure;

        public BRepMesh_MeshTool(BRepMesh_DataStructureOfDelaun theStructure)
        {

            myStructure = (theStructure);


        }

        public void EraseFreeLinks()
        {
            for (int i = 1; i <= myStructure.NbLinks(); i++)
            {
                if (myStructure.ElementsConnectedTo(i).IsEmpty())
                {
                    BRepMesh_Edge anEdge = (BRepMesh_Edge)myStructure.GetLink(i);
                    if (anEdge.Movability() == BRepMesh_DegreeOfFreedom.BRepMesh_Deleted)
                    {
                        continue;
                    }

                    anEdge.SetMovability(BRepMesh_DegreeOfFreedom.BRepMesh_Free);
                    myStructure.RemoveLink(i);
                }
            }
        }
    }
    //! Class provides base functionality to build face triangulation using Dealunay approach.
    //! Performs generation of mesh using raw data from model.
    public abstract class BRepMesh_ConstrainedBaseMeshAlgo : BRepMesh_BaseMeshAlgo
    {  //! Returns size of cell to be used by acceleration circles grid structure.
        public virtual (int, int) getCellsCount(int theVerticesNb)
        {
            return (-1, -1);
        }
        //! Performs processing of generated mesh.
        //! By default does nothing.
        //! Expected to be called from method generateMesh() in successor classes.
        public virtual void postProcessMesh(BRepMesh_Delaun theMesher,
                                     Message_ProgressRange theRange)
        {
        }

    }
    //! Class provides base functionality for algorithms building face triangulation.
    //! Performs initialization of BRepMesh_DataStructureOfDelaun and nodes map structures.
    public abstract class BRepMesh_BaseMeshAlgo : IMeshTools_MeshAlgo
    {
        //! Gets mesh structure.
        public BRepMesh_DataStructureOfDelaun getStructure()
        {
            return myStructure;
        }


        //! Gets meshing parameters.
        protected IMeshTools_Parameters getParameters()
        {
            return myParameters;
        }

        //! Gets discrete face.
        public IMeshData_Face getDFace()
        {
            return myDFace;
        }
        //! Gets 3d nodes map.
        public VectorOfPnt getNodesMap()
        {
            return myNodesMap;
        }

        int addLinkToMesh(
  int theFirstNodeId,
  int theLastNodeId,
  TopAbs_Orientation theOrientation)
        {
            int aLinkIndex;
            if (theOrientation == TopAbs_Orientation.TopAbs_REVERSED)
                aLinkIndex = myStructure.AddLink(new BRepMesh_Edge(theLastNodeId, theFirstNodeId, BRepMesh_DegreeOfFreedom.BRepMesh_Frontier));
            else if (theOrientation == TopAbs_Orientation.TopAbs_INTERNAL)
                aLinkIndex = myStructure.AddLink(new BRepMesh_Edge(theFirstNodeId, theLastNodeId, BRepMesh_DegreeOfFreedom.BRepMesh_Fixed));
            else
                aLinkIndex = myStructure.AddLink(new BRepMesh_Edge(theFirstNodeId, theLastNodeId, BRepMesh_DegreeOfFreedom.BRepMesh_Frontier));

            return Math.Abs(aLinkIndex);
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

        public virtual int addNodeToStructure(
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
            if (aTriangles.IsEmpty())
            {
                return null;
            }

            Poly_Triangulation aRes = new Poly_Triangulation();
            aRes.ResizeTriangles(aTriangles.Extent(), false);
            //IteratorOfMapOfInteger aTriIt = new IteratorOfMapOfInteger(aTriangles);

            for (int aTriangeId = 1; aTriangeId <= aTriangles.Count; aTriangeId++)
            {
                int item = aTriangles.ToArray()[aTriangeId - 1];
                BRepMesh_Triangle aCurElem = myStructure.GetElement(item);

                int[] aNode = new int[3];
                myStructure.ElementNodes(aCurElem, aNode);

                for (int i = 0; i < 3; ++i)
                {
                    if (!myUsedNodes.IsBound(aNode[i]))
                    {
                        myUsedNodes.Bind(aNode[i], myUsedNodes.Size() + 1);
                    }

                    aNode[i] = myUsedNodes.Find(aNode[i]);
                }

                aRes.SetTriangle(aTriangeId, new Poly_Triangle(aNode[0], aNode[1], aNode[2]));
            }
            aRes.ResizeNodes(myUsedNodes.Extent(), false);
            aRes.AddUVNodes();
            return aRes;
        }

        //! Commits generated triangulation to TopoDS face.
        private void commitSurfaceTriangulation()
        {
            Poly_Triangulation aTriangulation = collectTriangles();
            if (aTriangulation == null)
            {
                myDFace.SetStatus(IMeshData_Status.IMeshData_Failure);
                return;
            }

            collectNodes(aTriangulation);

            BRepMesh_ShapeTool.AddInFace(myDFace.GetFace(), aTriangulation);
        }

        //=======================================================================
        public gp_Pnt2d getNodePoint2d(BRepMesh_Vertex theVertex)
        {
            return new gp_Pnt2d(theVertex.Coord());
        }

        //=======================================================================
        //function : collectNodes
        //purpose  :
        //=======================================================================
        public void collectNodes(Poly_Triangulation theTriangulation)
        {
            for (int i = 1; i <= myNodesMap.Size(); ++i)
            {
                if (myUsedNodes.IsBound(i))
                {
                    BRepMesh_Vertex aVertex = myStructure.GetNode(i);

                    int aNodeIndex = myUsedNodes.Find(i);
                    theTriangulation.SetNode(aNodeIndex, myNodesMap.Value(aVertex.Location3d()));
                    theTriangulation.SetUVNode(aNodeIndex, getNodePoint2d(aVertex));
                }
            }
        }
        NCollection_IncAllocator myAllocator;

        //! Generates mesh for the contour stored in data structure.
        public abstract void generateMesh(Message_ProgressRange theRange);
        public virtual void Perform(IMeshData_Face theDFace, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {
            try
            {
                myDFace = theDFace;
                myParameters = theParameters;
                myAllocator = new NCollection_IncAllocator(BRepLib.MEMORY_BLOCK_SIZE_HUGE);
                myStructure = new BRepMesh_DataStructureOfDelaun(myAllocator);
                myNodesMap = new VectorOfPnt(256);
                myUsedNodes = new DMapOfIntegerInteger(1, myAllocator);

                if (initDataStructure())
                {
                    if (!theRange.More())
                    {
                        return;
                    }
                    generateMesh(theRange);
                    commitSurfaceTriangulation();
                }
            }
            catch (Standard_Failure ex /*theException*/)
            {
            }

            myDFace = null; // Do not hold link to face.
            myStructure = null;
            myNodesMap = null;
            myUsedNodes = null;
            //myAllocator.Nullify();
        }

        TopAbs_Orientation fixSeamEdgeOrientation(
  IMeshData_Edge theDEdge,
  IMeshData_PCurve thePCurve)
        {
            for (int aPCurveIt = 0; aPCurveIt < theDEdge.PCurvesNb(); ++aPCurveIt)
            {
                var aPCurve = theDEdge.GetPCurve(aPCurveIt);
                if (aPCurve.GetFace() == myDFace && thePCurve != aPCurve)
                {
                    // Simple check that another pcurve of seam edge does not coincide with reference one.
                    gp_Pnt2d aPnt1_1 = thePCurve.GetPoint(0);
                    gp_Pnt2d aPnt2_1 = thePCurve.GetPoint(thePCurve.ParametersNb() - 1);

                    gp_Pnt2d aPnt1_2 = aPCurve.GetPoint(0);
                    gp_Pnt2d aPnt2_2 = aPCurve.GetPoint(aPCurve.ParametersNb() - 1);

                    double aSqDist1 = Math.Min(aPnt1_1.SquareDistance(aPnt1_2), aPnt1_1.SquareDistance(aPnt2_2));
                    double aSqDist2 = Math.Min(aPnt2_1.SquareDistance(aPnt1_2), aPnt2_1.SquareDistance(aPnt2_2));
                    if (aSqDist1 < Precision.SquareConfusion() &&
                        aSqDist2 < Precision.SquareConfusion())
                    {
                        return TopAbs_Orientation.TopAbs_INTERNAL;
                    }
                }
            }

            return thePCurve.GetOrientation();
        }

        protected virtual bool initDataStructure()
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
                    var aCurve = aDEdge.GetCurve();
                    IMeshData_PCurve aPCurve = aDEdge.GetPCurve(
                      myDFace, aDWire.GetEdgeOrientation(aEdgeIt));

                    TopAbs_Orientation aOri = fixSeamEdgeOrientation(aDEdge, aPCurve);

                    int aPrevNodeIndex = -1;
                    int aLastPoint = aPCurve.ParametersNb() - 1;
                    for (int aPointIt = 0; aPointIt <= aLastPoint; ++aPointIt)
                    {
                        int aNodeIndex = registerNode(
                          aCurve.GetPoint(aPointIt),
                          aPCurve.GetPoint(aPointIt),
                          BRepMesh_DegreeOfFreedom.BRepMesh_Frontier, false/*aPointIt > 0 && aPointIt < aLastPoint*/);

                        aPCurve.SetIndex(aPointIt, aNodeIndex);
                        myUsedNodes.Bind(aNodeIndex, aNodeIndex);

                        if (aPrevNodeIndex != -1 && aPrevNodeIndex != aNodeIndex)
                        {
                            int aLinksNb = myStructure.NbLinks();
                            int aLinkIndex = addLinkToMesh(aPrevNodeIndex, aNodeIndex, aOri);
                            if (aWireIt != 0 && aLinkIndex <= aLinksNb)
                            {
                                // Prevent holes around wire of zero area.
                                BRepMesh_Edge aLink = (BRepMesh_Edge)myStructure.GetLink(aLinkIndex);
                                aLink.SetMovability(BRepMesh_DegreeOfFreedom.BRepMesh_Fixed);
                            }
                        }

                        aPrevNodeIndex = aNodeIndex;
                    }
                }

            }



            return true;
        }

        IMeshData_Face myDFace;
        IMeshTools_Parameters myParameters;
        // Handle(NCollection_IncAllocator)       myAllocator;

        //Handle(VectorOfPnt)                    myNodesMap;
        DMapOfIntegerInteger myUsedNodes;

        BRepMesh_DataStructureOfDelaun myStructure;
    }
    public abstract class IDelaBella
    {

        // return 0: no output 
        // negative: all points are colinear, output hull vertices form colinear segment list, no triangles on output
        // positive: output hull vertices form counter-clockwise ordered segment contour, delaunay and hull triangles are available
        // if 'y' pointer is null, y coords are treated to be located immediately after every x
        // if advance_bytes is less than 2*sizeof coordinate type, it is treated as 2*sizeof coordinate type  

        public abstract DelaBella_Triangle GetFirstDelaunayTriangle(); // valid only if Triangulate() > 0
        public abstract int Triangulate(int points, double[] xy, int advance_bytes = 0);
        public static IDelaBella Create()
        {
            CDelaBella db = new CDelaBella();
            //if (!db)
            //   return 0;

            /*db->vert_alloc = 0;
            db->face_alloc = 0;
            db->max_verts = 0;
            db->max_faces = 0;

            db->first_dela_face = 0;
            db->first_hull_face = 0;
            db->first_hull_vert = 0;

            db->inp_verts = 0;
            db->out_verts = 0;

            db->errlog_proc = 0;
            db->errlog_file = 0;*/

            return db;
        }

    }

    public class CDelaBella : IDelaBella
    {


        public override DelaBella_Triangle GetFirstDelaunayTriangle()
        {
            //var ret1 = d.GetFirstTriangle();
            throw new NotImplementedException();
            DelaBella_Triangle ret = null;
            DelaBella_Triangle root = null;
            /* while (ret1 != null)
             {
                 var old = ret;

                 ret = new DelaBella_Triangle();
                 if (root == null)
                 {
                     root = ret;
                 }
                 if (old != null)
                 {
                     old.next = ret;
                 }
                 /*for (int i = 0; i < ret1.v.Length; i++)
                 {
                     TVert item = ret1.v[i];
                     ret.v[i] = new DelaBella_Vertex();
                     ret.v[i].x = item.x;
                     ret.v[i].y = item.y;
                     ret.v[i].i = item.index;

                 }
                 ret1 = ret1.Next;*/
            //  }
            //  return root;

        }
        //DelabellaWrapper d;
        // return 0: no output 
        // negative: all points are colinear, output hull vertices form colinear segment list, no triangles on output
        // positive: output hull vertices form counter-clockwise ordered segment contour, delaunay and hull triangles are available
        // if 'y' pointer is null, y coords are treated to be located immediately after every x
        // if advance_bytes is less than 2*sizeof coordinate type, it is treated as 2*sizeof coordinate type  
        public override int Triangulate(int points, double[] xy, int advance_bytes = 0)
        {
            // d = new DelabellaWrapper();

            if (xy == null)
                return 0;

            double[] x = new double[points];
            double[] y = new double[points];
            for (int i = 0; i < points; i++)
            {
                x[i] = xy[i * 2];
                y[i] = xy[i * 2 + 1];
            }
            //    var res = d.Triangulate(points, x, y);
            throw new NotImplementedException();
            // return res;
            //   if (y == null)
            //     y = x + 1;
        }
    }
    public class DelaBella_Triangle
    {
        public DelaBella_Vertex[] v = new DelaBella_Vertex[3]; // 3 vertices spanning this triangle
        public DelaBella_Triangle[] f = new DelaBella_Triangle[3]; // 3 adjacent faces, f[i] is at the edge opposite to vertex v[i]
        public DelaBella_Triangle next; // next triangle (of delaunay set or hull set)
    }

    public class DelaBella_Vertex
    {
        public int i; // index of original point
        public double x; // coordinates (input copy)
        public double y; // coordinates (input copy)
        public DelaBella_Vertex next; // next silhouette vertex



    }

    

    public class VectorOfPnt : NCollection_Vector<gp_Pnt>
    {
        public VectorOfPnt(int capacity) : base()
        {
        }

        internal int Size()
        {
            return Count;
        }

    }

    //! Default tool to define range of discrete face model and 
    //! obtain grid points distributed within this range.
    public class BRepMesh_DefaultRangeSplitter : AbstractRangeSplitter
    {
        public BRepMesh_DefaultRangeSplitter()
        {
            myIsValid = true;
        }

        (double, double) myRangeU;
        (double, double) myRangeV;
        (double, double) myDelta;
        (double, double) myTolerance;
        bool myIsValid;
        public override (double, double) GetToleranceUV()
        {
            return myTolerance;
        }
        public override bool IsValid()
        {
            return myIsValid;
        }

        public override void AddPoint(gp_Pnt2d thePoint)
        {
            myRangeU.Item1 = Math.Min(thePoint.X(), myRangeU.Item1);
            myRangeU.Item2 = Math.Max(thePoint.X(), myRangeU.Item2);
            myRangeV.Item1 = Math.Min(thePoint.Y(), myRangeV.Item1);
            myRangeV.Item2 = Math.Max(thePoint.Y(), myRangeV.Item2);
        }

        void updateRange(
   double theGeomFirst,
   double theGeomLast,
   bool isPeriodic,
  ref double theDiscreteFirst,
 ref double theDiscreteLast)
        {
            if (theDiscreteFirst < theGeomFirst ||
                theDiscreteLast > theGeomLast)
            {
                if (isPeriodic)
                {
                    if ((theDiscreteLast - theDiscreteFirst) > (theGeomLast - theGeomFirst))
                    {
                        theDiscreteLast = theDiscreteFirst + (theGeomLast - theGeomFirst);
                    }
                }
                else
                {
                    if ((theDiscreteFirst < theGeomLast) && (theDiscreteLast > theGeomFirst))
                    {
                        //Protection against the faces whose pcurve is out of the surface's domain
                        //(see issue #23675 and test cases "bugs iges buc60820*")

                        if (theGeomFirst > theDiscreteFirst)
                        {
                            theDiscreteFirst = theGeomFirst;
                        }

                        if (theGeomLast < theDiscreteLast)
                        {
                            theDiscreteLast = theGeomLast;
                        }
                    }
                }
            }
        }

        //=======================================================================
        // Function: AdjustRange
        // Purpose : 
        //=======================================================================
        public override void AdjustRange()
        {
            BRepAdaptor_Surface aSurface = GetSurface();
            updateRange(aSurface.FirstUParameter(), aSurface.LastUParameter(),
                        aSurface.IsUPeriodic(), ref myRangeU.Item1, ref myRangeU.Item2);

            if (myRangeU.Item2 < myRangeU.Item1)
            {
                myIsValid = false;
                return;
            }

            updateRange(aSurface.FirstVParameter(), aSurface.LastVParameter(),
                        aSurface.IsVPeriodic(), ref myRangeV.Item1, ref myRangeV.Item2);

            if (myRangeV.Item2 < myRangeV.Item1)
            {
                myIsValid = false;
                return;
            }

            var aLengthU = computeLengthU();
            var aLengthV = computeLengthV();
            myIsValid = aLengthU > Precision.PConfusion() && aLengthV > Precision.PConfusion();

            if (myIsValid)
            {
                computeTolerance(aLengthU, aLengthV);
                computeDelta(aLengthU, aLengthV);
            }
        }
        double computeLengthV()
        {
            double longv = 0.0;
            gp_Pnt P11 = new gp_Pnt(),
                P12 = new gp_Pnt(),
                P21 = new gp_Pnt(),
                P22 = new gp_Pnt(),
                P31 = new gp_Pnt(),
                P32 = new gp_Pnt();

            double dv = 0.05 * (myRangeV.Item2 - myRangeV.Item1);
            double dfuave = 0.5 * (myRangeU.Item2 + myRangeU.Item1);
            double dfvcur;
            int i1;

            BRepAdaptor_Surface gFace = GetSurface();
            gFace.D0(myRangeU.Item1, myRangeV.Item1, ref P11);
            gFace.D0(dfuave, myRangeV.Item1, ref P21);
            gFace.D0(myRangeU.Item2, myRangeV.Item1, ref P31);
            for (i1 = 1, dfvcur = myRangeV.Item1 + dv; i1 <= 20; i1++, dfvcur += dv)
            {
                gFace.D0(myRangeU.Item1, dfvcur, ref P12);
                gFace.D0(dfuave, dfvcur, ref P22);
                gFace.D0(myRangeU.Item2, dfvcur, ref P32);
                longv += (P11.Distance(P12) + P21.Distance(P22) + P31.Distance(P32));
                P11 = P12;
                P21 = P22;
                P31 = P32;
            }

            return longv / 3.0;
        }

        void computeTolerance(
  double theLenU,
  double theLenV)
        {
            double aDiffU = myRangeU.Item2 - myRangeU.Item1;
            double aDiffV = myRangeV.Item2 - myRangeV.Item1;

            // Slightly increase exact resolution so to cover links with approximate 
            // length equal to resolution itself on sub-resolution differences.
            double aTolerance = BRep_Tool.Tolerance(myDFace.GetFace());
            Adaptor3d_Surface aSurface = GetSurface().Surface();
            double aResU = aSurface.UResolution(aTolerance) * 1.1;
            double aResV = aSurface.VResolution(aTolerance) * 1.1;

            double aDeflectionUV = 1e-05;
            myTolerance.Item1 = Math.Max(Math.Min(aDeflectionUV, aResU), 1e-7 * aDiffU);
            myTolerance.Item2 = Math.Max(Math.Min(aDeflectionUV, aResV), 1e-7 * aDiffV);
        }

        void computeDelta(
   double theLengthU,
   double theLengthV)
        {
            double aDiffU = myRangeU.Item2 - myRangeU.Item1;
            double aDiffV = myRangeV.Item2 - myRangeV.Item1;

            myDelta.Item1 = aDiffU / (theLengthU < myTolerance.Item1 ? 1.0 : theLengthU);
            myDelta.Item2 = aDiffV / (theLengthV < myTolerance.Item2 ? 1.0 : theLengthV);
        }

        double computeLengthU()
        {
            double longu = 0.0;
            gp_Pnt P11 = new gp_Pnt(), P12 = new gp_Pnt(), P21 = new gp_Pnt(), P22 = new gp_Pnt(), P31 = new gp_Pnt(), P32 = new gp_Pnt();

            double du = 0.05 * (myRangeU.Item2 - myRangeU.Item1);
            double dfvave = 0.5 * (myRangeV.Item2 + myRangeV.Item1);
            double dfucur;
            int i1;

            BRepAdaptor_Surface gFace = GetSurface();
            gFace.D0(myRangeU.Item1, myRangeV.Item1, ref P11);
            gFace.D0(myRangeU.Item1, dfvave, ref P21);
            gFace.D0(myRangeU.Item1, myRangeV.Item2, ref P31);
            for (i1 = 1, dfucur = myRangeU.Item1 + du; i1 <= 20; i1++, dfucur += du)
            {
                gFace.D0(dfucur, myRangeV.Item1, ref P12);
                gFace.D0(dfucur, dfvave, ref P22);
                gFace.D0(dfucur, myRangeV.Item2, ref P32);
                longu += (P11.Distance(P12) + P21.Distance(P22) + P31.Distance(P32));
                P11 = P12;
                P21 = P22;
                P31 = P32;
            }

            return longu / 3.0;
        }

        //! Returns point in 3d space corresponded to the given 
        //! point defined in parameteric space of surface.
        public override gp_Pnt Point(gp_Pnt2d thePoint2d)
        {
            return GetSurface().Value(thePoint2d.X(), thePoint2d.Y());
        }

        //! Returns surface.
        public BRepAdaptor_Surface GetSurface()
        {
            return myDFace.GetSurface();
        }
        //! Returns U range.
        public override (double, double) GetRangeU()
        {
            return myRangeU;
        }
        //! Resets this splitter. Must be called before first use.
        public override void Reset(IMeshData_Face theDFace,
                                     IMeshTools_Parameters theParameters)
        {
            myDFace = theDFace;
            myRangeU.Item1 = myRangeV.Item1 = 1e+100;
            myRangeU.Item2 = myRangeV.Item2 = -1e+100;
            myDelta.Item1 = myDelta.Item2 = 1.0;
            myTolerance.Item1 = myTolerance.Item2 = Precision.Confusion();
        }
        IMeshData_Face myDFace;

        public override ListOfPnt2d GenerateSurfaceNodes(
  IMeshTools_Parameters theParameters)
        {
            return null;
        }
        //! Returns V range.
        public override (double, double) GetRangeV()
        {
            return myRangeV;
        }

        //! Returns delta.
        public override (double, double) GetDelta()
        {
            return myDelta;
        }

        public override gp_Pnt2d Scale(gp_Pnt2d thePoint, bool isToFaceBasis)
        {
            return isToFaceBasis ?
    new gp_Pnt2d((thePoint.X() - myRangeU.Item1) / myDelta.Item1,
              (thePoint.Y() - myRangeV.Item1) / myDelta.Item2) :
    new gp_Pnt2d(thePoint.X() * myDelta.Item1 + myRangeU.Item1,
              thePoint.Y() * myDelta.Item2 + myRangeV.Item1);
        }
    }


}



