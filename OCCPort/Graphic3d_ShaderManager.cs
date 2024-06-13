using System;

namespace OCCPort
{
    //! This class is responsible for generation of shader programs.
    public class Graphic3d_ShaderManager
    {


        //! Prepare standard GLSL program without lighting.
        //! @param theBits      [in] program bits
        //! @param theIsOutline [in] draw silhouette
        Graphic3d_ShaderProgram getStdProgramUnlit(int theBits,
                                                                           bool theIsOutline = false)
        {
            throw new NotImplementedException();
        }

    }
}
