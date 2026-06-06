namespace TKService
{
    //! Contains polylines array definition.
    public class Graphic3d_ArrayOfPolylines : Graphic3d_ArrayOfPrimitives
    {
        //! Creates an array of polylines (Graphic3d_TOPA_POLYLINES).
        //! @param theMaxVertexs defines the maximum allowed vertex number in the array
        //! @param theMaxBounds  defines the maximum allowed bound  number in the array
        //! @param theMaxEdges   defines the maximum allowed edge   number in the array
        //! @param theHasVColors when TRUE AddVertex(Point,Color) or AddVertex(Point,Normal,Color) should be used to specify per-vertex color values
        //! @param theHasBColors when TRUE AddBound(number,Color) should be used to specify sub-group color
        public Graphic3d_ArrayOfPolylines(int theMaxVertexs,
                                        int theMaxBounds = 0,
                                        int theMaxEdges = 0,
                                        bool theHasVColors = false,
                                        bool theHasBColors = false)
            : base(Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POLYLINES, theMaxVertexs, theMaxBounds, theMaxEdges,
                                           (theHasVColors ? Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_VertexColor : Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_None)
                               | (theHasBColors ? Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_BoundColor : Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_None))
        { }
    }
}



