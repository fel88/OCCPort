using System;
using System.Security.Cryptography;

namespace OCCPort
{
    //! Interface class representing context of BRepMesh algorithm.
    //! Intended to cache discrete model and instances of tools for 
    //! its processing.
    public class IMeshTools_Context : IMeshData_Shape
    {
        protected IMeshTools_ModelBuilder myModelBuilder;
        IMeshData_Model myModel;
        IMeshTools_Parameters myParameters = new IMeshTools_Parameters();
        public IMeshTools_Context()
        {

        }

        public IMeshTools_Context(TopoDS_Shape theShape) : base(theShape)
        {
        }

        //! Builds model using assigned model builder.
        //! @return True on success, False elsewhere.
        public virtual bool BuildModel()
        {
            if (myModelBuilder == null)            
                return false;            

            myModel = myModelBuilder.Perform(GetShape(), myParameters);
            return myModel != null;
        }

        //! Returns discrete model of a shape.
        public IMeshData_Model GetModel()
        {
            return myModel;
        }
    }
}