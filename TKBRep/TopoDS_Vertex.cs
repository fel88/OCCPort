using System;

namespace OCCPort
{

    //! Describes a vertex which
    //! - references an underlying vertex with the potential
    //! to be given a location and an orientation
    //! - has a location for the underlying vertex, giving its
    //! placement in the local coordinate system
    //! - has an orientation for the underlying vertex, in
    //! terms of its geometry (as opposed to orientation in
    //! relation to other shapes).
    public class TopoDS_Vertex : TopoDS_Shape
    {
        public TopoDS_Vertex() : base() { }
        public TopoDS_Vertex(TopoDS_Shape theOther) : base(theOther)
        {
        }

        public override void Orientation(TopAbs_Orientation theOrient) { base.Orientation(theOrient); }
        public override TopoDS_Shape Oriented(TopAbs_Orientation theOrient) {  return base.Oriented(theOrient);}
        public override void Reverse()
        {
            base.Reverse();
        }

    }
}