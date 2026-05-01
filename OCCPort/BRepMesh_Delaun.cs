using OCCPort;
using OCCPort.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCCPort
{
    //! Compute the Delaunay's triangulation with the algorithm of Watson.
    public class BRepMesh_Delaun
    {
        //! Creates instance of triangulator, but do not run the algorithm automatically.
        public BRepMesh_Delaun(BRepMesh_DataStructureOfDelaun theOldMesh,
                                   int theCellsCountU,
                                   int theCellsCountV,
                                   bool isFillCircles)
        {
            myMeshData = (theOldMesh);
            //myCircles(new NCollection_IncAllocator(
            //         IMeshData::MEMORY_BLOCK_SIZE_HUGE)),
            mySupVert = new VectorOfInteger(3);
            myInitCircles = (false);

            if (isFillCircles)
            {
                //InitCirclesTool(theCellsCountU, theCellsCountV);
            }
        }


        //! Destruction of auxiliary triangles containing the given vertices.
        //! Removes auxiliary vertices also.
        //! @param theAuxVertices auxiliary vertices to be cleaned up.
        public void RemoveAuxElements()
        {
            MapOfIntegerInteger aLoopEdges = new MapOfIntegerInteger(10);

            // Destruction of triangles containing a top of the super triangle
            BRepMesh_SelectorOfDataStructureOfDelaun aSelector = new BRepMesh_SelectorOfDataStructureOfDelaun(myMeshData);
            for (int aSupVertId = 0; aSupVertId < mySupVert.Size(); ++aSupVertId)
                aSelector.NeighboursOfNode(mySupVert[aSupVertId]);

            //            IteratorOfMapOfInteger aFreeTriangles=new IteratorOfMapOfInteger ( aSelector.Elements());
            foreach (var aFreeTriangles in aSelector.Elements())
            {
                deleteTriangle(aFreeTriangles, aLoopEdges);
            }

            // All edges that remain free are removed from aLoopEdges;
            // only the boundary edges of the triangulation remain there
            //MapOfIntegerInteger::Iterator aFreeEdges(aLoopEdges );
            //for (; aFreeEdges.More(); aFreeEdges.Next())
            foreach (var aFreeEdges in aLoopEdges)
            {
                if (myMeshData.ElementsConnectedTo(aFreeEdges.Key).IsEmpty())
                    myMeshData.RemoveLink(aFreeEdges.Key);
            }

            // The tops of the super triangle are destroyed
            for (int aSupVertId = 0; aSupVertId < mySupVert.Size(); ++aSupVertId)
                myMeshData.RemoveNode(mySupVert[aSupVertId]);
        }

        //! Forces insertion of constraint edges into the base triangulation. 
        public void ProcessConstraints()
        {
            insertInternalEdges();

            // Adjustment of meshes to boundary edges
            frontierAdjust();
        }
        //! Gives the list of frontier edges.
        MapOfInteger Frontier()
        {
            return getEdgesByType(BRepMesh_DegreeOfFreedom.BRepMesh_Frontier);
        }

        void frontierAdjust()
        {
            MapOfInteger aFrontier = Frontier();



            VectorOfInteger aFailedFrontiers = new VectorOfInteger(256);
            MapOfIntegerInteger aLoopEdges = new MapOfIntegerInteger(10);
            MapOfInteger aIntFrontierEdges = new MapOfInteger();

            for (int aPass = 1; aPass <= 2; ++aPass)
            {
                // 1 pass): find external triangles on boundary edges;
                // 2 pass): find external triangles on boundary edges appeared 
                //          during triangles replacement.

                IteratorOfMapOfInteger aFrontierIt = new IteratorOfMapOfInteger(aFrontier);
                for (; aFrontierIt.More(); aFrontierIt.Next())
                {
                    int aFrontierId = aFrontierIt.Key();
                    BRepMesh_PairOfIndex aPair = myMeshData.ElementsConnectedTo(aFrontierId);
                    int aNbElem = aPair.Extent();
                    for (int aElemIt = 1; aElemIt <= aNbElem; ++aElemIt)
                    {
                        int aPriorElemId = aPair.Index(aElemIt);
                        if (aPriorElemId < 0)
                            continue;

                        BRepMesh_Triangle aElement = GetTriangle(aPriorElemId);
                        var e = aElement.myEdges;
                        var o = aElement.myOrientations;

                        bool isTriangleFound = false;
                        for (int n = 0; n < 3; ++n)
                        {
                            if (aFrontierId == e[n] && !o[n])
                            {
                                // Destruction  of external triangles on boundary edges
                                isTriangleFound = true;
                                deleteTriangle(aPriorElemId, aLoopEdges);
                                break;
                            }
                        }

                        if (isTriangleFound)
                            break;
                    }
                }

                // destrucrion of remaining hanging edges :
                //MapOfIntegerInteger::Iterator aLoopEdgesIt(aLoopEdges );
                //for (; aLoopEdgesIt.More(); aLoopEdgesIt.Next())
                foreach (var aLoopEdgesIt in aLoopEdges)
                {
                    var aLoopEdgeId = aLoopEdgesIt.Key;
                    if (myMeshData.ElementsConnectedTo(aLoopEdgeId).IsEmpty())
                        myMeshData.RemoveLink(aLoopEdgeId);
                }


                // destruction of triangles crossing the boundary edges and 
                // their replacement by makeshift triangles
                aFrontierIt = new IteratorOfMapOfInteger(aFrontier);
                for (/*aFrontierIt.Reset()*/; aFrontierIt.More(); aFrontierIt.Next())
                {
                    int aFrontierId = aFrontierIt.Key();
                    if (!myMeshData.ElementsConnectedTo(aFrontierId).IsEmpty())
                        continue;

                    bool isSuccess =
                      meshLeftPolygonOf(aFrontierId, true, aIntFrontierEdges);

                    if (aPass == 2 && !isSuccess)
                        aFailedFrontiers.Append(aFrontierId);
                }
            }

            cleanupMesh();

            // When the mesh has been cleaned up, try to process frontier edges 
            // once again to fill the possible gaps that might be occurred in case of "saw" -
            // situation when frontier edge has a triangle at a right side, but its free 
            // links cross another frontieres  and meshLeftPolygonOf itself can't collect 
            // a closed polygon.
            //VectorOfInteger::Iterator aFailedFrontiersIt(aFailedFrontiers );
            //for (; aFailedFrontiersIt.More(); aFailedFrontiersIt.Next())
            foreach (var aFailedFrontiersIt in aFailedFrontiers)
            {
                int aFrontierId = aFailedFrontiersIt;
                if (!myMeshData.ElementsConnectedTo(aFrontierId).IsEmpty())
                    continue;

                meshLeftPolygonOf(aFrontierId, true, aIntFrontierEdges);
            }
        }

        //! Gives the list of free edges used only one time
        MapOfInteger FreeEdges()
        {
            return getEdgesByType(BRepMesh_DegreeOfFreedom.BRepMesh_Free);
        }
        //=======================================================================
        //function : getEdgesByType
        //purpose  : Gives the list of edges with type defined by input parameter
        //=======================================================================
        MapOfInteger getEdgesByType(
  BRepMesh_DegreeOfFreedom theEdgeType)
        {

            MapOfInteger aResult = new MapOfInteger();
            foreach (var anEdgeIt in myMeshData.LinksOfDomain())
            {
                int anEdge = anEdgeIt;
                bool isToAdd = (theEdgeType == BRepMesh_DegreeOfFreedom.BRepMesh_Free) ?
                  (myMeshData.ElementsConnectedTo(anEdge).Extent() <= 1) :
                  (GetEdge(anEdge).Movability() == theEdgeType);

                if (isToAdd)
                    aResult.Add(anEdge);
            }

            return aResult;
        }
        void cleanupMesh()
        {
            for (; ; )
            {
                //aAllocator->Reset(Standard_False);
                MapOfIntegerInteger aLoopEdges = new MapOfIntegerInteger(10);
                MapOfInteger aDelTriangles = new MapOfInteger();

                MapOfInteger aFreeEdges = FreeEdges();
                IteratorOfMapOfInteger aFreeEdgesIt = new IteratorOfMapOfInteger(aFreeEdges);
                for (; aFreeEdgesIt.More(); aFreeEdgesIt.Next())
                {
                    int aFreeEdgeId = aFreeEdgesIt.Key();
                    BRepMesh_Edge anEdge = GetEdge(aFreeEdgeId);
                    if (anEdge.Movability() == Enums.BRepMesh_DegreeOfFreedom.BRepMesh_Frontier)
                        continue;

                    BRepMesh_PairOfIndex aPair =
                      myMeshData.ElementsConnectedTo(aFreeEdgeId);
                    if (aPair.IsEmpty())
                    {
                        aLoopEdges.Bind(aFreeEdgeId, 1);
                        continue;
                    }

                    int aTriId = aPair.FirstIndex();

                    // Check that the connected triangle is not surrounded by another triangles
                    BRepMesh_Triangle aElement = GetTriangle(aTriId);
                    var anEdges = aElement.myEdges;

                    bool isCanNotBeRemoved = true;
                    for (int aCurEdgeIdx = 0; aCurEdgeIdx < 3; ++aCurEdgeIdx)
                    {
                        if (anEdges[aCurEdgeIdx] != aFreeEdgeId)
                            continue;

                        for (int anOtherEdgeIt = 1; anOtherEdgeIt <= 2 && isCanNotBeRemoved; ++anOtherEdgeIt)
                        {
                            int anOtherEdgeId = (aCurEdgeIdx + anOtherEdgeIt) % 3;
                            BRepMesh_PairOfIndex anOtherEdgePair =
                              myMeshData.ElementsConnectedTo(anEdges[anOtherEdgeId]);

                            if (anOtherEdgePair.Extent() < 2)
                            {
                                isCanNotBeRemoved = false;
                            }
                            else
                            {
                                for (int aTriIdx = 1; aTriIdx <= anOtherEdgePair.Extent() && isCanNotBeRemoved; ++aTriIdx)
                                {
                                    if (anOtherEdgePair.Index(aTriIdx) == aTriId)
                                        continue;

                                    int[] v = new int[3];
                                    BRepMesh_Triangle aCurTriangle = GetTriangle(anOtherEdgePair.Index(aTriIdx));
                                    myMeshData.ElementNodes(aCurTriangle, v);
                                    for (int aNodeIdx = 0; aNodeIdx < 3 && isCanNotBeRemoved; ++aNodeIdx)
                                    {
                                        if (isSupVertex(v[aNodeIdx]))
                                        {
                                            isCanNotBeRemoved = false;
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }

                    if (isCanNotBeRemoved)
                        continue;

                    bool[] isConnected = { false, false };
                    for (int aLinkNodeIt = 0; aLinkNodeIt < 2; ++aLinkNodeIt)
                    {
                        isConnected[aLinkNodeIt] = isBoundToFrontier((aLinkNodeIt == 0) ?
                          anEdge.FirstNode() : anEdge.LastNode(), aFreeEdgeId);
                    }

                    if (!isConnected[0] || !isConnected[1])
                        aDelTriangles.Add(aTriId);
                }

                // Destruction of triangles :
                int aDeletedTrianglesNb = 0;
                IteratorOfMapOfInteger aDelTrianglesIt = new IteratorOfMapOfInteger(aDelTriangles);
                for (; aDelTrianglesIt.More(); aDelTrianglesIt.Next())
                {
                    deleteTriangle(aDelTrianglesIt.Key(), aLoopEdges);
                    aDeletedTrianglesNb++;
                }

                // Destruction of remaining hanging edges
                //MapOfIntegerInteger::Iterator aLoopEdgesIt(aLoopEdges );
                //for (; aLoopEdgesIt.More(); aLoopEdgesIt.Next())
                foreach (var aLoopEdgesIt in aLoopEdges)
                {
                    if (myMeshData.ElementsConnectedTo(aLoopEdgesIt.Key).IsEmpty())
                        myMeshData.RemoveLink(aLoopEdgesIt.Key);
                }

                if (aDeletedTrianglesNb == 0)
                    break;
            }
        }

        private void deleteTriangle(int theIndex, MapOfIntegerInteger theLoopEdges)
        {
            if (myInitCircles)
            {
                //myCircles.Delete(theIndex);
            }

            BRepMesh_Triangle aElement = GetTriangle(theIndex);
            int[] e = aElement.myEdges;
            bool[] o = aElement.myOrientations;

            myMeshData.RemoveElement(theIndex);

            for (int i = 0; i < 3; ++i)
            {
                if (!theLoopEdges.Bind(e[i], o[i] ? 1 : 0))
                {
                    theLoopEdges.UnBind(e[i]);
                    myMeshData.RemoveLink(e[i]);
                }
            }
        }

        bool isBoundToFrontier(
  int theRefNodeId,
  int theRefLinkId)
        {
            Stack<int> aLinkStack = new Stack<int>();
            TColStd_PackedMapOfInteger aVisitedLinks = new TColStd_PackedMapOfInteger();

            aLinkStack.Push(theRefLinkId);
            while (aLinkStack.Count() > 0)
            {
                int aCurrentLinkId = aLinkStack.Peek();
                aLinkStack.Pop();

                BRepMesh_PairOfIndex aPair = myMeshData.ElementsConnectedTo(aCurrentLinkId);
                if (aPair.IsEmpty())
                    return false;

                int aNbElements = aPair.Extent();
                for (int anElemIt = 1; anElemIt <= aNbElements; ++anElemIt)
                {
                    int aTriId = aPair.Index(anElemIt);
                    if (aTriId < 0)
                        continue;

                    BRepMesh_Triangle aElement = GetTriangle(aTriId);
                    var anEdges = aElement.myEdges;

                    for (int anEdgeIt = 0; anEdgeIt < 3; ++anEdgeIt)
                    {
                        int anEdgeId = anEdges[anEdgeIt];
                        if (anEdgeId == aCurrentLinkId)
                            continue;

                        BRepMesh_Edge anEdge = GetEdge(anEdgeId);
                        if (anEdge.FirstNode() != theRefNodeId &&
                            anEdge.LastNode() != theRefNodeId)
                        {
                            continue;
                        }

                        if (anEdge.Movability() != Enums.BRepMesh_DegreeOfFreedom.BRepMesh_Free)
                            return true;

                        if (aVisitedLinks.Add(anEdgeId))
                        {
                            aLinkStack.Push(anEdgeId);
                        }
                    }
                }
            }

            return false;
        }
        //! Gives edge with the given index
        public BRepMesh_Edge GetEdge(int theIndex)
        {
            return myMeshData.GetLink(theIndex);
        }

        //! Gives triangle with the given index
        public BRepMesh_Triangle GetTriangle(int theIndex)
        {
            return myMeshData.GetElement(theIndex);
        }

        //! Checks whether the given vertex id relates to super contour.
        bool isSupVertex(int theVertexIdx)
        {
            foreach (var aIt in mySupVert)
            {
                if (theVertexIdx == aIt)
                {
                    return true;
                }
            }

            return false;
        }

        //! Gives the list of internal edges.
        MapOfInteger InternalEdges()
        {
            return getEdgesByType(BRepMesh_DegreeOfFreedom.BRepMesh_Fixed);
        }

        private void insertInternalEdges()
        {

            {
                MapOfInteger anInternalEdges = InternalEdges();

                // Destruction of triangles intersecting internal edges
                // and their replacement by makeshift triangles
                IteratorOfMapOfInteger anInernalEdgesIt = new IteratorOfMapOfInteger(anInternalEdges);
                for (; anInernalEdgesIt.More(); anInernalEdgesIt.Next())
                {
                    int aLinkIndex = anInernalEdgesIt.Key();
                    BRepMesh_PairOfIndex aPair = myMeshData.ElementsConnectedTo(aLinkIndex);

                    // Check both sides of link for adjusted triangle.
                    bool[] isGo = { true, true };
                    for (int aTriangleIt = 1; aTriangleIt <= aPair.Extent(); ++aTriangleIt)
                    {
                        BRepMesh_Triangle aElement = GetTriangle(aPair.Index(aTriangleIt));
                        var e = aElement.myEdges;
                        var o = aElement.myOrientations;

                        for (int i = 0; i < 3; ++i)
                        {
                            if (e[i] == aLinkIndex)
                            {
                                isGo[o[i] ? 0 : 1] = false;
                                break;
                            }
                        }
                    }

                    if (isGo[0])
                    {
                        meshLeftPolygonOf(aLinkIndex, true);
                    }

                    if (isGo[1])
                    {
                        meshLeftPolygonOf(aLinkIndex, false);
                    }
                }
            }
        }

        //=======================================================================
        //function : meshLeftPolygonOf
        //purpose  : Collect the polygon at the left of the given edge (material side)
        //=======================================================================
        bool meshLeftPolygonOf(
  int theStartEdgeId,
  bool isForward,
  MapOfInteger theSkipped = null)
        {
            throw new NotImplementedException();
        }

        VectorOfInteger mySupVert;
        bool myInitCircles;
        BRepMesh_DataStructureOfDelaun myMeshData;

        //! Explicitly sets ids of auxiliary vertices used to build mesh and used by 3rd-party algorithms.
        public void SetAuxVertices(VectorOfInteger theSupVert)
        {
            mySupVert = theSupVert;
        }
        //! Creates the triangulation with an existant Mesh data structure.
        public BRepMesh_Delaun(BRepMesh_DataStructureOfDelaun theOldMesh,
                                   VectorOfInteger theVertexIndices,
                                   int theCellsCountU,
                                   int theCellsCountV)
        {
            myMeshData = (theOldMesh);
            //myCircles(theVertexIndices.Length(), new NCollection_IncAllocator(
            //         IMeshData::MEMORY_BLOCK_SIZE_HUGE)),
            mySupVert = new VectorOfInteger(3);
            myInitCircles = false;

            perform(theVertexIndices, theCellsCountU, theCellsCountV);
        }
        //! Gives vertex with the given index
        BRepMesh_Vertex GetVertex(int theIndex)
        {
            return myMeshData.GetNode(theIndex);
        }
        double Precision = OCCPort.Precision.PConfusion();

        //=======================================================================
        //function : perform
        //purpose  : Create super mesh and run triangulation procedure
        //=======================================================================
        void perform(VectorOfInteger theVertexIndices,
                              int theCellsCountU /* = -1 */,
                              int theCellsCountV /* = -1 */)
        {
            if (theVertexIndices.Length() <= 2)
            {
                return;
            }

            Bnd_Box2d aBox = new Bnd_Box2d();
            int anIndex = theVertexIndices.Lower();
            int anUpper = theVertexIndices.Upper();
            for (; anIndex <= anUpper; ++anIndex)
            {
                aBox.Add(new gp_Pnt2d(GetVertex(theVertexIndices[anIndex]).Coord()));
            }

            aBox.Enlarge(Precision);

            //initCirclesTool(aBox, theCellsCountU, theCellsCountV);
            superMesh(aBox);

            //ComparatorOfIndexedVertexOfDelaun aCmp(myMeshData);
            //std::make_heap(theVertexIndices.begin(), theVertexIndices.end(), aCmp);
            //std::sort_heap(theVertexIndices.begin(), theVertexIndices.end(), aCmp);

            //compute(theVertexIndices);
        }
        BRepMesh_Triangle mySupTrian;

        void superMesh(Bnd_Box2d theBox)
        {
            double aMinX = 0, aMinY = 0, aMaxX = 0, aMaxY = 0;
            theBox.Get(ref aMinX, ref aMinY, ref aMaxX, ref aMaxY);
            double aDeltaX = aMaxX - aMinX;
            double aDeltaY = aMaxY - aMinY;

            double aDeltaMin = Math.Min(aDeltaX, aDeltaY);
            double aDeltaMax = Math.Max(aDeltaX, aDeltaY);
            double aDelta = aDeltaX + aDeltaY;

            mySupVert.Append(myMeshData.AddNode(
              new BRepMesh_Vertex((aMinX + aMaxX) / 2, aMaxY + aDeltaMax, Enums.BRepMesh_DegreeOfFreedom.BRepMesh_Free)));

            mySupVert.Append(myMeshData.AddNode(
              new BRepMesh_Vertex(aMinX - aDelta, aMinY - aDeltaMin, Enums.BRepMesh_DegreeOfFreedom.BRepMesh_Free)));

            mySupVert.Append(myMeshData.AddNode(
              new BRepMesh_Vertex(aMaxX + aDelta, aMinY - aDeltaMin, Enums.BRepMesh_DegreeOfFreedom.BRepMesh_Free)));

            int[] e = new int[3];
            bool[] o = new bool[3];
            for (int aNodeId = 0; aNodeId < 3; ++aNodeId)
            {
                int aFirstNode = aNodeId;
                int aLastNode = (aNodeId + 1) % 3;
                int aLinkIndex = myMeshData.AddLink(new BRepMesh_Edge(
                  mySupVert[aFirstNode], mySupVert[aLastNode], Enums.BRepMesh_DegreeOfFreedom.BRepMesh_Free));

                e[aNodeId] = Math.Abs(aLinkIndex);
                o[aNodeId] = (aLinkIndex > 0);
            }

            mySupTrian = new BRepMesh_Triangle(e, o, Enums.BRepMesh_DegreeOfFreedom.BRepMesh_Free);
        }
    }
}