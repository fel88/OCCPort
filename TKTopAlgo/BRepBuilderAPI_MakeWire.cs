using OCCPort;

namespace TKTopAlgo
{
    //! Describes functions to build wires from edges. A wire can
    //! be built from any number of edges.
    //! To build a wire you first initialize the  ruction, then
    //! add edges in sequence. An unlimited number of edges
    //! can be added. The initialization of  ruction is done with:
    //! -   no edge (an empty wire), or
    //! -   edges of an existing wire, or
    //! -   up to four connectable edges.
    //! In order to be added to a wire under  ruction, an
    //! edge (unless it is the first one) must satisfy the following
    //! condition: one of its vertices must be geometrically
    //! coincident with one of the vertices of the wire (provided
    //! that the highest tolerance factor is assigned to the two
    //! vertices). It could also be the same vertex.
    //! -   The given edge is shared by the wire if it contains:
    //! -   two vertices, identical to two vertices of the wire
    //! under  ruction (a general case of the wire closure), or
    //! -   one vertex, identical to a vertex of the wire under
    //!  ruction; the other vertex not being
    //! geometrically coincident with another vertex of the wire.
    //! -   In other cases, when one of the vertices of the edge
    //! is simply geometrically coincident with a vertex of the
    //! wire under  ruction (provided that the highest
    //! tolerance factor is assigned to the two vertices), the
    //! given edge is first copied and the coincident vertex is
    //! replaced in this new edge, by the coincident vertex of the wire.
    //! Note: it is possible to build non manifold wires using this  ruction tool.
    //! A MakeWire object provides a framework for:
    //! -   initializing the  ruction of a wire,
    //! -   adding edges to the wire under  ruction, and
    //! -   consulting the result.
    public class BRepBuilderAPI_MakeWire : BRepBuilderAPI_MakeShape
    {


        //! Adds the edge E to the wire under construction.
        //! E must be connectable to the wire under construction, and, unless it
        //! is the first edge of the wire, must satisfy the following
        //! condition: one of its vertices must be geometrically coincident
        //! with one of the vertices of the wire (provided that the highest
        //! tolerance factor is assigned to the two vertices). It could also
        //! be the same vertex.
        //! Warning
        //! If E is not connectable to the wire under construction it is not
        //! added. The function Error will return
        //! BRepBuilderAPI_DisconnectedWire, the function IsDone will return
        //! false and the function Wire will raise an error, until a new
        //! connectable edge is added.
        public void Add(TopoDS_Edge E)
        {
            myMakeWire.Add(E);
            if (myMakeWire.IsDone())
            {
                Done();
                myShape = myMakeWire.Wire();
            }
        }


        public TopoDS_Wire Wire()
        {
            return myMakeWire.Wire();
        }

        BRepLib_MakeWire myMakeWire = new BRepLib_MakeWire();
    }
}