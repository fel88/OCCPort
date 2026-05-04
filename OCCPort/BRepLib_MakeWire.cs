using OCCPort.Enums;
using System;

namespace OCCPort
{
    //! Provides methods to build wires.
    //!
    //! A wire may be built :
    //!
    //! * From a single edge.
    //!
    //! * From a wire and an edge.
    //!
    //! - A new wire  is created with the edges  of  the
    //! wire + the edge.
    //!
    //! - If the edge is not connected  to the wire the
    //! flag NotDone   is set and  the  method Wire will
    //! raise an error.
    //!
    //! - The connection may be :
    //!
    //! . Through an existing vertex. The edge is shared.
    //!
    //! . Through a geometric coincidence of vertices.
    //! The edge is  copied  and the vertices from the
    //! edge are  replaced  by  the vertices from  the
    //! wire.
    //!
    //! . The new edge and the connection vertices are
    //! kept by the algorithm.
    //!
    //! * From 2, 3, 4 edges.
    //!
    //! - A wire is  created from  the first edge, the
    //! following edges are added.
    //!
    //! * From many edges.
    //!
    //! - The following syntax may be used :
    //!
    //! BRepLib_MakeWire MW;
    //!
    //! // for all the edges ...
    //! MW.Add(anEdge);
    //!
    //! TopoDS_Wire W = MW;

    public class BRepLib_MakeWire : BRepLib_MakeShape
    {
        public BRepLib_MakeWire()
        {
            myShape = new TopoDS_Wire();//not origin code!
            myError = BRepLib_WireError.BRepLib_EmptyWire;
        }

        internal TopoDS_Wire Wire()
        {
            return TopoDS.Wire(Shape());

        }
        public void Add(TopoDS_Edge E)
        {
            Add(E, true);
        }   //=======================================================================
        //function : Add
        //purpose  : 
        // PMN  19/03/1998  For the Problem of performance TopExp.Vertices are not used on wire
        // PMN  10/09/1998  In case if the wire is previously closed (or degenerated) 
        //                  TopExp.Vertices is used to reduce the ambiguity.
        // IsCheckGeometryProximity flag : If true => check for the geometry proximity of vertices
        //=======================================================================
        public void Add(TopoDS_Edge E, bool IsCheckGeometryProximity)
        {

            bool forward = false;
            // to tell if it has been decided to add forward
            bool reverse = false;
            // to tell if it has been decided to add reversed
            bool init = false;
            // To know if it is necessary to calculate VL, VF
            BRep_Builder B = new BRep_Builder();
            TopoDS_Iterator it = new TopoDS_Iterator();

            if (myEdge.IsNull())
            {
                init = true;
                // first edge, create the wire
                B.MakeWire(TopoDS.Wire(myShape));

                // set the edge
                myEdge = E;

                // add the vertices
                for (it.Initialize(myEdge); it.More(); it.Next())
                    myVertices.Add(it.Value());
            }

            else
            {
                init = myShape.Closed(); // If it is closed no control
                TopoDS_Shape aLocalShape = E.Oriented(TopAbs_Orientation.TopAbs_FORWARD);
                TopoDS_Edge EE = TopoDS.Edge(aLocalShape);
                //    TopoDS_Edge EE = TopoDS.Edge(E.Oriented(TopAbs_FORWARD));

                // test the vertices of the edge

                bool connected = false;
                bool copyedge = false;

                if (myError != BRepLib_WireError.BRepLib_NonManifoldWire)
                {
                    if (VF.IsNull() || VL.IsNull())
                        myError = BRepLib_WireError.BRepLib_NonManifoldWire;
                }

                for (it.Initialize(EE); it.More(); it.Next())
                {

                    TopoDS_Vertex VE = TopoDS.Vertex(it.Value());

                    // if the vertex is in the wire, ok for the connection
                    if (myVertices.Contains(VE))
                    {
                        connected = true;
                        myVertex = VE;
                        if (myError != BRepLib_WireError.BRepLib_NonManifoldWire)
                        {
                            // is it always so ?
                            if (VF.IsSame(VL))
                            {
                                // Orientation indetermined (in 3d) : Preserve the initial
                                if (!VF.IsSame(VE)) myError = BRepLib_WireError.BRepLib_NonManifoldWire;
                            }
                            else
                            {
                                if (VF.IsSame(VE))
                                {
                                    if (VE.Orientation() == TopAbs_Orientation.TopAbs_FORWARD)
                                        reverse = true;
                                    else
                                        forward = true;
                                }
                                else if (VL.IsSame(VE))
                                {
                                    if (VE.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
                                        reverse = true;
                                    else
                                        forward = true;
                                }
                                else
                                    myError = BRepLib_WireError.BRepLib_NonManifoldWire;
                            }
                        }
                    }
                    else if (IsCheckGeometryProximity)
                    {
                        // search if there is a similar vertex in the edge	
                        gp_Pnt PE = BRep_Tool.Pnt(VE);

                        for (int i = 1; i <= myVertices.Extent(); i++)
                        {
                            TopoDS_Vertex VW = TopoDS.Vertex(myVertices.FindKey(i));
                            gp_Pnt PW = BRep_Tool.Pnt(VW);
                            double l = PE.Distance(PW);

                            if ((l < BRep_Tool.Tolerance(VE)) ||
                                (l < BRep_Tool.Tolerance(VW)))
                            {
                                copyedge = true;
                                if (myError != BRepLib_WireError.BRepLib_NonManifoldWire)
                                {
                                    // is it always so ?
                                    if (VF.IsSame(VL))
                                    {
                                        // Orientation indetermined (in 3d) : Preserve the initial
                                        if (!VF.IsSame(VW)) myError = BRepLib_WireError.BRepLib_NonManifoldWire;
                                    }
                                    else
                                    {
                                        if (VF.IsSame(VW))
                                        {
                                            if (VE.Orientation() == TopAbs_Orientation.TopAbs_FORWARD)
                                                reverse = true;
                                            else
                                                forward = true;
                                        }
                                        else if (VL.IsSame(VW))
                                        {
                                            if (VE.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
                                                reverse = true;
                                            else
                                                forward = true;
                                        }
                                        else
                                            myError = BRepLib_WireError.BRepLib_NonManifoldWire;
                                    }
                                }
                                break;
                            }
                        }
                        if (copyedge)
                        {
                            connected = true;
                        }
                    }
                }

                if (!connected)
                {
                    myError = BRepLib_WireError.BRepLib_DisconnectedWire;
                    NotDone();
                    return;
                }
                else
                {
                    if (!copyedge)
                    {
                        myEdge = EE;
                        for (it.Initialize(EE); it.More(); it.Next())
                            myVertices.Add(it.Value());
                    }
                    else
                    {
                        // copy the edge
                        TopoDS_Shape Dummy = EE.EmptyCopied();
                        myEdge = TopoDS.Edge(Dummy);

                        for (it.Initialize(EE); it.More(); it.Next())
                        {

                            TopoDS_Vertex VE = TopoDS.Vertex(it.Value());
                            gp_Pnt PE = BRep_Tool.Pnt(VE);

                            bool newvertex = false;
                            for (int i = 1; i <= myVertices.Extent(); i++)
                            {
                                TopoDS_Vertex VW = TopoDS.Vertex(myVertices.FindKey(i));
                                gp_Pnt PW = BRep_Tool.Pnt(VW);
                                double l = PE.Distance(PW), tolE, tolW;
                                tolW = BRep_Tool.Tolerance(VW);
                                tolE = BRep_Tool.Tolerance(VE);

                                if ((l < tolE) || (l < tolW))
                                {

                                    double maxtol = .5 * (tolW + tolE + l), cW, cE;
                                    if (maxtol > tolW && maxtol > tolE)
                                    {
                                        cW = (maxtol - tolE) / l;
                                        cE = 1.0 - cW;
                                    }
                                    else if (maxtol > tolW) { maxtol = tolE; cW = 0.0; cE = 1.0; }
                                    else { maxtol = tolW; cW = 1.0; cE = 0.0; }

                                    gp_Pnt PC = new gp_Pnt(cW * PW.X() + cE * PE.X(), cW * PW.Y() + cE * PE.Y(), cW * PW.Z() + cE * PE.Z());

                                    B.UpdateVertex(VW, PC, maxtol);

                                    newvertex = true;
                                    myVertex = VW;
                                    myVertex.Orientation(VE.Orientation());
                                    B.Add(myEdge, myVertex);
                                    B.Transfert(EE, myEdge, VE, myVertex);
                                    break;
                                }

                            }
                            if (!newvertex)
                            {
                                myVertices.Add(VE);
                                B.Add(myEdge, VE);
                                B.Transfert(EE, myEdge, VE, VE);
                            }
                        }
                    }
                }
                // Make a decision about the orientation of the edge
                // If there is an ambiguity (in 3d) preserve the orientation given at input
                // Case of ambiguity :
                // reverse and forward are false as nothing has been decided : 
                //       closed wire, internal vertex ... 
                // reverse and forward are true : closed or degenerated edge
                if (((forward == reverse) && (E.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)) ||
                   (reverse && !forward)) myEdge.Reverse();
            }

            // add myEdge to myShape
            B.Add(myShape, myEdge);
            myShape.Closed(false);

            // Initialize VF, VL
            if (init)
                TopExp.Vertices(TopoDS.Wire(myShape), ref VF, ref VL);
            else
            {
                if (myError == BRepLib_WireError.BRepLib_WireDone)
                { // Update only
                    TopoDS_Vertex V1 = new TopoDS_Vertex(), V2 = new TopoDS_Vertex (), VRef = null;
                    TopExp.Vertices(myEdge, ref V1, ref V2);
                    if (V1.IsSame(myVertex)) VRef = V2;
                    else if (V2.IsSame(myVertex)) VRef = V1;
                    else
                    {
                        //# ifdef OCCT_DEBUG
                        //   std.cout << "MakeWire : There is a PROBLEM !!" << std.endl;
                        //#endif
                        myError = BRepLib_WireError.BRepLib_NonManifoldWire;
                    }

                    if (VF.IsSame(VL))
                    {
                        // Particular case: it is required to control the orientation
                        //# ifdef OCCT_DEBUG
                        //     if (!VF.IsSame(myVertex))
                        //       std.cout << "MakeWire : There is a PROBLEM !!" << std.endl;
                        //#endif

                    }
                    else
                    { // General case
                        if (VF.IsSame(myVertex)) VF = VRef;
                        else if (VL.IsSame(myVertex)) VL = VRef;
                        else
                        {
                            //# ifdef OCCT_DEBUG
                            //   std.cout << "MakeWire : Y A UN PROBLEME !!" << std.endl;
                            //#endif
                            myError = BRepLib_WireError.BRepLib_NonManifoldWire;
                        }
                    }
                }
                if (myError == BRepLib_WireError.BRepLib_NonManifoldWire)
                {
                    VF = VL = new TopoDS_Vertex(); // nullify
                }
            }
            // Test myShape is closed
            if (!VF.IsNull() && !VL.IsNull() && VF.IsSame(VL))
                myShape.Closed(true);

            myError = BRepLib_WireError.BRepLib_WireDone;
            Done();
        }
        BRepLib_WireError myError;
        TopoDS_Edge myEdge = new TopoDS_Edge();
        TopoDS_Vertex myVertex = new TopoDS_Vertex();
        TopTools_IndexedMapOfShape myVertices = new TopTools_IndexedMapOfShape();
        TopoDS_Vertex FirstVertex;
        TopoDS_Vertex VF;
        TopoDS_Vertex VL;
    }
}