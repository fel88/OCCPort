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
        bool DiscretizeFaces(Message_ProgressRange theRange);
        IMeshData_Model GetModel();
        //! Builds model using assigned model builder.
        //! @return True on success, False elsewhere.
        bool BuildModel();
    }
}