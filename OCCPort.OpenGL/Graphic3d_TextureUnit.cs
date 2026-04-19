namespace OCCPort.OpenGL
{
    //! Texture unit.
    enum Graphic3d_TextureUnit
    {
        // value as index number
        Graphic3d_TextureUnit_0,
        Graphic3d_TextureUnit_1,
        Graphic3d_TextureUnit_2,
        Graphic3d_TextureUnit_3,
        Graphic3d_TextureUnit_4,
        Graphic3d_TextureUnit_5,
        Graphic3d_TextureUnit_6,
        Graphic3d_TextureUnit_7,
        Graphic3d_TextureUnit_8,
        Graphic3d_TextureUnit_9,
        Graphic3d_TextureUnit_10,
        Graphic3d_TextureUnit_11,
        Graphic3d_TextureUnit_12,
        Graphic3d_TextureUnit_13,
        Graphic3d_TextureUnit_14,
        Graphic3d_TextureUnit_15,

        // aliases

        //! sampler2D occSamplerBaseColor.
        //! RGB(A) base color of the material and alpha mask/opacity.
        Graphic3d_TextureUnit_BaseColor = Graphic3d_TextureUnit_0,
        //! sampler2D occSamplerEmissive.
        //! RGB emissive map controls the color and intensity of the light being emitted by the material.
        Graphic3d_TextureUnit_Emissive = Graphic3d_TextureUnit_1,
        //! sampler2D occSamplerOcclusion.
        //! Occlusion map indicating areas of indirect lighting.
        //! Encoded into RED channel, with 1.0 meaning no occlusion (full color intensity) and 0.0 complete occlusion (black).
        Graphic3d_TextureUnit_Occlusion = Graphic3d_TextureUnit_2,
        //! sampler2D occSamplerNormal.
        //! XYZ tangent space normal map.
        Graphic3d_TextureUnit_Normal = Graphic3d_TextureUnit_3,
        //! sampler2D occSamplerMetallicRoughness.
        //! Metalness + roughness of the material.
        //! Encoded into GREEN (roughness) + BLUE (metallic) channels,
        //! so that it can be optionally combined with occlusion texture (RED channel).
        Graphic3d_TextureUnit_MetallicRoughness = Graphic3d_TextureUnit_4,

        //! samplerCube occSampler0.
        //! Environment cubemap for background. Rendered by dedicated program and normally occupies first texture unit.
        Graphic3d_TextureUnit_EnvMap = Graphic3d_TextureUnit_0,

        //! sampler2D occSamplerPointSprite.
        //! Sprite alpha-mask or RGBA image mapped using point UV, additional to BaseColor (mapping using vertex UV).
        //! This texture unit is set Graphic3d_TextureUnit_1, so that it can be combined with Graphic3d_TextureUnit_BaseColor,
        //! while other texture maps (normal map and others) are unexpected and unsupported for points.
        //! Note that it can be overridden to Graphic3d_TextureUnit_0 for FFP fallback on hardware without multi-texturing.
        Graphic3d_TextureUnit_PointSprite = Graphic3d_TextureUnit_1,

        //! sampler2D occDepthPeelingDepth.
        //! 1st texture unit for Depth Peeling lookups.
        Graphic3d_TextureUnit_DepthPeelingDepth = -6,

        //! sampler2D occDepthPeelingFrontColor.
        //! 2nd texture unit for Depth Peeling lookups.
        Graphic3d_TextureUnit_DepthPeelingFrontColor = -5,

        //! sampler2D occShadowMapSampler.
        //! Directional light source shadowmap texture.
        Graphic3d_TextureUnit_ShadowMap = -4,

        //! sampler2D occEnvLUT.
        //! Lookup table for approximated PBR environment lighting.
        //! Configured as index at the end of available texture units - 3.
        Graphic3d_TextureUnit_PbrEnvironmentLUT = -3,

        //! sampler2D occDiffIBLMapSHCoeffs.
        //! Diffuse (irradiance) IBL map's spherical harmonics coefficients baked for PBR from environment cubemap image.
        //! Configured as index at the end of available texture units - 2.
        Graphic3d_TextureUnit_PbrIblDiffuseSH = -2,

        //! samplerCube occSpecIBLMap.
        //! Specular IBL (Image-Based Lighting) environment map baked for PBR from environment cubemap image.
        //! Configured as index at the end of available texture units - 1.
        Graphic3d_TextureUnit_PbrIblSpecular = -1,
    };
  //  enum
   // {
     //   Graphic3d_TextureUnit_NB = Graphic3d_TextureUnit_15 + 1,
  //  };

}
