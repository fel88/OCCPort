using OCCPort.Interfaces;

namespace OCCPort
{
    public abstract class AbstractMeshData_Shape: IMeshData_Shape
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

        //=======================================================================
        // Function: FacesNb
        // Purpose : 
        //=======================================================================
        public int FacesNb()
        {
            return myDFaces.Size();
        }

        protected VectorOfIFaceHandles myDFaces = new VectorOfIFaceHandles();
        //! Constructor.
        public AbstractMeshData_Shape()
        {
        }
        //! Constructor.
        public AbstractMeshData_Shape(TopoDS_Shape theShape)
        {
            myShape = (theShape);
        }

    }


}