using OCCPort;
using System;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace OCCPort
{
    //! Provides an algorithm to explore, inside a triangulation, the
    //! adjacency data for a node or a triangle.
    //! Adjacency data for a node consists of triangles which
    //! contain the node.
    //! Adjacency data for a triangle consists of:
    //! -   the 3 adjacent triangles which share an edge of the triangle,
    //! -   and the 3 nodes which are the other nodes of these adjacent triangles.
    //! Example
    //! Inside a triangulation, a triangle T
    //! has nodes n1, n2 and n3.
    //! It has adjacent triangles AT1, AT2 and AT3 where:
    //! - AT1 shares the nodes n2 and n3,
    //! - AT2 shares the nodes n3 and n1,
    //! - AT3 shares the nodes n1 and n2.
    //! It has adjacent nodes an1, an2 and an3 where:
    //! - an1 is the third node of AT1,
    //! - an2 is the third node of AT2,
    //! - an3 is the third node of AT3.
    //! So triangle AT1 is composed of nodes n2, n3 and an1.
    //! There are two ways of using this algorithm.
    //! -   From a given node you can look for one triangle that
    //! passes through the node, then look for the triangles
    //! adjacent to this triangle, then the adjacent nodes. You
    //! can thus explore the triangulation step by step (functions
    //! Triangle, Triangles and Nodes).
    //! -   From a given node you can look for all the triangles
    //! that pass through the node (iteration method, using the
    //! functions Initialize, More, Next and Value).
    //! A Connect object can be seen as a tool which analyzes a
    //! triangulation and translates it into a series of triangles. By
    //! doing this, it provides an interface with other tools and
    //! applications working on basic triangles, and which do not
    //! work directly with a Poly_Triangulation.
    public class Poly_Connect
    {
        //! Returns the triangulation analyzed by this tool.
        public Poly_Triangulation Triangulation() { return myTriangulation; }
        Poly_Triangulation myTriangulation;
        TColStd_Array1OfInteger myTriangles;
        TColStd_Array1OfInteger myAdjacents;
        int mytr;
        int myfirst;
        int mynode;
        int myothernode;
        bool mysense;
        bool mymore;
        TColStd_PackedMapOfInteger myPassedTr;
        public void Load(Poly_Triangulation theTriangulation)
        {
            myTriangulation = theTriangulation;
            mytr = 0;
            myfirst = 0;
            mynode = 0;
            myothernode = 0;
            mysense = false;
            mymore = false;

            int aNbNodes = myTriangulation.NbNodes();
            int aNbTris = myTriangulation.NbTriangles();
            {
                int aNbAdjs = 6 * aNbTris;
                if (myTriangles.Size() != aNbNodes)
                {
                    myTriangles.Resize(1, aNbNodes, false);
                }
                if (myAdjacents.Size() != aNbAdjs)
                {
                    myAdjacents.Resize(1, aNbAdjs, false);
                }
            }

            myTriangles.Init(0);
            myAdjacents.Init(0);

            // We first build an array of the list of edges connected to the nodes
            // create an array to store the edges starting from the vertices
            NCollection_Array1<polyedge> anEdges = new NCollection_Array1<polyedge>(1, aNbNodes);
            anEdges.Init(null);
            // use incremental allocator for small allocations
            //Handle(NCollection_IncAllocator) anIncAlloc = new NCollection_IncAllocator();

            // loop on the triangles
            NCollection_Vec3<int> aTriNodes = new NCollection_Vec3<int>();
            NCollection_Vec2<int> anEdgeNodes = new NCollection_Vec2<int>();
            for (int aTriIter = 1; aTriIter <= aNbTris; ++aTriIter)
            {
                // get the nodes

                //myTriangulation.Triangle(aTriIter).Get(ref aTriNodes[0], ref aTriNodes[1], ref aTriNodes[2]);                
                aTriNodes.Set(myTriangulation.Triangle(aTriIter).Get());

                // Update the myTriangles array
                myTriangles.SetValue(aTriNodes[0], aTriIter);
                myTriangles.SetValue(aTriNodes[1], aTriIter);
                myTriangles.SetValue(aTriNodes[2], aTriIter);

                // update the edge lists
                for (int aNodeInTri = 0; aNodeInTri < 3; ++aNodeInTri)
                {
                    int aNodeNext = (aNodeInTri + 1) % 3;  // the following node of the edge
                    if (aTriNodes[aNodeInTri] < aTriNodes[aNodeNext])
                    {
                        anEdgeNodes[0] = aTriNodes[aNodeInTri];
                        anEdgeNodes[1] = aTriNodes[aNodeNext];
                    }
                    else
                    {
                        anEdgeNodes[0] = aTriNodes[aNodeNext];
                        anEdgeNodes[1] = aTriNodes[aNodeInTri];
                    }

                    // edge from node 0 to node 1 with node 0 < node 1
                    // insert in the list of node 0
                    polyedge ced = anEdges[anEdgeNodes[0]];
                    for (; ced != null; ced = ced.next)
                    {
                        // the edge already exists
                        if (ced.nd == anEdgeNodes[1])
                        {
                            // just mark the adjacency if found
                            ced.nt[1] = aTriIter;
                            ced.nn[1] = aTriNodes[3 - aNodeInTri - aNodeNext];  // the third node
                            break;
                        }
                    }

                    if (ced == null)
                    {
                        // create the edge if not found
                        //ced = (polyedge*)anIncAlloc->Allocate(sizeof(polyedge));
                        ced = new polyedge();
                        ced.next = anEdges[anEdgeNodes[0]];
                        anEdges[anEdgeNodes[0]] = ced;
                        ced.nd = anEdgeNodes[1];
                        ced.nt[0] = aTriIter;
                        ced.nn[0] = aTriNodes[3 - aNodeInTri - aNodeNext];  // the third node
                        ced.nt[1] = 0;
                        ced.nn[1] = 0;
                    }
                }
            }

            // now complete the myAdjacents array
            int anAdjIndex = 1;
            for (int aTriIter = 1; aTriIter <= aNbTris; ++aTriIter)
            {
                // get the nodes

                aTriNodes.Set(myTriangulation.Triangle(aTriIter).Get());

                // for each edge in triangle
                for (int aNodeInTri = 0; aNodeInTri < 3; ++aNodeInTri)
                {
                    int aNodeNext = (aNodeInTri + 1) % 3;  // the following node of the edge
                    if (aTriNodes[aNodeInTri] < aTriNodes[aNodeNext])
                    {
                        anEdgeNodes[0] = aTriNodes[aNodeInTri];
                        anEdgeNodes[1] = aTriNodes[aNodeNext];
                    }
                    else
                    {
                        anEdgeNodes[0] = aTriNodes[aNodeNext];
                        anEdgeNodes[1] = aTriNodes[aNodeInTri];
                    }

                    // edge from node 0 to node 1 with node 0 < node 1
                    // find in the list of node 0
                    polyedge ced = anEdges[anEdgeNodes[0]];
                    while (ced.nd != anEdgeNodes[1])
                    {
                        ced = ced.next;
                    }

                    // Find the adjacent triangle
                    int l = ced.nt[0] == aTriIter ? 1 : 0;

                    myAdjacents.SetValue(anAdjIndex, ced.nt[l]);
                    myAdjacents.SetValue(anAdjIndex + 3, ced.nn[l]);
                    ++anAdjIndex;
                }
                anAdjIndex += 3;
            }

            // destroy the edges array - can be skipped when using NCollection_IncAllocator
            /*for (Standard_Integer aNodeIter = anEdges.Lower(); aNodeIter <= anEdges.Upper(); ++aNodeIter)
            {
              for (polyedge* anEdgeIter = anEdges[aNodeIter]; anEdgeIter != NULL;)
              {
                polyedge* aTmp = anEdgeIter->next;
                anIncAlloc->Free (anEdgeIter);
                anEdgeIter = aTmp;
              }
            }*/
        }

        //! Initializes an iterator to search for all the triangles
        //! containing the node referenced at index N in the nodes
        //! table, for the triangulation analyzed by this tool.
        //! The iterator is managed by the following functions:
        //! -   More, which checks if there are still elements in the iterator
        //! -   Next, which positions the iterator on the next element
        //! -   Value, which returns the current element.
        //! The use of such an iterator provides direct access to the
        //! triangles around a particular node, i.e. it avoids iterating on
        //! all the component triangles of a triangulation.
        //! Example
        //! Poly_Connect C(Tr);
        //! for
        //! (C.Initialize(n1);C.More();C.Next())
        //! {
        //! t = C.Value();
        //! }
        internal void Initialize(int N)
        {
            mynode = N;
            myfirst = Triangle(N);
            mytr = myfirst;
            mysense = true;
            mymore = (myfirst != 0);
            myPassedTr.Clear();
            myPassedTr.Add(mytr);
            if (mymore)
            {
                int i;
                int[] no = new int[3];
                myTriangulation.Triangle(myfirst).Get(ref no[0], ref no[1], ref no[2]);
                for (i = 0; i < 3; i++)
                    if (no[i] == mynode)
                        break;

                myothernode = no[(i + 2) % 3];
            }
        }
        //! Returns the index of a triangle containing the node at
        //! index N in the nodes table specific to the triangulation analyzed by this tool
        int Triangle(int N) { return myTriangles[N]; }

        //! Returns true if there is another element in the iterator
        //! defined with the function Initialize (i.e. if there is another
        //! triangle containing the given node).
        internal bool More()
        {
            return mymore;
        }

        //! Returns in t1, t2 and t3, the indices of the 3 triangles
        //! adjacent to the triangle at index T in the triangles table
        //! specific to the triangulation analyzed by this tool.
        //! Warning
        //! Null indices are returned when there are fewer than 3
        //! adjacent triangles.
        public void Triangles(int T, ref int t1, ref int t2, ref int t3)
        {
            int index = 6 * (T - 1);
            t1 = myAdjacents[index + 1];
            t2 = myAdjacents[index + 2];
            t3 = myAdjacents[index + 3];
        }


        public void Next()
        {
            int i, j;
            int[] n = new int[3];
            int[] t = new int[3];
            Triangles(mytr, ref t[0], ref t[1], ref t[2]);
            if (mysense)
            {
                for (i = 0; i < 3; i++)
                {
                    if (t[i] != 0)
                    {
                        myTriangulation.Triangle(t[i]).Get(ref n[0], ref n[1], ref n[2]);
                        for (j = 0; j < 3; j++)
                        {
                            if ((n[j] == mynode) && (n[(j + 1) % 3] == myothernode))
                            {
                                mytr = t[i];
                                myothernode = n[(j + 2) % 3];
                                mymore = !myPassedTr.Contains(mytr);
                                myPassedTr.Add(mytr);
                                return;
                            }
                        }
                    }
                }
                // sinon, depart vers la gauche.
                myTriangulation.Triangle(myfirst).Get(ref n[0], ref n[1], ref n[2]);
                for (i = 0; i < 3; i++)
                    if (n[i] == mynode)
                        break;

                myothernode = n[(i + 1) % 3];
                mysense = false;
                mytr = myfirst;
                Triangles(mytr, ref t[0], ref t[1], ref t[2]);
            }
            if (!mysense)
            {
                for (i = 0; i < 3; i++)
                {
                    if (t[i] != 0)
                    {
                        myTriangulation.Triangle(t[i]).Get(ref n[0], ref n[1], ref n[2]);
                        for (j = 0; j < 3; j++)
                        {
                            if ((n[j] == mynode) && (n[(j + 2) % 3] == myothernode))
                            {
                                mytr = t[i];
                                myothernode = n[(j + 1) % 3];
                                mymore = !myPassedTr.Contains(mytr);
                                myPassedTr.Add(mytr);
                                return;
                            }
                        }
                    }
                }
            }
            mymore = false;
        }

        //! Returns the index of the current triangle to which the
        //! iterator, defined with the function Initialize, points. This is
        //! an index in the triangles table specific to the triangulation
        //! analyzed by this tool
        public int Value() { return mytr; }

    }
}