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

        //! Performs meshing of faces of discrete model using assigned meshing algorithm.
        //! @return True on success, False elsewhere.
        public bool DiscretizeFaces(Message_ProgressRange theRange)
        {
            if (myModel == null || myFaceDiscret == null)
            {
                return false;
            }

            // Discretize faces of a model.
            return myFaceDiscret.Perform(myModel, myParameters, theRange);
        }

        //! Gets instance of meshing algorithm.
        public IMeshTools_ModelAlgo GetFaceDiscret()
        {
            return myFaceDiscret;
        }

        IMeshTools_ModelAlgo myFaceDiscret;

        public IMeshTools_Context()
        {

        }

        //! Sets instance of meshing algorithm.
        public void SetFaceDiscret(IMeshTools_ModelAlgo theFaceDiscret)
        {
            myFaceDiscret = theFaceDiscret;
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