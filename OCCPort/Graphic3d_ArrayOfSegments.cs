using System;

namespace OCCPort.Tester
{
    public class Graphic3d_ArrayOfSegments : Graphic3d_ArrayOfPrimitives
    {
        

        public Graphic3d_ArrayOfSegments(int theMaxVertexs,
                             int theMaxEdges = 0,
                             bool theHasVColors = false)
            : base(Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_SEGMENTS, theMaxVertexs, 0, theMaxEdges,
                 theHasVColors ? Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_VertexColor :
               Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_None)
        {

        }


    }
}