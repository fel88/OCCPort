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
        internal TopoDS_Wire Wire()
        {
            return TopoDS.Wire(Shape());

        }
    }
}