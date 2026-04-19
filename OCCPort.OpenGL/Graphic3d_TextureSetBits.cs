using System;

namespace OCCPort.OpenGL
{
    //! Standard texture units combination bits.
    
    enum Graphic3d_TextureSetBits 
    {
        Graphic3d_TextureSetBits_NONE = 0,
        Graphic3d_TextureSetBits_BaseColor = (1 << (Graphic3d_TextureUnit.Graphic3d_TextureUnit_BaseColor)),
        Graphic3d_TextureSetBits_Emissive = (1 << (Graphic3d_TextureUnit.Graphic3d_TextureUnit_Emissive)),
        Graphic3d_TextureSetBits_Occlusion = (1 << (Graphic3d_TextureUnit.Graphic3d_TextureUnit_Occlusion)),
        Graphic3d_TextureSetBits_Normal = (1 << (Graphic3d_TextureUnit.Graphic3d_TextureUnit_Normal)),
        Graphic3d_TextureSetBits_MetallicRoughness = (1 << (Graphic3d_TextureUnit.Graphic3d_TextureUnit_MetallicRoughness)),
    };

}
