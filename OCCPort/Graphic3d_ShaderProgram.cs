using System;
using System.Diagnostics.Contracts;

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

        //! Pushes int uniform.
        public bool PushVariableInt(string theName, int theValue)
        {
            return PushVariable<int>(theName, theValue);
        }


        
        Graphic3d_ShaderObjectList myShaderObjects; //!< the list of attached shader objects
        Graphic3d_ShaderVariableList myVariables;     //!< the list of custom uniform variables
        Graphic3d_ShaderAttributeList myAttributes;    //!< the list of custom vertex attributes
        string myHeader;        //!< GLSL header with version code and used extensions

        bool PushVariable<T>(string theName,
                                                         T theValue)
        {
            Graphic3d_ShaderVariable aVariable = Graphic3d_ShaderVariable.Create(theName, theValue);
            if (aVariable == null || !aVariable.IsDone())
            {
                return false;
            }

            myVariables.Append(aVariable);
            return true;
        }



    }
}