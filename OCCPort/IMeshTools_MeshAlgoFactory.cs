using System;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace OCCPort
{
    //! Base interface for factories producing instances of triangulation
    //! algorithms taking into account type of surface of target face.
    public interface IMeshTools_MeshAlgoFactory
    {
        //! Creates instance of meshing algorithm for the given type of surface.
        IMeshTools_MeshAlgo GetAlgo(
        GeomAbs_SurfaceType theSurfaceType,
        ref IMeshTools_Parameters theParameters);

    }
}