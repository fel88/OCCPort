namespace OCCPort
{
	public abstract class IMeshData_Model : IMeshData_Shape
	{
		public IMeshData_Model(TopoDS_Shape theShape) : base(theShape)
		{
		}

		//! Returns number of faces in discrete model.
		public abstract int FacesNb();
	}
}