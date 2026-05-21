namespace OCCPort.Tester
{
    //! Contains points array definition.
    public class Graphic3d_ArrayOfPoints : Graphic3d_ArrayOfPrimitives
    {
        //! Creates an array of points (Graphic3d_TOPA_POINTS).
        //! The array must be filled using the AddVertex(Point) method.
        //! @param theMaxVertexs maximum number of points
        //! @param theArrayFlags array flags
        public Graphic3d_ArrayOfPoints(int theMaxVertexs,
                                 Graphic3d_ArrayFlags theArrayFlags)
        : base(Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POINTS, theMaxVertexs, 0, 0, theArrayFlags)
        {

        }

        //! Creates an array of points (Graphic3d_TOPA_POINTS).
        //! The array must be filled using the AddVertex(Point) method.
        //! @param theMaxVertexs  maximum number of points
        //! @param theHasVColors  when TRUE, AddVertex(Point,Color)  should be used for specifying vertex color
        //! @param theHasVNormals when TRUE, AddVertex(Point,Normal) should be used for specifying vertex normal
        public Graphic3d_ArrayOfPoints(int theMaxVertexs,
                                      bool theHasVColors = false,
                                      bool theHasVNormals = false)
             : base(Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POINTS, theMaxVertexs, 0, 0,
                                            (theHasVColors ? Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_VertexColor : Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_None)
                                    | (theHasVNormals ? Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_VertexNormal : Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_None))
        {

        }

    }
}