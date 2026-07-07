//! List of shader objects.
//! List of custom vertex shader attributes

namespace OCCPort.OpenGL
{
    public static class ShadersConstants
    {
        // This file has been automatically generated from resource file src/Shaders/PBRSpotLight.glsl

        public  const string Shaders_PBRSpotLight_glsl =
          "//! Function computes contribution of spotlight source\n"
 + "//! into global variable DirectLighting (PBR shading).\n"
 + "//! @param theId      light source index\n"
 + "//! @param theNormal  surface normal\n"
 + "//! @param theView    view direction\n"
 + "//! @param thePoint   3D position (world space)\n"
 + "//! @param theIsFront front/back face flag\n"
 + "void occSpotLight (in int  theId,\n"
 + "                   in vec3 theNormal,\n"
 + "                   in vec3 theView,\n"
 + "                   in vec3 thePoint,\n"
 + "                   in bool theIsFront,\n"
 + "                   in float theShadow)\n"
 + "{\n"
 + "  vec3 aLight = occLight_Position (theId) - thePoint;\n"
 + "\n"
 + "  float aDist = length (aLight);\n"
 + "  float aRange = occLight_Range (theId);\n"
 + "  float anAtten = occPointLightAttenuation (aDist, aRange);\n"
 + "  if (anAtten <= 0.0) return;\n"
 + "  aLight /= aDist;\n"
 + "\n"
 + "  vec3 aSpotDir = occLight_SpotDirection (theId);\n"
 + "  // light cone\n"
 + "  float aCosA = dot (aSpotDir, -aLight);\n"
 + "  float aRelativeAngle = 2.0 * acos(aCosA) / occLight_SpotCutOff(theId);\n"
 + "  if (aCosA >= 1.0 || aRelativeAngle > 1.0)\n"
 + "  {\n"
 + "    return;\n"
 + "  }\n"
 + "  float anExponent = occLight_SpotExponent (theId);\n"
 + "  if ((1.0 - aRelativeAngle) <= anExponent)\n"
 + "  {\n"
 + "    float anAngularAttenuationOffset = cos(0.5 * occLight_SpotCutOff(theId));\n"
 + "    float anAngularAttenuationScale = 1.0 / max(0.001, cos(0.5 * occLight_SpotCutOff(theId) * (1.0 - anExponent)) - anAngularAttenuationOffset);\n"
 + "    anAngularAttenuationOffset *= -anAngularAttenuationScale;\n"
 + "    float anAngularAttenuantion = clamp(aCosA * anAngularAttenuationScale + anAngularAttenuationOffset, 0.0, 1.0);\n"
 + "    anAtten *= anAngularAttenuantion * anAngularAttenuantion;\n"
 + "  }\n"
 + "  theNormal = theIsFront ? theNormal : -theNormal;\n"
 + "  DirectLighting += occPBRIllumination (theView, aLight, theNormal,\n"
 + "                                        BaseColor, Metallic, Roughness, IOR,\n"
 + "                                        occLight_Specular(theId),\n"
 + "                                        occLight_Intensity(theId) * anAtten) * theShadow;\n"
 + "}\n";

        // This file has been automatically generated from resource file src/Shaders/PhongDirectionalLight.glsl

        public const string Shaders_PhongDirectionalLight_glsl =
          "//! Function computes contribution of directional light source\n"
+  "//! into global variables Diffuse and Specular (Phong shading).\n"
+  "//! @param theId      light source index\n"
+  "//! @param theNormal  surface normal\n"
+  "//! @param theView    view direction\n"
+  "//! @param theIsFront front/back face flag\n"
+  "//! @param theShadow  shadow attenuation\n"
+  "void occDirectionalLight (in int  theId,\n"
+  "                          in vec3 theNormal,\n"
+  "                          in vec3 theView,\n"
+  "                          in bool theIsFront,\n"
+  "                          in float theShadow)\n"
+  "{\n"
+  "  vec3 aLight = occLight_Position (theId);\n"
+  "  vec3 aHalf = normalize (aLight + theView);\n"
+  "\n"
+  "  vec3  aFaceSideNormal = theIsFront ? theNormal : -theNormal;\n"
+  "  float aNdotL = max (0.0, dot (aFaceSideNormal, aLight));\n"
+  "  float aNdotH = max (0.0, dot (aFaceSideNormal, aHalf ));\n"
+  "\n"
+  "  float aSpecl = 0.0;\n"
+  "  if (aNdotL > 0.0)\n"
+  "  {\n"
+  "    aSpecl = pow (aNdotH, occMaterial_Shininess (theIsFront));\n"
+  "  }\n"
+  "\n"
+  "  Diffuse  += occLight_Diffuse  (theId) * aNdotL * theShadow;\n"
+  "  Specular += occLight_Specular (theId) * aSpecl * theShadow;\n"
+  "}\n";

        // This file has been automatically generated from resource file src/Shaders/PBRDirectionalLight.glsl

        public const string  Shaders_PBRDirectionalLight_glsl =
          "//! Function computes contribution of directional light source\n"
+  "//! into global variable DirectLighting (PBR shading).\n"
+  "//! @param theId      light source index\n"
+  "//! @param theNormal  surface normal\n"
+  "//! @param theView    view direction\n"
+  "//! @param theIsFront front/back face flag\n"
+  "//! @param theShadow  shadow attenuation\n"
+  "void occDirectionalLight (in int  theId,\n"
+  "                          in vec3 theNormal,\n"
+  "                          in vec3 theView,\n"
+  "                          in bool theIsFront,\n"
+  "                          in float theShadow)\n"
+  "{\n"
+  "  vec3 aLight = occLight_Position (theId);\n"
+  "  theNormal = theIsFront ? theNormal : -theNormal;\n"
+  "  DirectLighting += occPBRIllumination (theView, aLight, theNormal,\n"
+  "                                        BaseColor, Metallic, Roughness, IOR,\n"
+  "                                        occLight_Specular (theId),\n"
+  "                                        occLight_Intensity(theId)) * theShadow;\n"
+  "}\n";


        // This file has been automatically generated from resource file src/Shaders/PBRPointLight.glsl

        public const string  Shaders_PBRPointLight_glsl =
          "//! Function computes contribution of isotropic point light source\n"
+  "//! into global variable DirectLighting (PBR shading).\n"
+  "//! @param theId      light source index\n"
+  "//! @param theNormal  surface normal\n"
+  "//! @param theView    view direction\n"
+  "//! @param thePoint   3D position (world space)\n"
+  "//! @param theIsFront front/back face flag\n"
+  "void occPointLight (in int  theId,\n"
+  "                    in vec3 theNormal,\n"
+  "                    in vec3 theView,\n"
+  "                    in vec3 thePoint,\n"
+  "                    in bool theIsFront)\n"
+  "{\n"
+  "  vec3 aLight = occLight_Position (theId) - thePoint;\n"
+  "\n"
+  "  float aDist = length (aLight);\n"
+  "  float aRange = occLight_Range (theId);\n"
+  "  float anAtten = occPointLightAttenuation (aDist, aRange);\n"
+  "  if (anAtten <= 0.0) return;\n"
+  "  aLight /= aDist;\n"
+  "\n"
+  "  theNormal = theIsFront ? theNormal : -theNormal;\n"
+  "  DirectLighting += occPBRIllumination (theView, aLight, theNormal,\n"
+  "                                        BaseColor, Metallic, Roughness, IOR,\n"
+  "                                        occLight_Specular (theId),\n"
+  "                                        occLight_Intensity(theId) * anAtten);\n"
+  "}\n";

        // This file has been automatically generated from resource file src/Shaders/PhongSpotLight.glsl

        public const  string Shaders_PhongSpotLight_glsl =
          "//! Function computes contribution of spotlight source\n"                                                         +
  "//! into global variables Diffuse and Specular (Phong shading).\n"                                                        +
  "//! @param theId      light source index\n"                                                                               +
  "//! @param theNormal  surface normal\n"                                                                                   +
  "//! @param theView    view direction\n"                                                                                   +
  "//! @param thePoint   3D position (world space)\n"                                                                        +
  "//! @param theIsFront front/back face flag\n"                                                                             +
  "//! @param theShadow  the value from shadow map\n"                                                                        +
  "void occSpotLight (in int  theId,\n"                                                                                      +
  "                   in vec3 theNormal,\n"                                                                                  +
  "                   in vec3 theView,\n"                                                                                    +
  "                   in vec3 thePoint,\n"                                                                                   +
  "                   in bool theIsFront,\n"                                                                                 +
  "                   in float theShadow)\n"                                                                                 +
  "{\n"                                                                                                                      +
  "  vec3 aLight = occLight_Position (theId) - thePoint;\n"                                                                  +
  "\n"                                                                                                                       +
  "  float aDist = length (aLight);\n"                                                                                       +
  "  float aRange = occLight_Range (theId);\n"                                                                               +
  "  float anAtten = occPointLightAttenuation (aDist, aRange, occLight_LinearAttenuation (theId), occLight_ConstAttenuation (theId));\n"+
  "  if (anAtten <= 0.0) return;\n"                                                                                                     +
  "  aLight /= aDist;\n"                                                                                                                +
  "\n"                                                                                                                                  +
  "  vec3 aSpotDir = occLight_SpotDirection (theId);\n"                                                                                 +
  "  // light cone\n"                                                                                                                   +
  "  float aCosA = dot (aSpotDir, -aLight);\n"                                                                                          +
  "  if (aCosA >= 1.0 || aCosA < cos (occLight_SpotCutOff (theId)))\n"                                                                  +
  "  {\n"                                                                                                                               +
  "    return;\n"                                                                                                                       +
  "  }\n"                                                                                                                               +
  "\n"                                                                                                                                  +
  "  float anExponent = occLight_SpotExponent (theId);\n"                                                                               +
  "  if (anExponent > 0.0)\n"                                                                                                           +
  "  {\n"                                                                                                                               +
  "    anAtten *= pow (aCosA, anExponent * 128.0);\n"                                                                                   +
  "  }\n"                                                                                                                               +
  "\n"                                                                                                                                  +
  "  vec3 aHalf = normalize (aLight + theView);\n"                                                                                      +
  "\n"                                                                                                                                  +
  "  vec3  aFaceSideNormal = theIsFront ? theNormal : -theNormal;\n"                                                                    +
  "  float aNdotL = max (0.0, dot (aFaceSideNormal, aLight));\n"                                                                        +
  "  float aNdotH = max (0.0, dot (aFaceSideNormal, aHalf ));\n"                                                                        +
  "\n"                                                                                                                                  +
  "  float aSpecl = 0.0;\n"                                                                                                             +
  "  if (aNdotL > 0.0)\n"                                                                                                               +
  "  {\n"                                                                                                                               +
  "    aSpecl = pow (aNdotH, occMaterial_Shininess (theIsFront));\n"                                                                    +
  "  }\n"                                                                                                                               +
  "\n"                                                                                                                                  +
  "  Diffuse  += occLight_Diffuse (theId) * aNdotL * anAtten * theShadow;\n"                                                            +
  "  Specular += occLight_Specular(theId) * aSpecl * anAtten * theShadow;\n"                                                            +
  "}\n";


        // This file has been automatically generated from resource file src/Shaders/PhongPointLight.glsl

        public const string Shaders_PhongPointLight_glsl =
          "//! Function computes contribution of isotropic point light source\n"                                                   +
  "//! into global variables Diffuse and Specular (Phong shading).\n"                                                              +
  "//! @param theId      light source index\n"                                                                                     +
  "//! @param theNormal  surface normal\n"                                                                                         +
  "//! @param theView    view direction\n"                                                                                         +
  "//! @param thePoint   3D position (world space)\n"                                                                              +
  "//! @param theIsFront front/back face flag\n"                                                                                   +
  "void occPointLight (in int  theId,\n"                                                                                           +
  "                    in vec3 theNormal,\n"                                                                                       +
  "                    in vec3 theView,\n"                                                                                         +
  "                    in vec3 thePoint,\n"                                                                                        +
  "                    in bool theIsFront)\n"                                                                                      +
  "{\n"                                                                                                                            +
  "  vec3 aLight = occLight_Position (theId) - thePoint;\n"                                                                        +
  "\n"                                                                                                                             +
  "  float aDist = length (aLight);\n"                                                                                             +
  "  float aRange = occLight_Range (theId);\n"+
  "  float anAtten = occPointLightAttenuation (aDist, aRange, occLight_LinearAttenuation (theId), occLight_ConstAttenuation (theId));\n"+
  "  if (anAtten <= 0.0) return;\n"                                                                                                      +
  "  aLight /= aDist;\n"                                                                                                                 +
  "\n"                                                                                                                                   +
  "  vec3 aHalf = normalize (aLight + theView);\n"                                                                                       +
  "\n"                                                                                                                                   +
  "  vec3  aFaceSideNormal = theIsFront ? theNormal : -theNormal;\n"                                                                     +
  "  float aNdotL = max (0.0, dot (aFaceSideNormal, aLight));\n"                                                                         +
  "  float aNdotH = max (0.0, dot (aFaceSideNormal, aHalf ));\n"                                                                         +
  "\n"                                                                                                                                   +
  "  float aSpecl = 0.0;\n"                                                                                                              +
  "  if (aNdotL > 0.0)\n"                                                                                                                +
  "  {\n"                                                                                                                                +
  "    aSpecl = pow (aNdotH, occMaterial_Shininess (theIsFront));\n"                                                                     +
  "  }\n"                                                                                                                                +
  "\n"                                                                                                                                   +
  "  Diffuse  += occLight_Diffuse (theId) * aNdotL * anAtten;\n"                                                                         +
  "  Specular += occLight_Specular(theId) * aSpecl * anAtten;\n"+
  "}\n";

        // This file has been automatically generated from resource file src/Shaders/PointLightAttenuation.glsl

        public const  string Shaders_PointLightAttenuation_glsl =
          "//! Returns point light source attenuation factor\n"                                                                  +
  "float occRangedPointLightAttenuation (in float theDistance, in float theRange)\n"                                             +
  "{\n"                                                                                                                          +
  "  if (theDistance <= theRange)\n"                                                                                             +
  "  {\n"                                                                                                                        +
  "    float aResult = theDistance / theRange;\n"                                                                                +
  "    aResult *= aResult;\n"                                                                                                    +
  "    aResult *= aResult;\n"                                                                                                    +
  "    aResult = 1.0 - aResult;\n"                                                                                               +
  "    aResult = clamp(aResult, 0.0, 1.0);\n"                                                                                    +
  "    aResult /= max(0.0001, theDistance * theDistance);\n"                                                                     +
  "    return aResult;\n"                                                                                                        +
  "  }\n"                                                                                                                        +
  "  return -1.0;\n"                                                                                                             +
  "}\n"                                                                                                                          +
  "\n"                                                                                                                           +
  "//! Returns point light source attenuation factor with quadratic attenuation in case of zero range.\n"                        +
  "float occPointLightAttenuation (in float theDistance, in float theRange)\n"                                                   +
  "{\n"                                                                                                                          +
  "  if (theRange == 0.0)\n"                                                                                                     +
  "  {\n"                                                                                                                        +
  "    return 1.0 / max(0.0001, theDistance * theDistance);\n"                                                                   +
  "  }\n"                                                                                                                        +
  "  return occRangedPointLightAttenuation (theDistance, theRange);\n"                                                           +
  "}\n"                                                                                                                          +
  "\n"                                                                                                                           +
  "//! Returns point light source attenuation factor with linear attenuation in case of zero range.\n"+
  "float occPointLightAttenuation (in float theDistance, in float theRange, in float theLinearAttenuation, in float theConstAttenuation)\n"+
  "{\n"+
  "  if (theRange == 0.0)\n"+
  "  {\n"+
  "    return 1.0 / (theConstAttenuation + theLinearAttenuation * theDistance);\n"+
  "  }\n"+
  "  return occRangedPointLightAttenuation (theDistance, theRange);\n"+
  "}\n";

        // This file has been automatically generated from resource file src/Shaders/LightShadow.glsl

        public const string  Shaders_LightShadow_glsl =
          "#if (__VERSION__ >= 120)\n"+
  "//! Coefficients for gathering close samples for antialiasing.\n"                                                                       +
  "//! Use only with decent OpenGL (array constants cannot be initialized with GLSL 1.1 / GLSL ES 1.1)\n"                                  +
  "const vec2 occPoissonDisk16[16] = vec2[](\n"                                                                                            +
  " vec2(-0.94201624,-0.39906216), vec2( 0.94558609,-0.76890725), vec2(-0.09418410,-0.92938870), vec2( 0.34495938, 0.29387760),\n"         +
  " vec2(-0.91588581, 0.45771432), vec2(-0.81544232,-0.87912464), vec2(-0.38277543, 0.27676845), vec2( 0.97484398, 0.75648379),\n"         +
  " vec2( 0.44323325,-0.97511554), vec2( 0.53742981,-0.47373420), vec2(-0.26496911,-0.41893023), vec2( 0.79197514, 0.19090188),\n"         +
  " vec2(-0.24188840, 0.99706507), vec2(-0.81409955, 0.91437590), vec2( 0.19984126, 0.78641367), vec2( 0.14383161,-0.14100790)\n"          +
  ");\n"                                                                                                                                   +
  "#endif\n"                                                                                                                               +
  "\n"                                                                                                                                     +
  "//! Function computes directional and spot light shadow attenuation (1.0 means no shadow).\n"                                           +
  "float occLightShadow (in sampler2D theShadow,\n"                                                                                        +
  "                      in int  theId,\n"                                                                                                 +
  "                      in vec3 theNormal)\n"                                                                                             +
  "{\n"                                                                                                                                    +
  "  vec4 aPosLightSpace = PosLightSpace[occLight_Index(theId)];\n"                                                                        +
  "  vec3 aLightDir = occLight_Position (theId);\n"                                                                                        +
  "  vec3 aProjCoords = (aPosLightSpace.xyz / aPosLightSpace.w);\n"                                                                        +
  "#ifdef THE_ZERO_TO_ONE_DEPTH\n"                                                                                                         +
  "  aProjCoords.xy = aProjCoords.xy * 0.5 + vec2 (0.5);\n"                                                                                +
  "#else\n"                                                                                                                                +
  "  aProjCoords = aProjCoords * 0.5 + vec3 (0.5);\n"                                                                                      +
  "#endif\n"                                                                                                                               +
  "  float aCurrentDepth = aProjCoords.z;\n"                                                                                               +
  "  if (aProjCoords.x < 0.0 || aProjCoords.x > 1.0\n"                                                                                     +
  "   || aProjCoords.y < 0.0 || aProjCoords.y > 1.0\n"                                                                                     +
  "   || aCurrentDepth > 1.0)\n"                                                                                                           +
  "  {\n"                                                                                                                                  +
  "    return 1.0;\n"                                                                                                                      +
  "  }\n"                                                                                                                                  +
  "\n"                                                                                                                                     +
  "  vec2 aTexelSize = vec2 (occShadowMapSizeBias.x);\n"                                                                                   +
  "  float aBias = max (occShadowMapSizeBias.y * (1.0 - dot (theNormal, aLightDir)), occShadowMapSizeBias.y * 0.1);\n"                     +
  "#if (__VERSION__ >= 120)\n"                                                                                                             +
  "  float aShadow = 0.0;\n"                                                                                                               +
  "  for (int aPosIter = 0; aPosIter < 16; ++aPosIter)\n"                                                                                  +
  "  {\n"                                                                                                                                  +
  "    float aClosestDepth = occTexture2D (theShadow, aProjCoords.xy + occPoissonDisk16[aPosIter] * aTexelSize).r;\n"                      +
  "    aShadow += (aCurrentDepth - aBias) > aClosestDepth ? 1.0 : 0.0;\n"                                                                  +
  "  }\n"                                                                                                                                  +
  "  return 1.0 - aShadow / 16.0;\n"                                                                                                       +
  "#else\n"                                                                                                                                +
  "  float aClosestDepth = occTexture2D (theShadow, aProjCoords.xy).r;\n"                                                                  +
  "  float aShadow = (aCurrentDepth - aBias) > aClosestDepth ? 1.0 : 0.0;\n"                                                               +
  "  return 1.0 - aShadow;\n"                                                                                                              +
  "#endif\n"                                                                                                                               +
  "}\n";

        // This file has been automatically generated from resource file src/Shaders/Declarations.glsl

        public const string Shaders_Declarations_glsl =
         "\n" +
 "//! @file Declarations.glsl includes definition of common uniform variables in OCCT GLSL programs\n" +
 "//! @def THE_MAX_LIGHTS\n" +
 "//! Specifies the length of array of lights, which is 8 by default. Defined by Shader Manager.\n" +
 "// #define THE_MAX_LIGHTS 8\n" +
 "\n" +
 "//! @def THE_MAX_CLIP_PLANES\n" +
 "//! Specifies the length of array of clipping planes, which is 8 by default. Defined by Shader Manager.\n" +
 "// #define THE_MAX_CLIP_PLANES 8\n" +
 "\n" +
 "//! @def THE_NB_FRAG_OUTPUTS\n" +
 "//! Specifies the length of array of Fragment Shader outputs, which is 1 by default. Defined by Shader Manager.\n" +
 "// #define THE_NB_FRAG_OUTPUTS 1\n" +
 "\n" +
 "// compatibility macros\n" +
 "#if (__VERSION__ >= 130)\n" +
 "  #define THE_ATTRIBUTE  in\n" +
 "  #define THE_SHADER_IN  in\n" +
 "  #define THE_SHADER_OUT out\n" +
 "  #define THE_OUT        out\n" +
 "  #define occTexture1D   texture\n" +
 "  #define occTexture2D   texture\n" +
 "  #define occTexture3D   texture\n" +
 "  #define occTextureCube texture\n" +
 "  #define occTextureCubeLod textureLod\n" +
 "#else\n" +
 "  #define THE_ATTRIBUTE  attribute\n" +
 "  #define THE_SHADER_IN  varying\n" +
 "  #define THE_SHADER_OUT varying\n" +
 "  #define THE_OUT\n" +
 "  #define occTexture1D   texture1D\n" +
 "  #define occTexture2D   texture2D\n" +
 "  #define occTexture3D   texture3D\n" +
 "  #define occTextureCube textureCube\n" +
 "  #if !defined(GL_ES) || defined(textureCubeLod)\n" +
 "    #define occTextureCubeLod textureCubeLod\n" +
 "  #else // fallback\n" +
 "    #define occTextureCubeLod(theSampl,theCoord,theLod) textureCube(theSampl,theCoord)\n" +
 "  #endif\n" +
 "#endif\n" +
 "\n" +
 "#ifdef GL_ES\n" +
 "#if (__VERSION__ >= 300)\n" +
 "  #define THE_PREC_ENUM highp // lowp should be enough for enums but triggers driver bugs\n" +
 "#else\n" +
 "  #define THE_PREC_ENUM lowp\n" +
 "#endif\n" +
 "#else\n" +
 "  #define THE_PREC_ENUM\n" +
 "#endif\n" +
 "\n" +
 "// Vertex attributes\n" +
 "#ifdef VERTEX_SHADER\n" +
 "  THE_ATTRIBUTE vec4 occVertex;\n" +
 "  THE_ATTRIBUTE vec3 occNormal;\n" +
 "  THE_ATTRIBUTE vec4 occTexCoord;\n" +
 "  THE_ATTRIBUTE vec4 occVertColor;\n" +
 "#elif defined(FRAGMENT_SHADER)\n" +
 "  #if (__VERSION__ >= 130)\n" +
 "    #ifdef OCC_ENABLE_draw_buffers\n" +
 "      out vec4 occFragColorArray[THE_NB_FRAG_OUTPUTS];\n" +
 "      #define occFragColorArrayAlias occFragColorArray\n" +
 "      #define occFragColor0 occFragColorArray[0]\n" +
 "    #else\n" +
 "      out vec4 occFragColor0;\n" +
 "    #endif\n" +
 "  #else\n" +
 "    #ifdef OCC_ENABLE_draw_buffers\n" +
 "      #define occFragColorArrayAlias gl_FragData\n" +
 "      #define occFragColor0 gl_FragData[0]\n" +
 "    #else\n" +
 "      #define occFragColor0 gl_FragColor\n" +
 "    #endif\n" +
 "  #endif\n" +
 "\n" +
 "  #if (THE_NB_FRAG_OUTPUTS >= 2)\n" +
 "    #define occFragColor1 occFragColorArrayAlias[1]\n" +
 "  #else\n" +
 "    vec4 occFragColor1;\n" +
 "  #endif\n" +
 "  #if (THE_NB_FRAG_OUTPUTS >= 3)\n" +
 "    #define occFragColor2 occFragColorArrayAlias[2]\n" +
 "  #else\n" +
 "    vec4 occFragColor2;\n" +
 "  #endif\n" +
 "  #if (THE_NB_FRAG_OUTPUTS >= 4)\n" +
 "    #define occFragColor3 occFragColorArrayAlias[3]\n" +
 "  #else\n" +
 "    vec4 occFragColor3;\n" +
 "  #endif\n" +
 "\n" +
 "  // Built-in outputs notation\n" +
 "  #define occFragColor    occFragColor0\n" +
 "  #define occFragCoverage occFragColor1\n" +
 "\n" +
 "  #define occPeelDepth      occFragColor0\n" +
 "  #define occPeelFrontColor occFragColor1\n" +
 "  #define occPeelBackColor  occFragColor2\n" +
 "\n" +
 "  //! Define the main Fragment Shader early return procedure.\n" +
 "  bool occFragEarlyReturn();\n" +
 "\n" +
 "  //! Define the main Fragment Shader output - color value.\n" +
 "  void occSetFragColor (in vec4 theColor);\n" +
 "#endif\n" +
 "\n" +
 "// Pi number definitions\n" +
 "#define PI       3.141592654\n" +
 "#define PI_2     6.283185307\n" +
 "#define PI_DIV_2 1.570796327\n" +
 "#define PI_DIV_3 1.047197551\n" +
 "#define PI_DIV_4 0.785398163\n" +
 "#define INV_PI   0.318309886\n" +
 "#define INV_PI_2 0.159154943\n" +
 "\n" +
 "// Matrix state\n" +
 "uniform mat4 occWorldViewMatrix;  //!< World-view  matrix\n" +
 "uniform mat4 occProjectionMatrix; //!< Projection  matrix\n" +
 "uniform mat4 occModelWorldMatrix; //!< Model-world matrix\n" +
 "\n" +
 "uniform mat4 occWorldViewMatrixInverse;    //!< Inverse of the world-view  matrix\n" +
 "uniform mat4 occProjectionMatrixInverse;   //!< Inverse of the projection  matrix\n" +
 "uniform mat4 occModelWorldMatrixInverse;   //!< Inverse of the model-world matrix\n" +
 "\n" +
 "uniform mat4 occWorldViewMatrixTranspose;  //!< Transpose of the world-view  matrix\n" +
 "uniform mat4 occProjectionMatrixTranspose; //!< Transpose of the projection  matrix\n" +
 "uniform mat4 occModelWorldMatrixTranspose; //!< Transpose of the model-world matrix\n" +
 "\n" +
 "uniform mat4 occWorldViewMatrixInverseTranspose;  //!< Transpose of the inverse of the world-view  matrix\n" +
 "uniform mat4 occProjectionMatrixInverseTranspose; //!< Transpose of the inverse of the projection  matrix\n" +
 "uniform mat4 occModelWorldMatrixInverseTranspose; //!< Transpose of the inverse of the model-world matrix\n" +
 "\n" +
 "#if defined(THE_IS_PBR)\n" +
 "uniform sampler2D   occEnvLUT;             //!< Environment Lookup Table\n" +
 "uniform sampler2D   occDiffIBLMapSHCoeffs; //!< Packed diffuse (irradiance) IBL map's spherical harmonics coefficients\n" +
 "uniform samplerCube occSpecIBLMap;         //!< Specular IBL map\n" +
 "uniform int         occNbSpecIBLLevels;    //!< Number of mipmap levels used in occSpecIBLMap to store different roughness values maps\n" +
 "\n" +
 "vec3 occDiffIBLMap (in vec3 theNormal); //!< Unpacks spherical harmonics coefficients to diffuse IBL map's values\n" +
 "#endif\n" +
 "\n" +
 "// light type enumeration (same as Graphic3d_TypeOfLightSource)\n" +
 "const int OccLightType_Direct = 1; //!< directional     light source\n" +
 "const int OccLightType_Point  = 2; //!< isotropic point light source\n" +
 "const int OccLightType_Spot   = 3; //!< spot            light source\n" +
 "\n" +
 "// Light sources\n" +
 "uniform               vec4 occLightAmbient;      //!< Cumulative ambient color\n" +
 "#if defined(THE_MAX_LIGHTS) && (THE_MAX_LIGHTS > 0)\n" +
 "#if (THE_MAX_LIGHTS > 1)\n" +
 "  #define occLight_Index(theId) theId\n" +
 "#else\n" +
 "  #define occLight_Index(theId) 0\n" +
 "#endif\n" +
 "uniform THE_PREC_ENUM int  occLightSourcesCount; //!< Total number of light sources\n" +
 "\n" +
 "//! Type of light source, int (see OccLightType enum).\n" +
 "#define occLight_Type(theId)              occLightSourcesTypes[occLight_Index(theId)]\n" +
 "\n" +
 "//! Specular intensity (equals to diffuse), vec3.\n" +
 "#define occLight_Specular(theId)          occLightSources[occLight_Index(theId) * 4 + 0].rgb\n" +
 "\n" +
 "//! Intensity of light source (>= 0), float.\n" +
 "#define occLight_Intensity(theId)         occLightSources[occLight_Index(theId) * 4 + 0].a\n" +
 "\n" +
 "//! Is light a headlight, bool? DEPRECATED method.\n" +
 "#define occLight_IsHeadlight(theId) false\n" +
 "\n" +
 "//! Position of specified light source or direction of directional light source, vec3.\n" +
 "#define occLight_Position(theId)          occLightSources[occLight_Index(theId) * 4 + 1].xyz\n" +
 "\n" +
 "//! Direction of specified spot light source, vec3.\n" +
 "#define occLight_SpotDirection(theId)     occLightSources[occLight_Index(theId) * 4 + 2].xyz\n" +
 "\n" +
 "//! Range on which point light source (positional or spot) can affect (>= 0), float.\n" +
 "#define occLight_Range(theId)             occLightSources[occLight_Index(theId) * 4 + 2].w\n" +
 "\n" +
 "//! Maximum spread angle of the spot light (in radians), float.\n" +
 "#define occLight_SpotCutOff(theId)        occLightSources[occLight_Index(theId) * 4 + 3].z\n" +
 "\n" +
 "//! Attenuation of the spot light intensity (from 0 to 1), float.\n" +
 "#define occLight_SpotExponent(theId)      occLightSources[occLight_Index(theId) * 4 + 3].w\n" +
 "\n" +
 "#if !defined(THE_IS_PBR)\n" +
 "//! Diffuse intensity (equals to Specular), vec3.\n" +
 "#define occLight_Diffuse(theId)           occLightSources[occLight_Index(theId) * 4 + 0].rgb\n" +
 "\n" +
 "//! Const attenuation factor of positional light source, float.\n" +
 "#define occLight_ConstAttenuation(theId)  occLightSources[occLight_Index(theId) * 4 + 3].x\n" +
 "\n" +
 "//! Linear attenuation factor of positional light source, float.\n" +
 "#define occLight_LinearAttenuation(theId) occLightSources[occLight_Index(theId) * 4 + 3].y\n" +
 "#endif\n" +
 "#endif\n" +
 "\n" +
 "#if defined(THE_IS_PBR)\n" +
 "//! Converts roughness value from range [0, 1] to real value for calculations\n" +
 "float occRoughness (in float theNormalizedRoughness);\n" +
 "\n" +
 "// Front/back material properties accessors\n" +
 "vec4  occPBRMaterial_Color(in bool theIsFront);    //!< Base color of PBR material\n" +
 "float occPBRMaterial_Metallic(in bool theIsFront); //!< Metallic coefficient\n" +
 "float occPBRMaterial_NormalizedRoughness(in bool theIsFront); //!< Normalized roughness coefficient\n" +
 "vec3  occPBRMaterial_Emission(in bool theIsFront); //!< Light intensity emitted by material\n" +
 "float occPBRMaterial_IOR(in bool theIsFront);      //!< Index of refraction\n" +
 "#define occMaterial_Emission occPBRMaterial_Emission\n" +
 "#define occMaterial_Color occPBRMaterial_Color\n" +
 "#else\n" +
 "vec4  occMaterial_Diffuse(in bool theIsFront);     //!< Diffuse  reflection\n" +
 "vec3  occMaterial_Specular(in bool theIsFront);    //!< Specular reflection\n" +
 "float occMaterial_Shininess(in bool theIsFront);   //!< Specular exponent\n" +
 "vec3  occMaterial_Ambient(in bool theIsFront);     //!< Ambient  reflection\n" +
 "vec3  occMaterial_Emission(in bool theIsFront);    //!< Emission color\n" +
 "#define occMaterial_Color occMaterial_Diffuse\n" +
 "#endif\n" +
 "\n" +
 "#ifdef THE_HAS_DEFAULT_SAMPLER\n" +
 "#define occActiveSampler    occSampler0  //!< alias for backward compatibility\n" +
 "#define occSamplerBaseColor occSampler0  //!< alias to a base color texture\n" +
 "uniform sampler2D           occSampler0; //!< current active sampler;\n" +
 "#endif                                   //!  occSampler1, occSampler2,... should be defined in GLSL program body for multitexturing\n" +
 "\n" +
 "#if defined(THE_HAS_TEXTURE_COLOR) && defined(FRAGMENT_SHADER)\n" +
 "#define occMaterialBaseColor(theIsFront, theTexCoord) (occMaterial_Color(theIsFront) * occTexture2D(occSamplerBaseColor, theTexCoord))\n" +
 "#else\n" +
 "#define occMaterialBaseColor(theIsFront, theTexCoord) occMaterial_Color(theIsFront)\n" +
 "#endif\n" +
 "\n" +
 "#if defined(THE_HAS_TEXTURE_OCCLUSION) && defined(FRAGMENT_SHADER)\n" +
 "uniform sampler2D occSamplerOcclusion;   //!< R occlusion texture sampler\n" +
 "#define occMaterialOcclusion(theColor, theTexCoord) theColor *= occTexture2D(occSamplerOcclusion, theTexCoord).r;\n" +
 "#else\n" +
 "#define occMaterialOcclusion(theColor, theTexCoord)\n" +
 "#endif\n" +
 "\n" +
 "#if defined(THE_HAS_TEXTURE_EMISSIVE) && defined(FRAGMENT_SHADER)\n" +
 "uniform sampler2D occSamplerEmissive;    //!< RGB emissive texture sampler\n" +
 "#define occMaterialEmission(theIsFront, theTexCoord) (occMaterial_Emission(theIsFront) * occTexture2D(occSamplerEmissive, theTexCoord).rgb)\n" +
 "#else\n" +
 "#define occMaterialEmission(theIsFront, theTexCoord) occMaterial_Emission(theIsFront)\n" +
 "#endif\n" +
 "\n" +
 "#if defined(THE_HAS_TEXTURE_NORMAL) && defined(FRAGMENT_SHADER)\n" +
 "uniform sampler2D occSamplerNormal;      //!< XYZ normal texture sampler with W==0 indicating no texture\n" +
 "#define occTextureNormal(theTexCoord) occTexture2D(occSamplerNormal, theTexCoord)\n" +
 "#else\n" +
 "#define occTextureNormal(theTexCoord) vec4(0.0) // no normal map\n" +
 "#endif\n" +
 "\n" +
 "#if defined(THE_HAS_TEXTURE_METALROUGHNESS) && defined(FRAGMENT_SHADER)\n" +
 "uniform sampler2D occSamplerMetallicRoughness; //!< BG metallic-roughness texture sampler\n" +
 "#define occMaterialRoughness(theIsFront, theTexCoord) (occPBRMaterial_NormalizedRoughness(theIsFront) * occTexture2D(occSamplerMetallicRoughness, theTexCoord).g)\n" +
 "#define occMaterialMetallic(theIsFront,  theTexCoord) (occPBRMaterial_Metallic(theIsFront) * occTexture2D(occSamplerMetallicRoughness, theTexCoord).b)\n" +
 "#else\n" +
 "#define occMaterialRoughness(theIsFront, theTexCoord) occPBRMaterial_NormalizedRoughness(theIsFront)\n" +
 "#define occMaterialMetallic(theIsFront,  theTexCoord) occPBRMaterial_Metallic(theIsFront)\n" +
 "#endif\n" +
 "\n" +
 "uniform               vec4      occColor;              //!< color value (in case of disabled lighting)\n" +
 "uniform THE_PREC_ENUM int       occDistinguishingMode; //!< Are front and back faces distinguished?\n" +
 "uniform THE_PREC_ENUM int       occTextureEnable;      //!< Is texture enabled?\n" +
 "uniform               vec4      occTexTrsf2d[2];       //!< 2D texture transformation parameters\n" +
 "uniform               float     occPointSize;          //!< point size\n" +
 "\n" +
 "//! Parameters of blended order-independent transparency rendering algorithm\n" +
 "uniform               int       occOitOutput;      //!< Enable bit for writing output color buffers for OIT (occFragColor, occFragCoverage)\n" +
 "uniform               float     occOitDepthFactor; //!< Influence of the depth component to the coverage of the accumulated fragment\n" +
 "uniform               float     occAlphaCutoff;    //!< alpha test cutoff value\n" +
 "\n" +
 "//! Parameters of clipping planes\n" +
 "#if defined(THE_MAX_CLIP_PLANES) && (THE_MAX_CLIP_PLANES > 0)\n" +
 "uniform               vec4 occClipPlaneEquations[THE_MAX_CLIP_PLANES];\n" +
 "uniform THE_PREC_ENUM int  occClipPlaneChains[THE_MAX_CLIP_PLANES]; //! Indicating the number of planes in the Chain\n" +
 "uniform THE_PREC_ENUM int  occClipPlaneCount;   //!< Total number of clip planes\n" +
 "#endif\n" +
 "//! @endfile Declarations.glsl\n";





        // This file has been automatically generated from resource file src/Shaders/DeclarationsImpl.glsl

        public const string Shaders_DeclarationsImpl_glsl =
          "\n" +
  "//! @file DeclarationsImpl.glsl includes implementation of common functions and properties accessors\n" +
  "#if defined(FRAGMENT_SHADER)\n" +
  "\n" +
  "#if defined(OCC_DEPTH_PEEL_OIT)\n" +
  "uniform sampler2D occDepthPeelingDepth;\n" +
  "uniform sampler2D occDepthPeelingFrontColor;\n" +
  "int IsFrontPeelLayer = -1;\n" +
  "bool occFragEarlyReturn()\n" +
  "{\n" +
  "  #define THE_DEPTH_CLEAR_VALUE -1e15f\n" +
  "  ivec2  aFragCoord = ivec2 (gl_FragCoord.xy);\n" +
  "  vec2   aLastDepth = texelFetch (occDepthPeelingDepth, aFragCoord, 0).rg;\n" +
  "  occPeelFrontColor = texelFetch (occDepthPeelingFrontColor, aFragCoord, 0);\n" +
  "  occPeelDepth.rg   = vec2 (THE_DEPTH_CLEAR_VALUE); // depth value always increases, so that MAX blend equation can be used\n" +
  "  occPeelBackColor  = vec4 (0.0); // back color is blend after each peeling pass\n" +
  "\n" +
  "  float aNearDepth = -aLastDepth.x;\n" +
  "  float aFarDepth  =  aLastDepth.y;\n" +
  "  float aFragDepth = gl_FragCoord.z; // 0 - 1\n" +
  "  if (aFragDepth < aNearDepth || aFragDepth > aFarDepth)\n" +
  "  {\n" +
  "    return true; // skip peeled depth\n" +
  "  }\n" +
  "  else if (aFragDepth > aNearDepth && aFragDepth < aFarDepth)\n" +
  "  {\n" +
  "    // to be rendered at next peeling pass\n" +
  "    occPeelDepth.rg = vec2 (-aFragDepth, aFragDepth);\n" +
  "    return true;\n" +
  "  }\n" +
  "\n" +
  "  IsFrontPeelLayer = (gl_FragCoord.z == aNearDepth) ? 1 : 0;\n" +
  "  return false;\n" +
  "}\n" +
  "#else\n" +
  "bool occFragEarlyReturn() { return false; }\n" +
  "#endif\n" +
  "\n" +
  "void occSetFragColor (in vec4 theColor)\n" +
  "{\n" +
  "#if defined(OCC_ALPHA_TEST)\n" +
  "  if (theColor.a < occAlphaCutoff) discard;\n" +
  "#endif\n" +
  "#if defined(OCC_WRITE_WEIGHT_OIT_COVERAGE)\n" +
  "  float aWeight     = theColor.a * clamp (1e+2 * pow (1.0 - gl_FragCoord.z * occOitDepthFactor, 3.0), 1e-2, 1e+2);\n" +
  "  occFragCoverage.r = theColor.a * aWeight;\n" +
  "  occFragColor      = vec4 (theColor.rgb * theColor.a * aWeight, theColor.a);\n" +
  "#elif defined(OCC_DEPTH_PEEL_OIT)\n" +
  "  if (IsFrontPeelLayer == 1) // front is blended directly\n" +
  "  {\n" +
  "    vec4 aLastColor = occPeelFrontColor;\n" +
  "    float anAlphaMult = 1.0 - aLastColor.a;\n" +
  "    occPeelFrontColor.rgb = aLastColor.rgb + theColor.rgb * theColor.a * anAlphaMult;\n" +
  "    occPeelFrontColor.a = 1.0 - anAlphaMult * (1.0 - theColor.a);\n" +
  "  }\n" +
  "  else if (IsFrontPeelLayer == 0) // back is blended afterwards\n" +
  "  {\n" +
  "    occPeelBackColor = theColor;\n" +
  "  }\n" +
  "#else\n" +
  "  occFragColor = theColor;\n" +
  "#endif\n" +
  "}\n" +
  "#endif\n" +
  "\n" +
  "#if defined(THE_MAX_LIGHTS) && (THE_MAX_LIGHTS > 0)\n" +
  "// arrays of light sources\n" +
  "uniform               vec4 occLightSources[THE_MAX_LIGHTS * 4]; //!< packed light sources parameters\n" +
  "uniform THE_PREC_ENUM int occLightSourcesTypes[THE_MAX_LIGHTS]; //!< packed light sources types\n" +
  "#endif\n" +
  "\n" +
  "#if defined(THE_IS_PBR)\n" +
  "vec3 occDiffIBLMap (in vec3 theNormal)\n" +
  "{\n" +
  "  vec3 aSHCoeffs[9];\n" +
  "  for (int i = 0; i < 9; ++i)\n" +
  "  {\n" +
  "    aSHCoeffs[i] = occTexture2D (occDiffIBLMapSHCoeffs, vec2 ((float(i) + 0.5) / 9.0, 0.0)).rgb;\n" +
  "  }\n" +
  "  return aSHCoeffs[0]\n" +
  "\n" +
  "       + aSHCoeffs[1] * theNormal.x\n" +
  "	   + aSHCoeffs[2] * theNormal.y\n" +
  "	   + aSHCoeffs[3] * theNormal.z\n" +
  "\n" +
  "	   + aSHCoeffs[4] * theNormal.x * theNormal.z\n" +
  "	   + aSHCoeffs[5] * theNormal.y * theNormal.z\n" +
  "	   + aSHCoeffs[6] * theNormal.x * theNormal.y\n" +
  "\n" +
  "	   + aSHCoeffs[7] * (3.0 * theNormal.z * theNormal.z - 1.0)\n" +
  "	   + aSHCoeffs[8] * (theNormal.x * theNormal.x - theNormal.y * theNormal.y);\n" +
  "}\n" +
  "#endif\n" +
  "\n" +
  "// front and back material properties accessors\n" +
  "#if defined(THE_IS_PBR)\n" +
  "uniform vec4 occPbrMaterial[3 * 2];\n" +
  "\n" +
  "#define MIN_ROUGHNESS 0.01\n" +
  "float occRoughness (in float theNormalizedRoughness) { return theNormalizedRoughness * (1.0 - MIN_ROUGHNESS) + MIN_ROUGHNESS; }\n" +
  "vec4  occPBRMaterial_Color(in bool theIsFront)     { return theIsFront ? occPbrMaterial[0]     : occPbrMaterial[3]; }\n" +
  "vec3  occPBRMaterial_Emission(in bool theIsFront)  { return theIsFront ? occPbrMaterial[1].rgb : occPbrMaterial[4].rgb; }\n" +
  "float occPBRMaterial_IOR(in bool theIsFront)       { return theIsFront ? occPbrMaterial[1].w   : occPbrMaterial[4].w; }\n" +
  "float occPBRMaterial_Metallic(in bool theIsFront)  { return theIsFront ? occPbrMaterial[2].b   : occPbrMaterial[5].b; }\n" +
  "float occPBRMaterial_NormalizedRoughness(in bool theIsFront) { return theIsFront ? occPbrMaterial[2].g : occPbrMaterial[5].g; }\n" +
  "#else\n" +
  "uniform vec4 occCommonMaterial[4 * 2];\n" +
  "\n" +
  "vec4  occMaterial_Diffuse(in bool theIsFront)   { return theIsFront ? occCommonMaterial[0]     : occCommonMaterial[4]; }\n" +
  "vec3  occMaterial_Emission(in bool theIsFront)  { return theIsFront ? occCommonMaterial[1].rgb : occCommonMaterial[5].rgb; }\n" +
  "vec3  occMaterial_Specular(in bool theIsFront)  { return theIsFront ? occCommonMaterial[2].rgb : occCommonMaterial[6].rgb; }\n" +
  "float occMaterial_Shininess(in bool theIsFront) { return theIsFront ? occCommonMaterial[2].a   : occCommonMaterial[6].a; }\n" +
  "vec3  occMaterial_Ambient(in bool theIsFront)   { return theIsFront ? occCommonMaterial[3].rgb : occCommonMaterial[7].rgb; }\n" +
  "#endif\n" +
  "\n" +
  "// 2D texture coordinates transformation\n" +
  "vec2  occTextureTrsf_Translation(void) { return occTexTrsf2d[0].xy; }\n" +
  "vec2  occTextureTrsf_Scale(void)       { return occTexTrsf2d[0].zw; }\n" +
  "float occTextureTrsf_RotationSin(void) { return occTexTrsf2d[1].x; }\n" +
  "float occTextureTrsf_RotationCos(void) { return occTexTrsf2d[1].y; }\n" +
  "//! @endfile DeclarationsImpl.glsl\n";

    }
}