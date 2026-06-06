using System;
using System.Collections.Generic;

namespace OCCPort.OpenGL
{
    /*
     * 
  typedef NCollection_Shared< NCollection_DataMap<TCollection_AsciiString, Handle(OpenGl_Resource)> > OpenGl_ResourcesMap;

     */
    public class OpenGl_ResourcesMap : NCollection_DataMap<string, OpenGl_Resource>
    {
        public bool Bind(string theKey, OpenGl_ShaderProgram theResource)
        {
            if (ContainsKey(theKey))
            {
                return false;
            }
            Add(theKey, theResource);
            return true;
        }
    }
}