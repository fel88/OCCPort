using System;

namespace OCCPort
{
    public class Graphic3d_ShaderProgram
    {
        string myID;
        //! Returns unique ID used to manage resource in graphic driver.
        public string GetId() { return myID; }
        //! Attaches shader object to the program object.
        bool AttachShader(Graphic3d_ShaderObject theShader)
        {
            throw new Exception();
        }

        public void AttachShader(object value)
        {
            throw new NotImplementedException();
        }

        public void SetNbLightsMax(int v)
        {
            throw new NotImplementedException();
        }

        public void SetNbShadowMaps(int v)
        {
            throw new NotImplementedException();
        }

        public void SetDefaultSampler(bool v)
        {
            throw new NotImplementedException();
        }

        public void SetAlphaTest(bool v)
        {
            throw new NotImplementedException();
        }

        public object ShaderObjects()
        {
            throw new NotImplementedException();
        }
    }
}