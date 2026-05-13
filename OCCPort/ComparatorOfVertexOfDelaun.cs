using System;
using System.Collections.Generic;

namespace OCCPort
{
    //! Sort two points in projection on vector (1,1)
    public class ComparatorOfVertexOfDelaun 
    {
        public bool Compare(BRepMesh_Vertex theLeft, BRepMesh_Vertex theRight)
        {
            return theLeft.Coord().X() + theLeft.Coord().Y() < theRight.Coord().X() + theRight.Coord().Y();
        }
    };
}