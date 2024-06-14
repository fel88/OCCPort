using System;
using System.Collections.Generic;

namespace OCCPort
{
    /*
     * 
  typedef NCollection_Shared< NCollection_DataMap<TCollection_AsciiString, Handle(OpenGl_Resource)> > OpenGl_ResourcesMap;

     */
    internal class OpenGl_ResourcesMap : Dictionary<string, OpenGl_Resource>
    {
        internal bool Bind(string theKey, OpenGl_ShaderProgram theResource)
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