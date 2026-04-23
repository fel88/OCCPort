using OCCPort;
using OCCPort.Enums;
using OCCPort.Tester;
using OpenTK.Graphics.Egl;
using System;
using System.Linq;
using System.Reflection.Metadata;

namespace OCCPort.OpenGL
{
    //! This class is responsible for generation of shader programs.
    public class Graphic3d_ShaderManager
    {  //! Number specifying maximum number of light sources to prepare a GLSL program with unrolled loop.
        static int THE_NB_UNROLLED_LIGHTS_MAX = 32;
        // This file has been automatically generated from resource file src/Shaders/TangentSpaceNormal.glsl

        static string Shaders_TangentSpaceNormal_glsl =
          "//! Calculates transformation from tangent space and apply it to value from normal map to get normal in object space\n"
 + "vec3 TangentSpaceNormal (in mat2 theDeltaUVMatrix,\n"
+ "                         in mat2x3 theDeltaVectorMatrix,\n"
+ "                         in vec3 theNormalMapValue,\n"
+ "                         in vec3 theNormal,\n"
+ "                         in bool theIsInverse)\n"
+ "{\n"
+ "  theNormalMapValue = normalize(theNormalMapValue * 2.0 - vec3(1.0));\n"
+ "  // Inverse matrix\n"
+ "  theDeltaUVMatrix = mat2 (theDeltaUVMatrix[1][1], -theDeltaUVMatrix[0][1], -theDeltaUVMatrix[1][0], theDeltaUVMatrix[0][0]);\n"
+ "  theDeltaVectorMatrix = theDeltaVectorMatrix * theDeltaUVMatrix;\n"
+ "  // Gram-Schmidt orthogonalization\n"
+ "  theDeltaVectorMatrix[1] = normalize(theDeltaVectorMatrix[1] - dot(theNormal, theDeltaVectorMatrix[1]) * theNormal);\n"
+ "  theDeltaVectorMatrix[0] = cross(theDeltaVectorMatrix[1], theNormal);\n"
+ "  float aDirection = theIsInverse ? -1.0 : 1.0;\n"
+ "  return mat3 (aDirection * theDeltaVectorMatrix[0], aDirection * theDeltaVectorMatrix[1], theNormal) * theNormalMapValue;\n"
 + "}\n";

        //! Compute the size of array storing holding light sources definition.
        static int roundUpMaxLightSources(int theNbLights)
        {
            int aMaxLimit = THE_NB_UNROLLED_LIGHTS_MAX;
            for (; aMaxLimit < theNbLights; aMaxLimit *= 2) { }
            return aMaxLimit;
        }

        //! Process a chain of 2 clipping planes in Fragment Shader (3/4 section).
        string THE_FRAG_CLIP_CHAINS_2 =
        Environment.NewLine + "  vec4 aClipEquation0 = occClipPlaneEquations[0];"
+ Environment.NewLine + "  vec4 aClipEquation1 = occClipPlaneEquations[1];"
+ Environment.NewLine + "  if (dot (aClipEquation0.xyz, PositionWorld.xyz / PositionWorld.w) + aClipEquation0.w < 0.0"
+ Environment.NewLine + "   && dot (aClipEquation1.xyz, PositionWorld.xyz / PositionWorld.w) + aClipEquation1.w < 0.0)"
+ Environment.NewLine + "  {"
+ Environment.NewLine + "    discard;"
+ Environment.NewLine + "  }";

        //! Process 1 clipping plane in Fragment Shader.
        string THE_FRAG_CLIP_PLANES_1 =
          Environment.NewLine + "  vec4 aClipEquation0 = occClipPlaneEquations[0];"
 + Environment.NewLine + "  if (dot (aClipEquation0.xyz, PositionWorld.xyz / PositionWorld.w) + aClipEquation0.w < 0.0)"
  + Environment.NewLine + "  {"
  + Environment.NewLine + "    discard;"
  + Environment.NewLine + "  }";

        //! Process 2 clipping planes in Fragment Shader.
        string THE_FRAG_CLIP_PLANES_2 =
          Environment.NewLine + "  vec4 aClipEquation0 = occClipPlaneEquations[0];"
          + Environment.NewLine + "  vec4 aClipEquation1 = occClipPlaneEquations[1];"
          + Environment.NewLine + "  if (dot (aClipEquation0.xyz, PositionWorld.xyz / PositionWorld.w) + aClipEquation0.w < 0.0"
          + Environment.NewLine + "   || dot (aClipEquation1.xyz, PositionWorld.xyz / PositionWorld.w) + aClipEquation1.w < 0.0)"
          + Environment.NewLine + "  {"
          + Environment.NewLine + "    discard;"
          + Environment.NewLine + "  }";

        //! Process clipping planes in Fragment Shader.
        //! Should be added at the beginning of the main() function.
        string THE_FRAG_CLIP_PLANES_N =
           Environment.NewLine + "  for (int aPlaneIter = 0; aPlaneIter < occClipPlaneCount; ++aPlaneIter)"
  + Environment.NewLine + "  {"
  + Environment.NewLine + "    vec4 aClipEquation = occClipPlaneEquations[aPlaneIter];"
  + Environment.NewLine + "    if (dot (aClipEquation.xyz, PositionWorld.xyz / PositionWorld.w) + aClipEquation.w < 0.0)"
  + Environment.NewLine + "    {"
  + Environment.NewLine + "      discard;"
  + Environment.NewLine + "    }"
  + Environment.NewLine + "  }";

        //! The same function as THE_FUNC_transformNormal but is used in PBR pipeline.
        //! The normals are expected to be in world coordinate system in PBR pipeline.
        string THE_FUNC_transformNormal_world =
            Environment.NewLine + "vec3 transformNormal (in vec3 theNormal)"
  + Environment.NewLine + "{"
  + Environment.NewLine + "  vec4 aResult = occModelWorldMatrixInverseTranspose"
  + Environment.NewLine + "               * vec4 (theNormal, 0.0);"
  + Environment.NewLine + "  return normalize (aResult.xyz);"
  + Environment.NewLine + "}";

        //! Process chains of clipping planes in Fragment Shader.
        string THE_FRAG_CLIP_CHAINS_N =
        Environment.NewLine + "  for (int aPlaneIter = 0; aPlaneIter < occClipPlaneCount;)"
+ Environment.NewLine + "  {"
+ Environment.NewLine + "    vec4 aClipEquation = occClipPlaneEquations[aPlaneIter];"
+ Environment.NewLine + "    if (dot (aClipEquation.xyz, PositionWorld.xyz / PositionWorld.w) + aClipEquation.w < 0.0)"
+ Environment.NewLine + "    {"
+ Environment.NewLine + "      if (occClipPlaneChains[aPlaneIter] == 1)"
+ Environment.NewLine + "      {"
+ Environment.NewLine + "        discard;"
+ Environment.NewLine + "      }"
+ Environment.NewLine + "      aPlaneIter += 1;"
+ Environment.NewLine + "    }"
+ Environment.NewLine + "    else"
+ Environment.NewLine + "    {"
+ Environment.NewLine + "      aPlaneIter += occClipPlaneChains[aPlaneIter];"
+ Environment.NewLine + "    }"
+ Environment.NewLine + "  }";


        //! Compute TexCoord value in Vertex Shader
        string THE_VARY_TexCoord_Trsf =
          Environment.NewLine + "  float aRotSin = occTextureTrsf_RotationSin();"
  + Environment.NewLine + "  float aRotCos = occTextureTrsf_RotationCos();"
  + Environment.NewLine + "  vec2  aTex2   = vec2 (occTexCoord.x * aRotCos - occTexCoord.y * aRotSin,"
  + Environment.NewLine + "                        occTexCoord.x * aRotSin + occTexCoord.y * aRotCos);"
  + Environment.NewLine + "  aTex2 = (aTex2 + occTextureTrsf_Translation()) * occTextureTrsf_Scale();"
  + Environment.NewLine + "  TexCoord = vec4(aTex2, occTexCoord.zw);";


        //! Compute gl_Position vertex shader output.
        string THE_VERT_gl_Position =
         Environment.NewLine + "  gl_Position = occProjectionMatrix * occWorldViewMatrix * occModelWorldMatrix * occVertex;";

        //! Displace gl_Position alongside vertex normal for outline rendering.
        //! This code adds silhouette only for smooth surfaces of closed primitive, and produces visual artifacts on sharp edges.
        string THE_VERT_gl_Position_OUTLINE =
        Environment.NewLine + "  float anOutlineDisp = occOrthoScale > 0.0 ? occOrthoScale : gl_Position.w;"
+ Environment.NewLine + "  vec4  anOutlinePos  = occVertex + vec4 (occNormal * (occSilhouetteThickness * anOutlineDisp), 0.0);"
+ Environment.NewLine + "  gl_Position = occProjectionMatrix * occWorldViewMatrix * occModelWorldMatrix * anOutlinePos;";

        //bool myGlslExtensions[Graphic3d_GlslExtension_NB];
        bool myHasFlatShading;      //!< flag indicating flat shading usage
        bool myToReverseDFdxSign;   //!< flag to reverse flat shading normal (workaround)
        bool mySetPointSize;        //!< always set gl_PointSize variable
        bool myUseRedAlpha;         //!< use RED channel instead of ALPHA (e.g. GAPI supports only GL_RED textures and not GL_ALPHA)
        bool myToEmulateDepthClamp; //!< emulate depth clamping in GLSL program
        bool mySRgbState;           //!< track sRGB state
        Aspect_GraphicsLibrary myGapi;          //!< GAPI name

        public Graphic3d_ShaderManager(Aspect_GraphicsLibrary theGapi)
        {
            myGapi = (theGapi);
            // desktop defines a dedicated API for point size, with gl_PointSize added later to GLSL
            myHasFlatShading = (true);
            myToReverseDFdxSign = (false);
            //mySetPointSize(myGapi == Aspect_GraphicsLibrary_OpenGLES);
            myUseRedAlpha = (false);
            myToEmulateDepthClamp = (true);
            mySRgbState = (true);


            //memset(myGlslExtensions, 0, sizeof(myGlslExtensions));
        }



        //! Modify color for Wireframe presentation.
        const string THE_FRAG_WIREFRAME_COLOR =
        "vec4 getFinalColor(void)" +
"{" +
"  float aDistance = min (min (EdgeDistance[0], EdgeDistance[1]), EdgeDistance[2]);" +
"  bool isHollow = occWireframeColor.a < 0.0;" +
"  float aMixVal = smoothstep (occLineWidth - occLineFeather * 0.5, occLineWidth + occLineFeather * 0.5, aDistance);" +
"  vec4 aMixColor = isHollow" +
"                 ? vec4 (getColor().rgb, 1.0 - aMixVal)" +  // edges only (of interior color)
"                 : mix (occWireframeColor, getColor(), aMixVal);" +// interior + edges
"  return aMixColor;" +
"}";



        protected OpenGl_ShaderProgramList myProgramList;        //!< The list of shader programs

        public virtual OpenGl_ShaderProgramList ShaderPrograms() { return myProgramList; }

        protected OpenGl_Context myContext;            //!< OpenGL context

        protected bool Create(Graphic3d_ShaderProgram theProxy,
                ref string theShareKey,
                OpenGl_ShaderProgram theProgram)
        {
            theProgram = null;
            if (theProxy == null)
            {
                return false;
            }

            theShareKey = theProxy.GetId();
            if (myContext.GetResource<OpenGl_ShaderProgram>(theShareKey, theProgram))
            {
                if (theProgram.Share())
                {
                    myProgramList.Append(theProgram);
                }
                return true;
            }

            theProgram = new OpenGl_ShaderProgram(theProxy);
            if (!theProgram.Initialize(myContext, theProxy.ShaderObjects()))
            {
                theProgram.Release(myContext);
                theShareKey.Clear();
                theProgram = null;
                return false;
            }

            myProgramList.Append(theProgram);
            myContext.ShareResource(theShareKey, theProgram);
            return true;
        }

        // =======================================================================
        // function : pointSpriteShadingSrc
        // purpose  :
        //! Prepare standard GLSL program for computing point sprite shading.
        // =======================================================================
        string pointSpriteShadingSrc(string theBaseColorSrc,
                                                                        int theBits)
        {
            string aSrcFragGetColor = "";
            if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_PointSpriteA) == (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_PointSpriteA)
            {
                aSrcFragGetColor = pointSpriteAlphaSrc(theBits)
                  + Environment.NewLine + "vec4 getColor(void)"
                + Environment.NewLine + "{"
                + Environment.NewLine + "  vec4 aColor = " + theBaseColorSrc + ";"
                + Environment.NewLine + "  aColor.a = getAlpha();"
                + Environment.NewLine + "  if (aColor.a <= 0.1) discard;"
                + Environment.NewLine + "  return aColor;"
                + Environment.NewLine + "}";
            }
            else if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_PointSprite) == (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_PointSprite)
            {
                aSrcFragGetColor = "" +
                  Environment.NewLine + "vec4 getColor(void)"
                + Environment.NewLine + "{"
                + Environment.NewLine + "  vec4 aColor = " + theBaseColorSrc + ";"
                + Environment.NewLine + "  aColor = occTexture2D(occSamplerPointSprite, " + THE_VEC2_glPointCoord + ") * aColor;"
                + Environment.NewLine + "  if (aColor.a <= 0.1) discard;"
                + Environment.NewLine + "  return aColor;"
                + Environment.NewLine + "}";
            }

            return aSrcFragGetColor;
        }

        string THE_VEC2_glPointCoord = "vec2 (gl_PointCoord.x, 1.0 - gl_PointCoord.y)";
        //! Prepare standard GLSL program for accessing point sprite alpha.
        string pointSpriteAlphaSrc(int theBits)
        {
            bool isAlpha = (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_PointSpriteA) == (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_PointSpriteA;
            return isAlpha && myUseRedAlpha ? Environment.NewLine + "float getAlpha(void) { return occTexture2D(occSamplerPointSprite, " + THE_VEC2_glPointCoord + ").r; }"
       : Environment.NewLine + "float getAlpha(void) { return occTexture2D(occSamplerPointSprite, " + THE_VEC2_glPointCoord + ").a; }";
        }

        // =======================================================================
        // function : getStdProgramPhong
        // purpose  :
        // =======================================================================
        public Graphic3d_ShaderProgram getStdProgramPhong(Graphic3d_LightSet theLights,
                                                                              int theBits,
                                                                              bool theIsFlatNormal,
                                                                              bool theIsPBR,
                                                                              int theNbShadowMaps)
        {
            string aPhongCompLight =
              "computeLighting (normalize (Normal), normalize (View), PositionWorld, gl_FrontFacing)";
            bool isFlatNormal = theIsFlatNormal && myHasFlatShading;
            string aDFdxSignReversion = myToReverseDFdxSign ? "-" : "";
            bool toUseTexColor = false;
            if (isFlatNormal != theIsFlatNormal)
            {
                Message.SendWarning("Warning: flat shading requires OpenGL ES 3.0+ or GL_OES_standard_derivatives extension");
            }
            else if (isFlatNormal && myToReverseDFdxSign)
            {
                Message.SendWarning("Warning: applied workaround for GLSL flat shading normal computation using dFdx/dFdy on Adreno");
            }

            Graphic3d_ShaderProgram aProgramSrc = new Graphic3d_ShaderProgram();
            aProgramSrc.SetPBR(theIsPBR); // should be set before defaultGlslVersion()

            string aSrcVert, aSrcVertExtraFunc = "", aSrcVertExtraMain = "";
            string aSrcFrag = "", aSrcFragGetVertColor = "", aSrcFragExtraMain = "";
            string aSrcFragGetColor = Environment.NewLine + "vec4 getColor(void) { return " + aPhongCompLight + "; }";
            Graphic3d_ShaderObject.ShaderVariableList aUniforms = new Graphic3d_ShaderObject.ShaderVariableList(), aStageInOuts = new Graphic3d_ShaderObject.ShaderVariableList();
            if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_IsPoint) != 0)
            {
                if (mySetPointSize)
                {
                    aSrcVertExtraMain += Environment.NewLine + "  gl_PointSize = occPointSize;";
                }

                if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_PointSprite) != 0)
                {
                    aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("sampler2D occSamplerPointSprite", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                    aSrcFragGetColor = pointSpriteShadingSrc(aPhongCompLight, theBits);
                }

                if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_TextureRGB) != 0
                 && (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_VertColor) == 0)
                {
                    aProgramSrc.SetTextureSetBits((int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_BaseColor);
                    aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("sampler2D occSamplerBaseColor", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX));
                    aStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec4 VertColor", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));

                    aSrcVertExtraMain += Environment.NewLine + "  VertColor = occTexture2D (occSamplerBaseColor, occTexCoord.xy);";
                    aSrcFragGetVertColor = Environment.NewLine + "vec4 getVertColor(void) { return VertColor; }";
                }
            }
            else
            {
                if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_TextureRGB) != 0)
                {
                    toUseTexColor = true;
                    aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("sampler2D occSamplerBaseColor", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                    aStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec4 TexCoord", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                    aSrcVertExtraMain += THE_VARY_TexCoord_Trsf;

                    var aTextureBits = (Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_BaseColor | Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_Occlusion | Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_Emissive);
                    if (theIsPBR)
                    {
                        aTextureBits |= Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_MetallicRoughness;
                    }
                    if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_HasTextures) == (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_TextureNormal
                     && !isFlatNormal)
                    {
                        if (myHasFlatShading)
                        {
                            aTextureBits |= Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_Normal;
                        }
                        else
                        {
                            OCCPort. Message.SendWarning("Warning: ignoring Normal Map texture in GLSL due to hardware capabilities");
                        }
                    }
                    aProgramSrc.SetTextureSetBits((int)aTextureBits);
                }
            }

            if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_VertColor) != 0)
            {
                aStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec4 VertColor", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                aSrcVertExtraMain += Environment.NewLine + "  VertColor = occVertColor;";
                aSrcFragGetVertColor = Environment.NewLine + "vec4 getVertColor(void) { return VertColor; }";
            }

            int aNbClipPlanes = 0;
            if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_ClipPlanesN) != 0)
            {
                if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_ClipPlanesN) == (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_ClipPlanesN)
                {
                    aNbClipPlanes = Graphic3d_ShaderProgram.THE_MAX_CLIP_PLANES_DEFAULT;
                    aSrcFragExtraMain += (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_ClipChains) != 0
                                       ? THE_FRAG_CLIP_CHAINS_N
                                       : THE_FRAG_CLIP_PLANES_N;
                }
                else if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_ClipPlanes1) != 0)
                {
                    aNbClipPlanes = 1;
                    aSrcFragExtraMain += THE_FRAG_CLIP_PLANES_1;
                }
                else if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_ClipPlanes2) != 0)
                {
                    aNbClipPlanes = 2;
                    aSrcFragExtraMain += (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_ClipChains) != 0
                                       ? THE_FRAG_CLIP_CHAINS_2
                                       : THE_FRAG_CLIP_PLANES_2;
                }
            }
            if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_OitDepthPeeling) != 0)
            {
                aProgramSrc.SetNbFragmentOutputs(3);
                aProgramSrc.SetOitOutput(Graphic3d_RenderTransparentMethod.Graphic3d_RTM_DEPTH_PEELING_OIT);
            }
            else if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_WriteOit) != 0)
            {
                aProgramSrc.SetNbFragmentOutputs(2);
                aProgramSrc.SetOitOutput(Graphic3d_RenderTransparentMethod.Graphic3d_RTM_BLEND_OIT);
            }

            if (isFlatNormal)
            {
                aSrcFragExtraMain += ""
                  + Environment.NewLine + "  Normal = " + aDFdxSignReversion + "normalize (cross (dFdx (PositionWorld.xyz / PositionWorld.w), dFdy (PositionWorld.xyz / PositionWorld.w)));"
                  + Environment.NewLine + "  if (!gl_FrontFacing) { Normal = -Normal; }";
            }
            else
            {
                aStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec3 vNormal", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                aSrcVertExtraFunc += THE_FUNC_transformNormal_world;
                aSrcVertExtraMain += Environment.NewLine + "  vNormal = transformNormal (occNormal);";
                aSrcFragExtraMain += Environment.NewLine + "  Normal = vNormal;";

                if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_IsPoint) == 0
                 && (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_HasTextures) == (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_TextureNormal
                 && myHasFlatShading)
                {
                    aSrcFrag += Shaders_TangentSpaceNormal_glsl;
                    // apply normal map texture
                    aSrcFragExtraMain +=
                      Environment.NewLine + "#if defined(THE_HAS_TEXTURE_NORMAL)" +
                    Environment.NewLine + "  vec2 aTexCoord = TexCoord.st / TexCoord.w;" +
                    Environment.NewLine + "  vec4 aMapNormalValue = occTextureNormal(aTexCoord);"
                    + Environment.NewLine + "  if (aMapNormalValue.w > 0.5)"
                    + Environment.NewLine + "  {"
                    + Environment.NewLine + "    mat2 aDeltaUVMatrix = mat2 (dFdx(aTexCoord), dFdy(aTexCoord));"
                    + Environment.NewLine + "    mat2x3 aDeltaVectorMatrix = mat2x3 (dFdx (PositionWorld.xyz), dFdy (PositionWorld.xyz));"
                    + Environment.NewLine + "    Normal = TangentSpaceNormal (aDeltaUVMatrix, aDeltaVectorMatrix, aMapNormalValue.xyz, Normal, !gl_FrontFacing);"
                    + Environment.NewLine + "  }"
                    + Environment.NewLine + "#endif";
                }
            }

            aStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec4 PositionWorld", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
            aStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec3 View", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
            if (theNbShadowMaps > 0)
            {
                aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("mat4      occShadowMapMatrices[THE_NB_SHADOWMAPS]", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX));
                aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("sampler2D occShadowMapSamplers[THE_NB_SHADOWMAPS]", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("vec2      occShadowMapSizeBias", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));

                aStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec4 PosLightSpace[THE_NB_SHADOWMAPS]", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                aSrcVertExtraMain +=
                  Environment.NewLine + "  for (int aShadowIter = 0; aShadowIter < THE_NB_SHADOWMAPS; ++aShadowIter)"
                  + Environment.NewLine + "  {"
                          + Environment.NewLine + "    PosLightSpace[aShadowIter] = occShadowMapMatrices[aShadowIter] * PositionWorld;"
                          + Environment.NewLine + "  }";
            }

            aSrcVert = ""
              + aSrcVertExtraFunc
              + Environment.NewLine + "void main()"
        + Environment.NewLine + "{"
        + Environment.NewLine + "  PositionWorld = occModelWorldMatrix * occVertex;"
        + Environment.NewLine + "  if (occProjectionMatrix[3][3] == 1.0)"
        + Environment.NewLine + "  {"
        + Environment.NewLine + "    View = (occWorldViewMatrixInverse * vec4(0.0, 0.0, 1.0, 0.0)).xyz;"
        + Environment.NewLine + "  }"
        + Environment.NewLine + "  else"
        + Environment.NewLine + "  {"
        + Environment.NewLine + "    vec3 anEye = (occWorldViewMatrixInverse * vec4(0.0, 0.0, 0.0, 1.0)).xyz;"
        + Environment.NewLine + "    View = normalize (anEye - PositionWorld.xyz);"
        + Environment.NewLine + "  }"
        + aSrcVertExtraMain
        + THE_VERT_gl_Position
        + Environment.NewLine + "}";

            string aSrcGeom = prepareGeomMainSrc(aUniforms, aStageInOuts, theBits);
            aSrcFragGetColor += (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_MeshEdges) != 0
              ? THE_FRAG_WIREFRAME_COLOR
              : Environment.NewLine + "#define getFinalColor getColor";

            int aNbLights = 0;
            string aLights = stdComputeLighting(aNbLights, theLights, !aSrcFragGetVertColor.IsEmpty(),
                                                                        theIsPBR, toUseTexColor, theNbShadowMaps);
            aSrcFrag += ""
              + Environment.NewLine
              + aSrcFragGetVertColor
              + Environment.NewLine + "vec3  Normal;"
              + aLights
              + aSrcFragGetColor
              + Environment.NewLine +
        Environment.NewLine + "void main()"
        + Environment.NewLine + "{"
        + Environment.NewLine + "  if (occFragEarlyReturn()) { return; }"
        + aSrcFragExtraMain
        + Environment.NewLine + "  occSetFragColor (getFinalColor());"
        + Environment.NewLine + "}";

            string aProgId = (theIsFlatNormal ? "flat-" : "phong-") + (theIsPBR ? "pbr-" : "")
                                                  + genLightKey(theLights, theNbShadowMaps > 0) + "-";
            defaultGlslVersion(aProgramSrc, aProgId, theBits, isFlatNormal);
            aProgramSrc.SetDefaultSampler(false);
            aProgramSrc.SetNbLightsMax(aNbLights);
            aProgramSrc.SetNbShadowMaps(theNbShadowMaps);
            aProgramSrc.SetNbClipPlanesMax(aNbClipPlanes);
            aProgramSrc.SetAlphaTest((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_AlphaTest) != 0);

            int aNbGeomInputVerts = !aSrcGeom.IsEmpty() ? 3 : 0;
            aProgramSrc.AttachShader(Graphic3d_ShaderObject.CreateFromSource(aSrcVert, Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX, aUniforms, aStageInOuts, "", "", aNbGeomInputVerts));
            aProgramSrc.AttachShader(Graphic3d_ShaderObject.CreateFromSource(aSrcGeom, Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY, aUniforms, aStageInOuts, "geomIn", "geomOut", aNbGeomInputVerts));
            aProgramSrc.AttachShader(Graphic3d_ShaderObject.CreateFromSource(aSrcFrag, Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT, aUniforms, aStageInOuts, "", "", aNbGeomInputVerts));
            return aProgramSrc;
        }

        private string stdComputeLighting(int aNbLights, Graphic3d_LightSet theLights, bool v, bool theIsPBR, bool toUseTexColor, int theNbShadowMaps)
        {
            throw new NotImplementedException();
        }

        //! Generate map key for light sources configuration.
        //! @param theLights [in] list of light sources
        //! @param theHasShadowMap [in] flag indicating shadow maps usage
        // =======================================================================
        // function : genLightKey
        // purpose  :
        // =======================================================================
        string genLightKey(Graphic3d_LightSet theLights, bool theHasShadowMap)
        {
            if (theLights.NbEnabled() <= THE_NB_UNROLLED_LIGHTS_MAX)
            {
                return theHasShadowMap
                     ? ("ls_") + theLights.KeyEnabledLong()
                     : ("l_") + theLights.KeyEnabledLong();
            }

            int aMaxLimit = roundUpMaxLightSources(theLights.NbEnabled());
            return ("l_") + theLights.KeyEnabledShort() + aMaxLimit;
        }

        //! Prepare standard GLSL program without lighting.
        //! @param theBits      [in] program bits
        //! @param theIsOutline [in] draw silhouette
        public Graphic3d_ShaderProgram getStdProgramUnlit(int theBits,
                                                                                   bool theIsOutline = false)
        {
            Graphic3d_ShaderProgram aProgramSrc = new Graphic3d_ShaderProgram();
            string aSrcVert = "", aSrcVertExtraMain = "", aSrcVertExtraFunc = "", aSrcGetAlpha = "", aSrcVertEndMain = "";
            string aSrcFrag = "", aSrcFragExtraMain = "";
            string aSrcFragGetColor = "vec4 getColor(void) { return occColor; }";
            string aSrcFragMainGetColor = "  occSetFragColor (getFinalColor());";
            Graphic3d_ShaderObject.ShaderVariableList aUniforms = new Graphic3d_ShaderObject.ShaderVariableList(),
                aStageInOuts = new Graphic3d_ShaderObject.ShaderVariableList();
            if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_IsPoint) != 0)
            {
                if (mySetPointSize)
                {
                    aSrcVertExtraMain += "  gl_PointSize = occPointSize;";
                }
            }



            string aSrcGeom = prepareGeomMainSrc(aUniforms, aStageInOuts, theBits);
            aSrcFragGetColor += (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_MeshEdges) != 0
              ? THE_FRAG_WIREFRAME_COLOR
              : "#define getFinalColor getColor";



            aSrcFrag =
                aSrcFragGetColor
              + aSrcGetAlpha
              + "void main()" +
        "{" +
        "  if (occFragEarlyReturn()) { return; }"
        + aSrcFragExtraMain
        + aSrcFragMainGetColor
        + "}";


            defaultGlslVersion(aProgramSrc, theIsOutline ? "outline" : "unlit", theBits);

            aProgramSrc.SetDefaultSampler(false);
            aProgramSrc.SetNbLightsMax(0);
            aProgramSrc.SetNbShadowMaps(0);
            //aProgramSrc.SetNbClipPlanesMax(aNbClipPlanes);
            aProgramSrc.SetAlphaTest((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_AlphaTest) != 0);
            int aNbGeomInputVerts = !aSrcGeom.IsEmpty() ? 3 : 0;

            aProgramSrc.AttachShader(Graphic3d_ShaderObject.CreateFromSource(aSrcVert, Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX, aUniforms, aStageInOuts, "", "", aNbGeomInputVerts));
            aProgramSrc.AttachShader(Graphic3d_ShaderObject.CreateFromSource(aSrcGeom, Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY, aUniforms, aStageInOuts, "geomIn", "geomOut", aNbGeomInputVerts));
            aProgramSrc.AttachShader(Graphic3d_ShaderObject.CreateFromSource(aSrcFrag, Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT, aUniforms, aStageInOuts, "", "", aNbGeomInputVerts));

            return aProgramSrc;
        }

        private string prepareGeomMainSrc(
            Graphic3d_ShaderObject.ShaderVariableList theUnifoms,
            Graphic3d_ShaderObject.ShaderVariableList theStageInOuts,
            int theBits)
        {

            if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_NeedsGeomShader) == 0)
            {
                return string.Empty;
            }


            string aSrcMainGeom =
              "void main()" +
        "{";

            if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_MeshEdges) != 0)
            {
                theUnifoms.Append(new Graphic3d_ShaderObject.ShaderVariable("vec4 occViewport", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY));
                theUnifoms.Append(new Graphic3d_ShaderObject.ShaderVariable("bool occIsQuadMode", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY));
                theUnifoms.Append(new Graphic3d_ShaderObject.ShaderVariable("float occLineWidth", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY));
                theUnifoms.Append(new Graphic3d_ShaderObject.ShaderVariable("float occLineWidth", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                theUnifoms.Append(new Graphic3d_ShaderObject.ShaderVariable("float occLineFeather", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                theUnifoms.Append(new Graphic3d_ShaderObject.ShaderVariable("vec4 occWireframeColor", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                theStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec3 EdgeDistance", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));

                aSrcMainGeom =
                @"vec3 ViewPortTransform (vec4 theVec)
                {
                  vec3 aWinCoord = theVec.xyz / theVec.w;
                 aWinCoord    = aWinCoord * 0.5 + 0.5;
                  aWinCoord.xy = aWinCoord.xy * occViewport.zw + occViewport.xy;
                  return aWinCoord;
                }"
        + aSrcMainGeom
        + @"  vec3 aSideA = ViewPortTransform (gl_in[2].gl_Position) - ViewPortTransform (gl_in[1].gl_Position);
                  vec3 aSideB = ViewPortTransform (gl_in[2].gl_Position) - ViewPortTransform (gl_in[0].gl_Position);
                  vec3 aSideC = ViewPortTransform (gl_in[1].gl_Position) - ViewPortTransform (gl_in[0].gl_Position);
                  float aQuadArea = abs (aSideB.x * aSideC.y - aSideB.y * aSideC.x);
                  vec3 aLenABC    = vec3 (length (aSideA), length (aSideB), length (aSideC));
                  vec3 aHeightABC = vec3 (aQuadArea) / aLenABC;
                  aHeightABC = max (aHeightABC, vec3 (10.0 * occLineWidth)); // avoid shrunk presentation disappearing at distance
        float aQuadModeHeightC = occIsQuadMode ? occLineWidth + 1.0 : 0.0;";
            }

            for (int aVertIter = 0; aVertIter < 3; ++aVertIter)
            {
                string aVertIndex = (aVertIter).ToString();
                // pass variables from Vertex shader to Fragment shader through Geometry shader
                foreach (var aVarListIter in theStageInOuts)
                {


                    if (aVarListIter.Stages == (int)(Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT))
                    {
                        string aVarName = aVarListIter.Name().Token(" ", 2);
                        if (aVarName.Value(aVarName.Length()) == ']')
                        {
                            // copy the whole array
                            string aVarName2 = aVarName.Token("[", 1);
                            aSrcMainGeom += "  geomOut." + aVarName2 + " = geomIn[" + aVertIndex + "]." + aVarName2 + ";";
                        }
                        else
                        {
                            aSrcMainGeom += "  geomOut." + aVarName + " = geomIn[" + aVertIndex + "]." + aVarName + ";";
                        }
                    }
                }

                if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_MeshEdges) != 0)
                {
                    switch (aVertIter)
                    {
                        case 0: aSrcMainGeom += "  EdgeDistance = vec3 (aHeightABC[0], 0.0, aQuadModeHeightC);"; break;
                        case 1: aSrcMainGeom += "  EdgeDistance = vec3 (0.0, aHeightABC[1], aQuadModeHeightC);"; break;
                        case 2: aSrcMainGeom += "  EdgeDistance = vec3 (0.0, 0.0, aHeightABC[2]);"; break;
                    }
                }
                aSrcMainGeom += "  gl_Position = gl_in[" + aVertIndex
                    + "].gl_Position;" +
                 "  EmitVertex();";
            }

            aSrcMainGeom +=
              "  EndPrimitive();" +
        "}";

            return aSrcMainGeom;

        }

        // =======================================================================
        // function : defaultGlslVersion
        // purpose  :
        // =======================================================================
        protected int defaultGlslVersion(Graphic3d_ShaderProgram theProgram,
                                                 string theName,
                                                 int theBits,
                                                 bool theUsesDerivates = false)
        {
            return -1;
        }


    }
    //  enum
    // {
    //   Graphic3d_TextureUnit_NB = Graphic3d_TextureUnit_15 + 1,
    //  };

}
