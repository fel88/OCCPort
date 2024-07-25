namespace OCCPort
{
	//! Interface class representing model with associated TopoDS_Shape.
	//! Intended for inheritance by structures and algorithms keeping 
	//! reference TopoDS_Shape.
	public class IMeshData_Shape
	{
		//! Returns shape assigned to discrete shape.
		public TopoDS_Shape GetShape()
		{
			return myShape;
		}
		TopoDS_Shape myShape;

	}


}