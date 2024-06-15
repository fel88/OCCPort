using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class Graphic3d_ShaderVariableList : List<Graphic3d_ShaderVariable>
    {
        internal void Append(Graphic3d_ShaderVariable aVariable)
        {
            Add(aVariable);
        }
    }
}