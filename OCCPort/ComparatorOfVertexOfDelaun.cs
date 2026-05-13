using System;
using System.Collections.Generic;

namespace OCCPort
{
    //! Sort two points in projection on vector (1,1)
    public class ComparatorOfVertexOfDelaun : IComparer<BRepMesh_Vertex>
    {
        public int Compare(BRepMesh_Vertex theLeft, BRepMesh_Vertex theRight)
        {
            return Math.Sign((theLeft.Coord().X() + theLeft.Coord().Y()) - (theRight.Coord().X() + theRight.Coord().Y()));
        }
    };
}