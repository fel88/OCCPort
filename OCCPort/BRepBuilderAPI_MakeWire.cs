using System;

namespace OCCPort
{
    //! Describes functions to build wires from edges. A wire can
    //! be built from any number of edges.
    //! To build a wire you first initialize the construction, then
    //! add edges in sequence. An unlimited number of edges
    //! can be added. The initialization of construction is done with:
    //! -   no edge (an empty wire), or
    //! -   edges of an existing wire, or
    //! -   up to four connectable edges.
    //! In order to be added to a wire under construction, an
    //! edge (unless it is the first one) must satisfy the following
    //! condition: one of its vertices must be geometrically
    //! coincident with one of the vertices of the wire (provided
    //! that the highest tolerance factor is assigned to the two
    //! vertices). It could also be the same vertex.
    //! -   The given edge is shared by the wire if it contains:
    //! -   two vertices, identical to two vertices of the wire
    //! under construction (a general case of the wire closure), or
    //! -   one vertex, identical to a vertex of the wire under
    //! construction; the other vertex not being
    //! geometrically coincident with another vertex of the wire.
    //! -   In other cases, when one of the vertices of the edge
    //! is simply geometrically coincident with a vertex of the
    //! wire under construction (provided that the highest
    //! tolerance factor is assigned to the two vertices), the
    //! given edge is first copied and the coincident vertex is
    //! replaced in this new edge, by the coincident vertex of the wire.
    //! Note: it is possible to build non manifold wires using this construction tool.
    //! A MakeWire object provides a framework for:
    //! -   initializing the construction of a wire,
    //! -   adding edges to the wire under construction, and
    //! -   consulting the result.
    public class BRepBuilderAPI_MakeWire : BRepBuilderAPI_MakeShape
    {
        public void Add(TopoDS_Edge e1)
        {
            throw new NotImplementedException();
        }

        public TopoDS_Wire Wire()
        {
            return myMakeWire.Wire();

        }
        BRepLib_MakeWire myMakeWire;

    }
}