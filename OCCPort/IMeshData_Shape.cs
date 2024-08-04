namespace OCCPort
{
    //! Interface class representing model with associated TopoDS_Shape.
    //! Intended for inheritance by structures and algorithms keeping 
    //! reference TopoDS_Shape.
    public abstract class IMeshData_Shape
    {
        //! Returns shape assigned to discrete shape.
        public TopoDS_Shape GetShape()
        {
            return myShape;
        }
        TopoDS_Shape myShape;

        //! Assigns shape to discrete shape.
        public void SetShape(TopoDS_Shape theShape)
        {
            myShape = theShape;
        }

        //! Constructor.
        public IMeshData_Shape()
        {
        }
        //! Constructor.
        public IMeshData_Shape(TopoDS_Shape theShape)
        {
            myShape = (theShape);
        }

    }


}