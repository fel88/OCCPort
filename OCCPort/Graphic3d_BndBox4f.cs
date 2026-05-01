using System;

namespace OCCPort
{
    //! Redefines BVH_Box<Standard_ShortReal, 4> for AABB representation
    //! Describes rendering parameters and effects.

    public class Graphic3d_BndBox4f: BVH_Box//<float,4>
    {
        internal void Add(Graphic3d_Vec4 graphic3d_Vec4)
        {
            throw new NotImplementedException();
        }
    }
}