using System;

namespace OCCPort
{
    internal class Graphic3d_ShaderVariable
    {
        public Graphic3d_ShaderVariable(string theName)
        {
        }

        internal static Graphic3d_ShaderVariable Create<T>(string theName, T theValue)
        {
            Graphic3d_ShaderVariable theVariable = new Graphic3d_ShaderVariable(theName);
            theVariable.myValue = new Graphic3d_UniformValue<T>(theValue);
            return theVariable;

        }

        //! The name of uniform shader variable.
        string myName;

        //! The generic value of shader variable.
        Graphic3d_ValueInterface myValue;
        internal bool IsDone()
        {
            throw new NotImplementedException();
        }
    }
}