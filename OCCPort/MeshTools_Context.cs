using OCCPort.Interfaces;
using System.Reflection.Metadata;

namespace OCCPort
{
    public class MeshTools_Context : AbstractMeshData_Shape, IMeshTools_Context
    {
        protected IMeshTools_ModelBuilder myModelBuilder;
        IMeshData_Model myModel;
        IMeshTools_Parameters myParameters = new IMeshTools_Parameters();
        IMeshTools_ModelAlgo myEdgeDiscret;
        IMeshTools_ModelAlgo myPreProcessor;
        IMeshTools_ModelAlgo myPostProcessor;

        IMeshTools_ModelAlgo myModelHealer;
        //! Sets instance of post-processing algorithm.
        public void SetPostProcessor(IMeshTools_ModelAlgo thePostProcessor)
        {
            myPostProcessor = thePostProcessor;
        }
        //! Sets instance of pre-processing algorithm.
        public void SetPreProcessor(IMeshTools_ModelAlgo thePreProcessor)
        {
            myPreProcessor = thePreProcessor;
        }
        //! Sets instance of a tool to be used to heal discrete model.
        public void SetModelHealer(IMeshTools_ModelAlgo theModelHealer)
        {
            myModelHealer = theModelHealer;
        }
        //! Performs post-processing of discrete model using assigned algorithm.
        //! @return True on success, False elsewhere.
        public bool PostProcessModel()
        {
            if (myModel == null)
            {
                return false;
            }

            return myPostProcessor == null ?
              true :
              myPostProcessor.Perform(myModel, myParameters, new Message_ProgressRange());
        }

        //! Performs healing of discrete model built by DiscretizeEdges() method
        //! using assigned healing algorithm.
        //! @return True on success, False elsewhere.
        public bool HealModel()
        {
            if (myModel == null)
            {
                return false;
            }

            return myModelHealer == null ?
              true :
              myModelHealer.Perform(myModel, myParameters, new Message_ProgressRange());
        }
        //! Cleans temporary context data.
        public void Clean()
        {
            if (myParameters.CleanModel)
            {
                myModel = null;
            }
        }

        //! Performs pre-processing of discrete model using assigned algorithm.
        //! Performs auxiliary actions such as cleaning shape from old triangulation.
        //! @return True on success, False elsewhere.
        public bool PreProcessModel()
        {
            if (myModel == null)
            {
                return false;
            }

            return myPreProcessor == null ?
              true :
              myPreProcessor.Perform(myModel, myParameters, new Message_ProgressRange());
        }
        //! Sets instance of a tool to be used to discretize edges of a model.
        public void SetEdgeDiscret(IMeshTools_ModelAlgo theEdgeDiscret)
        {
            myEdgeDiscret = theEdgeDiscret;
        }

        //! Performs discretization of model edges using assigned edge discret algorithm.
        //! @return True on success, False elsewhere.
        public bool DiscretizeEdges()
        {
            if (myModel == null || myEdgeDiscret == null)
            {
                return false;
            }

            // Discretize edges of a model.
            return myEdgeDiscret.Perform(myModel, myParameters, new Message_ProgressRange());
        }

        //! Gets instance of a tool to be used to build discrete model.
        public IMeshTools_ModelBuilder GetModelBuilder()
        {
            return myModelBuilder;
        }

        //! Sets instance of a tool to be used to build discrete model.
        public void SetModelBuilder(IMeshTools_ModelBuilder theBuilder)
        {
            myModelBuilder = theBuilder;
        }

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

        public MeshTools_Context()
        {

        }

        //! Sets instance of meshing algorithm.
        public void SetFaceDiscret(IMeshTools_ModelAlgo theFaceDiscret)
        {
            myFaceDiscret = theFaceDiscret;
        }

        public MeshTools_Context(TopoDS_Shape theShape) : base(theShape)
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