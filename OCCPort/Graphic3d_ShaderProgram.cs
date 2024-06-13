using System;

namespace OCCPort
{
    internal class Graphic3d_ShaderProgram
    {
        string myID;
        //! Returns unique ID used to manage resource in graphic driver.
        string GetId() { return myID; }
        //! Attaches shader object to the program object.
        bool AttachShader(Graphic3d_ShaderObject theShader)
        {
            throw new Exception();
        }

    }
}