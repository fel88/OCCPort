using System;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace OCCPort.Interfaces
{
    //! Interface class representing context of BRepMesh algorithm.
    //! Intended to cache discrete model and instances of tools for 
    //! its processing.
    public interface IMeshTools_Context : IMeshData_Shape
    {
        void SetParameters(IMeshTools_Parameters p);

        //! Performs discretization of model edges using assigned edge discret algorithm.
        //! @return True on success, False elsewhere.
        bool DiscretizeEdges();
        bool HealModel();
        void Clean();
        IMeshTools_ModelBuilder GetModelBuilder();

        bool PreProcessModel();
        bool PostProcessModel();

        bool DiscretizeFaces(Message_ProgressRange theRange);
        IMeshData_Model GetModel();
        //! Builds model using assigned model builder.
        //! @return True on success, False elsewhere.
        bool BuildModel();
        IMeshTools_Parameters ChangeParameters();
    }
}