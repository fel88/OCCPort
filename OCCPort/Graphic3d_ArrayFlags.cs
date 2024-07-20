using System;

namespace OCCPort
{
    //! Graphic3d_ArrayFlags bitmask values.
    [Flags]
    public enum Graphic3d_ArrayFlags : int
    {
        Graphic3d_ArrayFlags_None = 0x00,  //!< no flags
        Graphic3d_ArrayFlags_VertexNormal = 0x01,  //!< per-vertex normal attribute
        Graphic3d_ArrayFlags_VertexColor = 0x02,  //!< per-vertex color  attribute
        Graphic3d_ArrayFlags_VertexTexel = 0x04,  //!< per-vertex texel coordinates (UV) attribute
        Graphic3d_ArrayFlags_BoundColor = 0x10,
        // advanced
        Graphic3d_ArrayFlags_AttribsMutable = 0x20,  //!< mutable array, which can be invalidated during lifetime without re-creation
        Graphic3d_ArrayFlags_AttribsDeinterleaved = 0x40,  //!< non-interleaved vertex attributes packed into single array
        Graphic3d_ArrayFlags_IndexesMutable = 0x80,  //!< mutable index array, which can be invalidated during lifetime without re-creation
    };

}