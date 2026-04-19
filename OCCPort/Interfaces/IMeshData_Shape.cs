namespace OCCPort.Interfaces
{
    //! Interface class representing model with associated TopoDS_Shape.
    //! Intended for inheritance by structures and algorithms keeping 
    //! reference TopoDS_Shape.
    public interface  IMeshData_Shape
    {
        TopoDS_Shape GetShape();
        void  SetShape(TopoDS_Shape shape);

    }


}