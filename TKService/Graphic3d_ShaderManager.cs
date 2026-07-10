//global using OpenGl_ShaderProgramList = TKernel.NCollection_Sequence<OpenGl_ShaderProgram>;

using OCCPort.Common;

namespace TKService
{
    //! This class is responsible for generation of shader programs.
    public class Graphic3d_ShaderManager
    {     //! Return GAPI version major number.
        public void SetGapiVersion(int theVerMajor,
                              int theVerMinor)
        {
            myGapiVersion.SetValues(theVerMajor, theVerMinor);
        }
        //! Generate map key for light sources configuration.
        //! @param theLights [in] list of light sources
        //! @param theHasShadowMap [in] flag indicating shadow maps usage
        // =======================================================================
        // function : genLightKey
        // purpose  :
        // =======================================================================

        public string genLightKey(Graphic3d_LightSet theLights,
                                                               bool theHasShadowMap)
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

        //! Number specifying maximum number of light sources to prepare a GLSL program with unrolled loop.
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

        //! The same as Shaders_PhongDirectionalLight_glsl but for the light with zero index
        //! (avoids limitations on some mobile devices).
        string THE_FUNC_directionalLightFirst =
         Environment.NewLine + "void directionalLightFirst (in vec3 theNormal,"
 + Environment.NewLine + "                            in vec3 theView,"
   + Environment.NewLine + "                            in bool theIsFront,"
   + Environment.NewLine + "                            in float theShadow)"
   + Environment.NewLine + "{"
   + Environment.NewLine + "  vec3 aLight = occLight_Position (0);"

   + Environment.NewLine + "  vec3 aHalf = normalize (aLight + theView);"

   + Environment.NewLine + "  vec3  aFaceSideNormal = theIsFront ? theNormal : -theNormal;"
   + Environment.NewLine + "  float aNdotL = max (0.0, dot (aFaceSideNormal, aLight));"
   + Environment.NewLine + "  float aNdotH = max (0.0, dot (aFaceSideNormal, aHalf ));"

   + Environment.NewLine + "  float aSpecl = 0.0;"
   + Environment.NewLine + "  if (aNdotL > 0.0)"
   + Environment.NewLine + "  {"
   + Environment.NewLine + "    aSpecl = pow (aNdotH, occMaterial_Shininess(theIsFront));"
   + Environment.NewLine + "  }"

   + Environment.NewLine + "  Diffuse  += occLight_Diffuse(0)  * aNdotL * theShadow;"
   + Environment.NewLine + "  Specular += occLight_Specular(0) * aSpecl * theShadow;"
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

        bool[] myGlslExtensions = new bool[(int)Graphic3d_GlslExtension.Graphic3d_GlslExtension_NB];
        bool myHasFlatShading;      //!< flag indicating flat shading usage
        bool myToReverseDFdxSign;   //!< flag to reverse flat shading normal (workaround)
        bool mySetPointSize;        //!< always set gl_PointSize variable
        bool myUseRedAlpha;         //!< use RED channel instead of ALPHA (e.g. GAPI supports only GL_RED textures and not GL_ALPHA)
        bool myToEmulateDepthClamp; //!< emulate depth clamping in GLSL program
        protected bool mySRgbState;           //!< track sRGB state
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
        //! Set if depth clamping should be emulated by GLSL program.
        public void SetEmulateDepthClamp(bool theToEmulate) { myToEmulateDepthClamp = theToEmulate; }

        //! @return true if detected GL version is greater or equal to requested one.
        public bool IsGapiGreaterEqual(int theVerMajor,
                                  int theVerMinor)
        {
            return (myGapiVersion[0] > theVerMajor)
                || (myGapiVersion[0] == theVerMajor && myGapiVersion[1] >= theVerMinor);
        }
        protected Graphic3d_Vec2i myGapiVersion = new TKernel.NCollection_Vec2<int>();         //!< GAPI version major/minor number pair

        //! Prepare standard GLSL program for FBO blit operation.
        protected Graphic3d_ShaderProgram getStdProgramFboBlit(int theNbSamples,
                                                                                 bool theIsFallback_sRGB)
        {
            ShaderVariableList aUniforms = new ShaderVariableList(), aStageInOuts = new ShaderVariableList();
            aStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec2 TexCoord", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));

            string aSrcVert =
                "void main()"
      + "{"
      + "  TexCoord    = occVertex.zw;"
      + "  gl_Position = vec4(occVertex.x, occVertex.y, 0.0, 1.0);"
      + "}";

            string aSrcFrag;
            if (theNbSamples > 1)
            {
                if (myGapi == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
                {
                    aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("highp sampler2DMS uColorSampler", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                    aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("highp sampler2DMS uDepthSampler", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                }
                else
                {
                    aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("sampler2DMS uColorSampler", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                    aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("sampler2DMS uDepthSampler", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                }

                aSrcFrag =
                "#define THE_NUM_SAMPLES " + theNbSamples
               + Environment.NewLine + (theIsFallback_sRGB ? "#define THE_SHIFT_sRGB" : "")
    + Environment.NewLine + "void main()"
             + Environment.NewLine + "{"
              + Environment.NewLine + "  ivec2 aSize  = textureSize (uColorSampler);"
              + Environment.NewLine + "  ivec2 anUV   = ivec2 (vec2 (aSize) * TexCoord);"
              + Environment.NewLine + "  gl_FragDepth = texelFetch (uDepthSampler, anUV, THE_NUM_SAMPLES / 2 - 1).r;"
              + Environment.NewLine +
      "  vec4 aColor = vec4 (0.0);"
             + Environment.NewLine + "  for (int aSample = 0; aSample < THE_NUM_SAMPLES; ++aSample)"
            + Environment.NewLine + "  {"
            + Environment.NewLine + "    vec4 aVal = texelFetch (uColorSampler, anUV, aSample);"
             + Environment.NewLine + "    aColor += aVal;"
             + Environment.NewLine + "  }"
             + Environment.NewLine + "  aColor /= float(THE_NUM_SAMPLES);"
             + Environment.NewLine + "#ifdef THE_SHIFT_sRGB"
              + Environment.NewLine + "  aColor.rgb = pow (aColor.rgb, vec3 (1.0 / 2.2));"
              + Environment.NewLine + "#endif"
               + Environment.NewLine + "  occSetFragColor (aColor);"
                + Environment.NewLine + "}";
            }
            else
            {
                aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("sampler2D uColorSampler", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("sampler2D uDepthSampler", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                aSrcFrag =
                 (theIsFallback_sRGB ? "#define THE_SHIFT_sRGB" : "")
     + Environment.NewLine + "void main()"
       + Environment.NewLine + "{"
         + Environment.NewLine + "  gl_FragDepth = occTexture2D (uDepthSampler, TexCoord).r;"
         + Environment.NewLine + "  vec4  aColor = occTexture2D (uColorSampler, TexCoord);"
          + Environment.NewLine + "#ifdef THE_SHIFT_sRGB"
           + Environment.NewLine + "  aColor.rgb = pow (aColor.rgb, vec3 (1.0 / 2.2));"
            + Environment.NewLine + "#endif"
            + Environment.NewLine + "  occSetFragColor (aColor);"
              + "}";
            }

            Graphic3d_ShaderProgram aProgramSrc = new Graphic3d_ShaderProgram();
            switch (myGapi)
            {
                case Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGL:
                    {
                        if (IsGapiGreaterEqual(3, 2))
                        {
                            aProgramSrc.SetHeader("#version 150");
                        }
                        break;
                    }
                case Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES:
                    {
                        if (IsGapiGreaterEqual(3, 1))
                        {
                            // required for MSAA sampler
                            aProgramSrc.SetHeader("#version 310 es");
                        }
                        else if (IsGapiGreaterEqual(3, 0))
                        {
                            aProgramSrc.SetHeader("#version 300 es");
                        }
                        else if (myGlslExtensions[(int)Graphic3d_GlslExtension.Graphic3d_GlslExtension_GL_EXT_frag_depth])
                        {
                            aProgramSrc.SetHeader("#extension GL_EXT_frag_depth : enable"
                                                    + "#define gl_FragDepth gl_FragDepthEXT");
                        }
                        else
                        {
                            // there is no way to draw into depth buffer
                            aSrcFrag =
                              "void main()"
                           + Environment.NewLine + "{"
                          + Environment.NewLine + "  occSetFragColor (occTexture2D (uColorSampler, TexCoord));"
                           + Environment.NewLine + "}";
                        }
                        break;
                    }
            }

            string anId = "occt_blit";
            if (theNbSamples > 1)
            {
                anId += ("_msaa") + theNbSamples;
            }
            if (theIsFallback_sRGB)
            {
                anId += "_gamma";
            }
            aProgramSrc.SetId(anId);
            aProgramSrc.SetDefaultSampler(false);
            aProgramSrc.SetNbLightsMax(0);
            aProgramSrc.SetNbShadowMaps(0);
            aProgramSrc.SetNbClipPlanesMax(0);
            aProgramSrc.AttachShader(Graphic3d_ShaderObject.CreateFromSource(aSrcVert, Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX, aUniforms, aStageInOuts));
            aProgramSrc.AttachShader(Graphic3d_ShaderObject.CreateFromSource(aSrcFrag, Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT, aUniforms, aStageInOuts));
            return aProgramSrc;
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

        protected Graphic3d_ShaderProgram getColoredQuadProgram()
        {
            Graphic3d_ShaderProgram aProgSrc = new Graphic3d_ShaderProgram();

            ShaderVariableList aUniforms = new ShaderVariableList();
            ShaderVariableList aStageInOuts = new ShaderVariableList();
            aStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec2 TexCoord", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
            aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("vec3 uColor1", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
            aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("vec3 uColor2", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));

            string aSrcVert = "void main()" +
    "{"
    + "  TexCoord    = occTexCoord.st;"
    + "  gl_Position = occProjectionMatrix * occWorldViewMatrix * occModelWorldMatrix * occVertex;"
    + "}";

            string aSrcFrag = "void main()"
    + "{"
    + "  vec3 c1 = mix (uColor1, uColor2, TexCoord.x);"
    + "  occSetFragColor (vec4 (mix (uColor2, c1, TexCoord.y), 1.0));"
    + "}";

            defaultGlslVersion(aProgSrc, "colored_quad", 0);
            aProgSrc.AttachShader(Graphic3d_ShaderObject.CreateFromSource(aSrcVert, Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX, aUniforms, aStageInOuts));
            aProgSrc.AttachShader(Graphic3d_ShaderObject.CreateFromSource(aSrcFrag, Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT, aUniforms, aStageInOuts));

            return aProgSrc;
        }


        //! Global shader variable for color definition with lighting enabled.
        string THE_FUNC_lightDef =
          Environment.NewLine + "vec3 Ambient;"   //!< Ambient  contribution of light sources
  + Environment.NewLine + "vec3 Diffuse;"   //!< Diffuse  contribution of light sources
  + Environment.NewLine + "vec3 Specular;"; //!< Specular contribution of light sources


        string
  //! Global shader variable for color definition with lighting enabled.
  THE_FUNC_PBR_lightDef =
  Environment.NewLine + "vec3  DirectLighting;" //!< Accumulator of direct lighting from light sources
  + Environment.NewLine + "vec4  BaseColor;"      //!< Base color (albedo) of material for PBR
  + Environment.NewLine + "float Metallic;"       //!< Metallic coefficient of material
  + Environment.NewLine + "float NormalizedRoughness;" //!< Normalized roughness coefficient of material
  + Environment.NewLine + "float Roughness;"      //!< Roughness coefficient of material
  + Environment.NewLine + "vec3  Emission;"       //!< Light intensity emitted by material
  + Environment.NewLine + "float IOR;";           //!< Material's index of refraction
        string THE_VEC2_glPointCoord = "vec2 (gl_PointCoord.x, 1.0 - gl_PointCoord.y)";
        //! Prepare standard GLSL program for accessing point sprite alpha.
        string pointSpriteAlphaSrc(int theBits)
        {
            bool isAlpha = (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_PointSpriteA) == (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_PointSpriteA;
            return isAlpha && myUseRedAlpha ? Environment.NewLine + "float getAlpha(void) { return occTexture2D(occSamplerPointSprite, " + THE_VEC2_glPointCoord + ").r; }"
       : Environment.NewLine + "float getAlpha(void) { return occTexture2D(occSamplerPointSprite, " + THE_VEC2_glPointCoord + ").a; }";
        }


        //! Prepare standard GLSL program with per-pixel lighting.
        //! @param theLights [in] list of light sources
        //! @param theBits   [in] program bits
        //! @param theIsFlatNormal [in] when TRUE, the Vertex normals will be ignored and Face normal will be computed instead
        //! @param theIsPBR  [in] when TRUE, the PBR pipeline will be activated
        //! @param theNbShadowMaps [in] number of shadow maps
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
            ShaderVariableList aUniforms = new ShaderVariableList(), aStageInOuts = new ShaderVariableList();
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
                            Message.SendWarning("Warning: ignoring Normal Map texture in GLSL due to hardware capabilities");
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
            string aLights = stdComputeLighting(ref aNbLights, theLights, !aSrcFragGetVertColor.IsEmpty(),
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
        // This file has been automatically generated from resource file src/Shaders/PBRIllumination.glsl

        const string Shaders_PBRIllumination_glsl =
         "//! Calculates direct illumination using Cook-Torrance BRDF.\n"
+ "vec3 occPBRIllumination (in vec3  theView,\n"
+ "                         in vec3  theLight,\n"
+ "                         in vec3  theNormal,\n"
+ "                         in vec4  theBaseColor,\n"
+ "                         in float theMetallic,\n"
+ "                         in float theRoughness,\n"
+ "                         in float theIOR,\n"
+ "                         in vec3  theLightColor,\n"
+ "                         in float theLightIntensity)\n"
+ "{\n"
+ "  vec3 aHalf = normalize (theView + theLight);\n"
+ "  float aCosVH = max(dot(theView, aHalf), 0.0);\n"
+ "  vec3 aFresnel = occPBRFresnel (theBaseColor.rgb, theMetallic, theIOR, aCosVH);\n"
+ "  vec3 aSpecular = occPBRCookTorrance (theView,\n"
+ "                                       theLight,\n"
+ "                                       theNormal,\n"
+ "                                       theBaseColor.rgb,\n"
+ "                                       theMetallic,\n"
+ "                                       theRoughness,\n"
+ "                                       theIOR);\n"
+ "  vec3 aDiffuse = vec3(1.0) - aFresnel;\n"
+ "  aDiffuse *= 1.0 - theMetallic;\n"
+ "  aDiffuse *= INV_PI;\n"
+ "  aDiffuse *= theBaseColor.rgb;\n"
+ "  aDiffuse = mix (vec3(0.0), aDiffuse, theBaseColor.a);\n"
+ "  return (aDiffuse + aSpecular) * theLightColor * theLightIntensity * max(0.0, dot(theLight, theNormal));\n"
+ "}\n";

        static string Shaders_PBRCookTorrance_glsl =
  "//! Calculates Cook-Torrance BRDF.\n"
 + "vec3 occPBRCookTorrance (in vec3  theView,\n"
 + "                         in vec3  theLight,\n"
 + "                         in vec3  theNormal,\n"
 + "                         in vec3  theBaseColor,\n"
 + "                         in float theMetallic,\n"
 + "                         in float theRoughness,\n"
 + "                         in float theIOR)\n"
 + "{\n"
 + "  vec3 aHalf = normalize (theView + theLight);\n"
 + "  float aCosV = max(dot(theView, theNormal), 0.0);\n"
 + "  float aCosL = max(dot(theLight, theNormal), 0.0);\n"
 + "  float aCosH = max(dot(aHalf, theNormal), 0.0);\n"
 + "  float aCosVH = max(dot(aHalf, theView), 0.0);\n"
 + "  vec3 aCookTorrance = occPBRDistribution (aCosH, theRoughness)\n"
 + "                     * occPBRGeometry     (aCosV, aCosL, theRoughness)\n"
 + "                     * occPBRFresnel      (theBaseColor, theMetallic, theIOR, aCosVH);\n"
 + "  aCookTorrance /= 4.0;\n"
 + "  return aCookTorrance;\n"
 + "}\n";

        static string Shaders_PBRFresnel_glsl =
 "//! Functions to calculate fresnel coefficient and approximate zero fresnel value.\n"
+ "vec3 occPBRFresnel (in vec3  theBaseColor,\n"
+ "                    in float theMetallic,\n"
+ "                    in float theIOR)\n"
+ "{\n"
+ "  theIOR = (1.0 - theIOR) / (1.0 + theIOR);\n"
+ "  theIOR *= theIOR;\n"
+ "  vec3 f0 = vec3(theIOR);\n"
+ "  f0 = mix (f0, theBaseColor.rgb, theMetallic);\n"
+ "  return f0;\n"
+ "}\n"
+ "\n"
+ "vec3 occPBRFresnel (in vec3  theBaseColor,\n"
+ "                    in float theMetallic,\n"
+ "                    in float theIOR,\n"
+ "                    in float theCosVH)\n"
+ "{\n"
+ "  vec3 f0 = occPBRFresnel (theBaseColor, theMetallic, theIOR);\n"
+ "  theCosVH = 1.0 - theCosVH;\n"
+ "  theCosVH *= theCosVH;\n"
+ "  theCosVH *= theCosVH * theCosVH * theCosVH * theCosVH;\n"
+ "  return f0 + (vec3 (1.0) - f0) * theCosVH;\n"
+ "}\n"
+ "\n"
+ "vec3 occPBRFresnel (in vec3  theBaseColor,\n"
+ "                    in float theMetallic,\n"
+ "                    in float theRoughness,\n"
+ "                    in float theIOR,\n"
+ "                    in float theCosV)\n"
+ "{\n"
+ "  vec3 f0 = occPBRFresnel (theBaseColor, theMetallic, theIOR);\n"
+ "  theCosV = 1.0 - theCosV;\n"
+ "  theCosV *= theCosV;\n"
+ "  theCosV *= theCosV * theCosV * theCosV * theCosV;\n"
+ "  return f0 + (max(vec3(1.0 - theRoughness), f0) - f0) * theCosV;\n"
+ "}\n";



        static string Shaders_PBRGeometry_glsl =
          "//! Calculates geometry factor for Cook-Torrance BRDF.\n"
 + "float occPBRGeometry (in float theCosV,\n"
 + "                      in float theCosL,\n"
 + "                      in float theRoughness)\n"
 + "{\n"
 + "  float k = theRoughness + 1.0;\n"
 + "  k *= 0.125 * k;\n"
 + "  float g1 = 1.0;\n"
 + "  g1 /= theCosV * (1.0 - k) + k;\n"
 + "  float g2 = 1.0;\n"
 + "  g2 /= theCosL * (1.0 - k) + k;\n"
 + "  return g1 * g2;\n"
 + "}\n";

        static string Shaders_PBRDistribution_glsl =
  "//! Calculates micro facet normals distribution.\n"
 + "float occPBRDistribution (in float theCosH,\n"
+ "                          in float theRoughness)\n"
 + "{\n"
 + "  float aDistribution = theRoughness * theRoughness;\n"
 + "  aDistribution = aDistribution / (theCosH * theCosH * (aDistribution * aDistribution - 1.0) + 1.0);\n"
 + "  aDistribution = INV_PI * aDistribution * aDistribution;\n"
 + "  return aDistribution;\n"
 + "}\n";

        private string stdComputeLighting(ref int theNbLights,
            Graphic3d_LightSet theLights,
            bool theHasVertColor, bool theIsPBR, bool theHasTexColor, int theNbShadowMaps)
        {
            string aLightsFunc = "", aLightsLoop = "";
            theNbLights = 0;
            if (theLights != null)
            {
                theNbLights = theLights.NbEnabled();
                if (theNbLights <= THE_NB_UNROLLED_LIGHTS_MAX)
                {
                    int anIndex = 0;
                    for (Graphic3d_LightSet.Iterator aLightIter = new Graphic3d_LightSet.Iterator(theLights, IterationFilter.IterationFilter_ExcludeDisabledAndAmbient);
                         aLightIter.More(); aLightIter.Next())
                    {
                        switch (aLightIter.Value().Type())
                        {
                            case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Ambient:
                                {
                                    break; // skip ambient
                                }
                            case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional:
                                {
                                    if (theNbShadowMaps > 0
                                     && aLightIter.Value().ToCastShadows())
                                    {
                                        aLightsLoop = aLightsLoop +
                                          Environment.NewLine + "    occDirectionalLight (" + anIndex + ", theNormal, theView, theIsFront,"
+ Environment.NewLine + "                         occLightShadow (occShadowMapSamplers[" + anIndex + "], " + anIndex + ", theNormal));";
                                    }
                                    else
                                    {
                                        aLightsLoop = aLightsLoop + Environment.NewLine + "    occDirectionalLight (" + anIndex + ", theNormal, theView, theIsFront, 1.0);";
                                    }
                                    ++anIndex;
                                    break;
                                }
                            case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Positional:
                                {
                                    aLightsLoop = aLightsLoop + Environment.NewLine + "    occPointLight (" + anIndex + ", theNormal, theView, aPoint, theIsFront);";
                                    ++anIndex;
                                    break;
                                }
                            case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Spot:
                                {
                                    if (theNbShadowMaps > 0
                                     && aLightIter.Value().ToCastShadows())
                                    {
                                        aLightsLoop = aLightsLoop
                                        + Environment.NewLine + "    occSpotLight (" + anIndex + ", theNormal, theView, aPoint, theIsFront,"
                                        + Environment.NewLine + "                  occLightShadow (occShadowMapSamplers[" + anIndex + "], " + anIndex + ", theNormal));";
                                    }
                                    else
                                    {
                                        aLightsLoop = aLightsLoop + Environment.NewLine + "    occSpotLight (" + anIndex + ", theNormal, theView, aPoint, theIsFront, 1.0);";
                                    }
                                    ++anIndex;
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    theNbLights = roundUpMaxLightSources(theNbLights);
                    bool isFirstInLoop = true;
                    aLightsLoop = aLightsLoop
                    + Environment.NewLine + "    for (int anIndex = 0; anIndex < occLightSourcesCount; ++anIndex)"
                    + Environment.NewLine + "    {"
                    + Environment.NewLine + "      int aType = occLight_Type (anIndex);";
                    if (theLights.NbEnabledLightsOfType(Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional) > 0)
                    {
                        isFirstInLoop = false;
                        aLightsLoop +=
                           Environment.NewLine + "      if (aType == OccLightType_Direct)"
                       + Environment.NewLine + "      {"
                       + Environment.NewLine + "        occDirectionalLight (anIndex, theNormal, theView, theIsFront, 1.0);"
                       + Environment.NewLine + "      }";
                    }
                    if (theLights.NbEnabledLightsOfType(Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Positional) > 0)
                    {
                        if (!isFirstInLoop)
                        {
                            aLightsLoop += Environment.NewLine + "      else ";
                        }
                        isFirstInLoop = false;
                        aLightsLoop +=
                           Environment.NewLine + "      if (aType == OccLightType_Point)"
                        + Environment.NewLine + "      {"
                       + Environment.NewLine + "        occPointLight (anIndex, theNormal, theView, aPoint, theIsFront);"
                       + Environment.NewLine + "      }";
                    }
                    if (theLights.NbEnabledLightsOfType(Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Spot) > 0)
                    {
                        if (!isFirstInLoop)
                        {
                            aLightsLoop += Environment.NewLine + "      else ";
                        }
                        isFirstInLoop = false;
                        aLightsLoop +=
                           Environment.NewLine + "      if (aType == OccLightType_Spot)"
                        + Environment.NewLine + "      {"
                        + Environment.NewLine + "        occSpotLight (anIndex, theNormal, theView, aPoint, theIsFront, 1.0);"
                        + Environment.NewLine + "      }";
                    }
                    aLightsLoop += Environment.NewLine + "    }";
                }

                if (theIsPBR)
                {
                    aLightsFunc += Shaders_PBRDistribution_glsl;
                    aLightsFunc += Shaders_PBRGeometry_glsl;
                    aLightsFunc += Shaders_PBRFresnel_glsl;
                    aLightsFunc += Shaders_PBRCookTorrance_glsl;
                    aLightsFunc += Shaders_PBRIllumination_glsl;
                }

                bool isShadowShaderAdded = false;
                if (theLights.NbEnabledLightsOfType(Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional) == 1
                 && theNbLights == 1
                 && !theIsPBR
                 && theNbShadowMaps == 0)
                {
                    // use the version with hard-coded first index
                    aLightsLoop = Environment.NewLine + "    directionalLightFirst(theNormal, theView, theIsFront, 1.0);";
                    aLightsFunc += THE_FUNC_directionalLightFirst;
                }
                else if (theLights.NbEnabledLightsOfType(Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional) > 0)
                {
                    if (theNbShadowMaps > 0 && !isShadowShaderAdded)
                    {
                        aLightsFunc += ShadersConstants.Shaders_LightShadow_glsl;
                        isShadowShaderAdded = true;
                    }
                    aLightsFunc += theIsPBR ? ShadersConstants.Shaders_PBRDirectionalLight_glsl : ShadersConstants.Shaders_PhongDirectionalLight_glsl;
                }
                if (theLights.NbEnabledLightsOfType(Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Positional) > 0)
                {
                    aLightsFunc += theIsPBR ? ShadersConstants.Shaders_PBRPointLight_glsl : ShadersConstants.Shaders_PhongPointLight_glsl;
                }
                if (theLights.NbEnabledLightsOfType(Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Spot) > 0)
                {
                    if (theNbShadowMaps > 0 && !isShadowShaderAdded)
                    {
                        aLightsFunc += ShadersConstants.Shaders_LightShadow_glsl;
                    }
                    aLightsFunc += theIsPBR ? ShadersConstants.Shaders_PBRSpotLight_glsl : ShadersConstants.Shaders_PhongSpotLight_glsl;
                }
            }

            if (!theIsPBR)
            {
                return THE_FUNC_lightDef
                + ShadersConstants.Shaders_PointLightAttenuation_glsl
                + aLightsFunc
                + Environment.NewLine
                 + Environment.NewLine + "vec4 computeLighting (in vec3 theNormal,"
                 + Environment.NewLine + "                      in vec3 theView,"
                 + Environment.NewLine + "                      in vec4 thePoint,"
                 + Environment.NewLine + "                      in bool theIsFront)"
                 + Environment.NewLine + "{"
                 + Environment.NewLine + "  Ambient  = occLightAmbient.rgb;"
                 + Environment.NewLine + "  Diffuse  = vec3 (0.0);"
                 + Environment.NewLine + "  Specular = vec3 (0.0);"
                 + Environment.NewLine + "  vec3 aPoint = thePoint.xyz / thePoint.w;"
    + aLightsLoop
    + Environment.NewLine + "  vec3 aMatAmbient  = occMaterial_Ambient(theIsFront);"
                 + Environment.NewLine + "  vec4 aMatDiffuse  = occMaterial_Diffuse(theIsFront);"
                 + Environment.NewLine + "  vec3 aMatSpecular = occMaterial_Specular(theIsFront);"
                 + Environment.NewLine + "  vec4 aColor = vec4(Ambient * aMatAmbient + Diffuse * aMatDiffuse.rgb + Specular * aMatSpecular, aMatDiffuse.a);"
    + (theHasVertColor ?
       Environment.NewLine + "  aColor *= getVertColor();" : "")
    + (theHasTexColor ? (
       Environment.NewLine + "#if defined(THE_HAS_TEXTURE_COLOR) && defined(FRAGMENT_SHADER)"
               + Environment.NewLine + "  aColor *= occTexture2D(occSamplerBaseColor, TexCoord.st / TexCoord.w);"
               + Environment.NewLine + "#endif") : "") +
     Environment.NewLine + "  occMaterialOcclusion(aColor.rgb, TexCoord.st / TexCoord.w);"
               + Environment.NewLine + "  vec3 aMatEmission = occMaterialEmission(theIsFront, TexCoord.st / TexCoord.w);"
              + Environment.NewLine + "  aColor.rgb += aMatEmission.rgb;"
             + Environment.NewLine + "  return aColor;"
              + Environment.NewLine + "}";
            }
            else
            {
                return THE_FUNC_PBR_lightDef
                + ShadersConstants.Shaders_PointLightAttenuation_glsl
                + aLightsFunc
                + Environment.NewLine
                 + Environment.NewLine + "vec4 computeLighting (in vec3 theNormal,"
                 + Environment.NewLine + "                      in vec3 theView,"
                 + Environment.NewLine + "                      in vec4 thePoint,"
                 + Environment.NewLine + "                      in bool theIsFront)"
                 + Environment.NewLine + "{"
                 + Environment.NewLine + "  DirectLighting = vec3(0.0);"
                 + Environment.NewLine + "  BaseColor           = occMaterialBaseColor(theIsFront, TexCoord.st / TexCoord.w)" + (theHasVertColor ? " * getVertColor()" : "") + ";"
    + Environment.NewLine + "  Emission            = occMaterialEmission(theIsFront, TexCoord.st / TexCoord.w);"
                 + Environment.NewLine + "  Metallic            = occMaterialMetallic(theIsFront, TexCoord.st / TexCoord.w);"
                 + Environment.NewLine + "  NormalizedRoughness = occMaterialRoughness(theIsFront, TexCoord.st / TexCoord.w);"
                 + Environment.NewLine + "  Roughness = occRoughness (NormalizedRoughness);"
                 + Environment.NewLine + "  IOR       = occPBRMaterial_IOR (theIsFront);"
                 + Environment.NewLine + "  vec3 aPoint = thePoint.xyz / thePoint.w;"
    + aLightsLoop
    + Environment.NewLine + "  vec3 aColor = DirectLighting;"
                 + Environment.NewLine + "  vec3 anIndirectLightingSpec = occPBRFresnel (BaseColor.rgb, Metallic, IOR);"
                 + Environment.NewLine + "  vec2 aCoeff = occTexture2D (occEnvLUT, vec2(abs(dot(theView, theNormal)), NormalizedRoughness)).xy;"
                 + Environment.NewLine + "  anIndirectLightingSpec *= aCoeff.x;"
                 + Environment.NewLine + "  anIndirectLightingSpec += aCoeff.y;"
                 + Environment.NewLine + "  anIndirectLightingSpec *= occTextureCubeLod (occSpecIBLMap, -reflect (theView, theNormal), NormalizedRoughness * float (occNbSpecIBLLevels - 1)).rgb;"
                 + Environment.NewLine + "  vec3 aRefractionCoeff = 1.0 - occPBRFresnel (BaseColor.rgb, Metallic, NormalizedRoughness, IOR, abs(dot(theView, theNormal)));"
                 + Environment.NewLine + "  aRefractionCoeff *= (1.0 - Metallic);"
                 + Environment.NewLine + "  vec3 anIndirectLightingDiff = aRefractionCoeff * BaseColor.rgb * BaseColor.a;"
                 + Environment.NewLine + "  anIndirectLightingDiff *= occDiffIBLMap (theNormal).rgb;"
                 + Environment.NewLine + "  aColor += occLightAmbient.rgb * (anIndirectLightingDiff + anIndirectLightingSpec);"
                 + Environment.NewLine + "  aColor += Emission;"
                 + Environment.NewLine + "  occMaterialOcclusion(aColor, TexCoord.st / TexCoord.w);"
                 + Environment.NewLine + "  return vec4 (aColor, mix(1.0, BaseColor.a, aRefractionCoeff.x));"
                 + Environment.NewLine + "}";
            }
        }

        //! Auxiliary function to transform normal from model to view coordinate system.
        static string THE_FUNC_transformNormal_view =
          "vec3 transformNormal (in vec3 theNormal)" +
  "{" +
  "  vec4 aResult = occWorldViewMatrixInverseTranspose" +
  "               * occModelWorldMatrixInverseTranspose" +
  "               * vec4 (theNormal, 0.0);" +
  "  return normalize (aResult.xyz);" +
  "}";

        public object Graphic3d_ShaderFlags_TextureEnv { get; private set; }

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
            ShaderVariableList aUniforms = new ShaderVariableList(),
                aStageInOuts = new ShaderVariableList();
            if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_IsPoint) != 0)
            {
                if (mySetPointSize)
                {
                    aSrcVertExtraMain += "  gl_PointSize = occPointSize;";
                }
            }
            else
            {
                if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_HasTextures) != 0)
                {
                    aUniforms.Append(new Graphic3d_ShaderObject.ShaderVariable("sampler2D occSamplerBaseColor", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                    aStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec4 TexCoord", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                    if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_HasTextures) == (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_TextureEnv)
                    {
                        aSrcVertExtraFunc = THE_FUNC_transformNormal_view;

                        aSrcVertExtraMain +=
                          "  vec4 aPosition = occWorldViewMatrix * occModelWorldMatrix * occVertex;" +
          "  vec3 aNormal   = transformNormal (occNormal);" +
          "  vec3 aReflect  = reflect (normalize (aPosition.xyz), aNormal);" +
          "  aReflect.z += 1.0;" +
          "  TexCoord = vec4(aReflect.xy * inversesqrt (dot (aReflect, aReflect)) * 0.5 + vec2 (0.5), 0.0, 1.0);";

                        aSrcFragGetColor =
                          "vec4 getColor(void) { return occTexture2D (occSamplerBaseColor, TexCoord.st); }";
                    }
                    else
                    {
                        aProgramSrc.SetTextureSetBits((int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_BaseColor);
                        aSrcVertExtraMain += THE_VARY_TexCoord_Trsf;

                        aSrcFragGetColor =
                          "vec4 getColor(void) { return occTexture2D(occSamplerBaseColor, TexCoord.st / TexCoord.w); }";
                    }
                }
            }
            if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_VertColor) != 0)
            {
                aStageInOuts.Append(new Graphic3d_ShaderObject.ShaderVariable("vec4 VertColor", Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX | Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT));
                aSrcVertExtraMain += "  VertColor = occVertColor;";
                aSrcFragGetColor = "vec4 getColor(void) { return VertColor; }";
            }

            int aNbClipPlanes = 0;

            aSrcVert =
            aSrcVertExtraFunc
          + "void main()"
  + "{"
+ aSrcVertExtraMain
+ THE_VERT_gl_Position
+ aSrcVertEndMain
+ "}";

            string aSrcGeom = prepareGeomMainSrc(aUniforms, aStageInOuts, theBits);
            aSrcFragGetColor += Environment.NewLine + (((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_MeshEdges) != 0)
              ? THE_FRAG_WIREFRAME_COLOR
              : "#define getFinalColor getColor");



            aSrcFrag =
                aSrcFragGetColor
           + Environment.NewLine + aSrcGetAlpha
             + Environment.NewLine + "void main()" +
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
            ShaderVariableList theUnifoms,
            ShaderVariableList theStageInOuts,
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
                        string aVarName = aVarListIter.Name.Token(" ", 2);
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
            int aBits = theBits;
            bool toUseDerivates = theUsesDerivates
                                    || (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_StippleLine) != 0
                                    || (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_HasTextures) == (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_TextureNormal;
            switch (myGapi)
            {
                case Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGL:
                    if (IsGapiGreaterEqual(3, 2))
                    {
                        theProgram.SetHeader("#version 150");
                    }
                    else
                    {
                        // TangentSpaceNormal() function uses mat2x3 type
                        bool toUseMat2x3 = (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_HasTextures) == (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_TextureNormal;
                        // gl_PointCoord has been added since GLSL 1.2
                        bool toUsePointCoord = (theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_PointSprite) != 0;
                        if (toUseMat2x3 || toUsePointCoord)
                        {
                            if (IsGapiGreaterEqual(2, 1))
                            {
                                theProgram.SetHeader("#version 120");
                            }
                        }
                        if ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_StippleLine) != 0
                         || theProgram.IsPBR())
                        {
                            if (IsGapiGreaterEqual(3, 0))
                            {
                                theProgram.SetHeader("#version 130");
                            }
                            else if (myGlslExtensions[(int)Graphic3d_GlslExtension.Graphic3d_GlslExtension_GL_EXT_gpu_shader4])
                            {
                                // GL_EXT_gpu_shader4 defines GLSL type "unsigned int", while core GLSL specs define type "uint"
                                theProgram.SetHeader("#extension GL_EXT_gpu_shader4 : enable\n" +

                                                       "#define uint unsigned int");
                            }
                        }
                    }
                    //(void)toUseDerivates;
                    break;
            }

            // should fit Graphic3d_ShaderFlags_NB
            string aBitsStr = aBits.ToString("X04");
            theProgram.SetId("occt_" + theName + aBitsStr);

            return -1;
        }


    }

    public enum Aspect_GraphicsLibrary
    {
        Aspect_GraphicsLibrary_OpenGL,
        Aspect_GraphicsLibrary_OpenGLES
    }

    public static class ShadersConstants
    {
        // This file has been automatically generated from resource file src/Shaders/PBRSpotLight.glsl

        public const string Shaders_PBRSpotLight_glsl =
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
+ "//! into global variables Diffuse and Specular (Phong shading).\n"
+ "//! @param theId      light source index\n"
+ "//! @param theNormal  surface normal\n"
+ "//! @param theView    view direction\n"
+ "//! @param theIsFront front/back face flag\n"
+ "//! @param theShadow  shadow attenuation\n"
+ "void occDirectionalLight (in int  theId,\n"
+ "                          in vec3 theNormal,\n"
+ "                          in vec3 theView,\n"
+ "                          in bool theIsFront,\n"
+ "                          in float theShadow)\n"
+ "{\n"
+ "  vec3 aLight = occLight_Position (theId);\n"
+ "  vec3 aHalf = normalize (aLight + theView);\n"
+ "\n"
+ "  vec3  aFaceSideNormal = theIsFront ? theNormal : -theNormal;\n"
+ "  float aNdotL = max (0.0, dot (aFaceSideNormal, aLight));\n"
+ "  float aNdotH = max (0.0, dot (aFaceSideNormal, aHalf ));\n"
+ "\n"
+ "  float aSpecl = 0.0;\n"
+ "  if (aNdotL > 0.0)\n"
+ "  {\n"
+ "    aSpecl = pow (aNdotH, occMaterial_Shininess (theIsFront));\n"
+ "  }\n"
+ "\n"
+ "  Diffuse  += occLight_Diffuse  (theId) * aNdotL * theShadow;\n"
+ "  Specular += occLight_Specular (theId) * aSpecl * theShadow;\n"
+ "}\n";

        // This file has been automatically generated from resource file src/Shaders/PBRDirectionalLight.glsl

        public const string Shaders_PBRDirectionalLight_glsl =
          "//! Function computes contribution of directional light source\n"
+ "//! into global variable DirectLighting (PBR shading).\n"
+ "//! @param theId      light source index\n"
+ "//! @param theNormal  surface normal\n"
+ "//! @param theView    view direction\n"
+ "//! @param theIsFront front/back face flag\n"
+ "//! @param theShadow  shadow attenuation\n"
+ "void occDirectionalLight (in int  theId,\n"
+ "                          in vec3 theNormal,\n"
+ "                          in vec3 theView,\n"
+ "                          in bool theIsFront,\n"
+ "                          in float theShadow)\n"
+ "{\n"
+ "  vec3 aLight = occLight_Position (theId);\n"
+ "  theNormal = theIsFront ? theNormal : -theNormal;\n"
+ "  DirectLighting += occPBRIllumination (theView, aLight, theNormal,\n"
+ "                                        BaseColor, Metallic, Roughness, IOR,\n"
+ "                                        occLight_Specular (theId),\n"
+ "                                        occLight_Intensity(theId)) * theShadow;\n"
+ "}\n";


        // This file has been automatically generated from resource file src/Shaders/PBRPointLight.glsl

        public const string Shaders_PBRPointLight_glsl =
          "//! Function computes contribution of isotropic point light source\n"
+ "//! into global variable DirectLighting (PBR shading).\n"
+ "//! @param theId      light source index\n"
+ "//! @param theNormal  surface normal\n"
+ "//! @param theView    view direction\n"
+ "//! @param thePoint   3D position (world space)\n"
+ "//! @param theIsFront front/back face flag\n"
+ "void occPointLight (in int  theId,\n"
+ "                    in vec3 theNormal,\n"
+ "                    in vec3 theView,\n"
+ "                    in vec3 thePoint,\n"
+ "                    in bool theIsFront)\n"
+ "{\n"
+ "  vec3 aLight = occLight_Position (theId) - thePoint;\n"
+ "\n"
+ "  float aDist = length (aLight);\n"
+ "  float aRange = occLight_Range (theId);\n"
+ "  float anAtten = occPointLightAttenuation (aDist, aRange);\n"
+ "  if (anAtten <= 0.0) return;\n"
+ "  aLight /= aDist;\n"
+ "\n"
+ "  theNormal = theIsFront ? theNormal : -theNormal;\n"
+ "  DirectLighting += occPBRIllumination (theView, aLight, theNormal,\n"
+ "                                        BaseColor, Metallic, Roughness, IOR,\n"
+ "                                        occLight_Specular (theId),\n"
+ "                                        occLight_Intensity(theId) * anAtten);\n"
+ "}\n";

        // This file has been automatically generated from resource file src/Shaders/PhongSpotLight.glsl

        public const string Shaders_PhongSpotLight_glsl =
          "//! Function computes contribution of spotlight source\n" +
  "//! into global variables Diffuse and Specular (Phong shading).\n" +
  "//! @param theId      light source index\n" +
  "//! @param theNormal  surface normal\n" +
  "//! @param theView    view direction\n" +
  "//! @param thePoint   3D position (world space)\n" +
  "//! @param theIsFront front/back face flag\n" +
  "//! @param theShadow  the value from shadow map\n" +
  "void occSpotLight (in int  theId,\n" +
  "                   in vec3 theNormal,\n" +
  "                   in vec3 theView,\n" +
  "                   in vec3 thePoint,\n" +
  "                   in bool theIsFront,\n" +
  "                   in float theShadow)\n" +
  "{\n" +
  "  vec3 aLight = occLight_Position (theId) - thePoint;\n" +
  "\n" +
  "  float aDist = length (aLight);\n" +
  "  float aRange = occLight_Range (theId);\n" +
  "  float anAtten = occPointLightAttenuation (aDist, aRange, occLight_LinearAttenuation (theId), occLight_ConstAttenuation (theId));\n" +
  "  if (anAtten <= 0.0) return;\n" +
  "  aLight /= aDist;\n" +
  "\n" +
  "  vec3 aSpotDir = occLight_SpotDirection (theId);\n" +
  "  // light cone\n" +
  "  float aCosA = dot (aSpotDir, -aLight);\n" +
  "  if (aCosA >= 1.0 || aCosA < cos (occLight_SpotCutOff (theId)))\n" +
  "  {\n" +
  "    return;\n" +
  "  }\n" +
  "\n" +
  "  float anExponent = occLight_SpotExponent (theId);\n" +
  "  if (anExponent > 0.0)\n" +
  "  {\n" +
  "    anAtten *= pow (aCosA, anExponent * 128.0);\n" +
  "  }\n" +
  "\n" +
  "  vec3 aHalf = normalize (aLight + theView);\n" +
  "\n" +
  "  vec3  aFaceSideNormal = theIsFront ? theNormal : -theNormal;\n" +
  "  float aNdotL = max (0.0, dot (aFaceSideNormal, aLight));\n" +
  "  float aNdotH = max (0.0, dot (aFaceSideNormal, aHalf ));\n" +
  "\n" +
  "  float aSpecl = 0.0;\n" +
  "  if (aNdotL > 0.0)\n" +
  "  {\n" +
  "    aSpecl = pow (aNdotH, occMaterial_Shininess (theIsFront));\n" +
  "  }\n" +
  "\n" +
  "  Diffuse  += occLight_Diffuse (theId) * aNdotL * anAtten * theShadow;\n" +
  "  Specular += occLight_Specular(theId) * aSpecl * anAtten * theShadow;\n" +
  "}\n";


        // This file has been automatically generated from resource file src/Shaders/PhongPointLight.glsl

        public const string Shaders_PhongPointLight_glsl =
          "//! Function computes contribution of isotropic point light source\n" +
  "//! into global variables Diffuse and Specular (Phong shading).\n" +
  "//! @param theId      light source index\n" +
  "//! @param theNormal  surface normal\n" +
  "//! @param theView    view direction\n" +
  "//! @param thePoint   3D position (world space)\n" +
  "//! @param theIsFront front/back face flag\n" +
  "void occPointLight (in int  theId,\n" +
  "                    in vec3 theNormal,\n" +
  "                    in vec3 theView,\n" +
  "                    in vec3 thePoint,\n" +
  "                    in bool theIsFront)\n" +
  "{\n" +
  "  vec3 aLight = occLight_Position (theId) - thePoint;\n" +
  "\n" +
  "  float aDist = length (aLight);\n" +
  "  float aRange = occLight_Range (theId);\n" +
  "  float anAtten = occPointLightAttenuation (aDist, aRange, occLight_LinearAttenuation (theId), occLight_ConstAttenuation (theId));\n" +
  "  if (anAtten <= 0.0) return;\n" +
  "  aLight /= aDist;\n" +
  "\n" +
  "  vec3 aHalf = normalize (aLight + theView);\n" +
  "\n" +
  "  vec3  aFaceSideNormal = theIsFront ? theNormal : -theNormal;\n" +
  "  float aNdotL = max (0.0, dot (aFaceSideNormal, aLight));\n" +
  "  float aNdotH = max (0.0, dot (aFaceSideNormal, aHalf ));\n" +
  "\n" +
  "  float aSpecl = 0.0;\n" +
  "  if (aNdotL > 0.0)\n" +
  "  {\n" +
  "    aSpecl = pow (aNdotH, occMaterial_Shininess (theIsFront));\n" +
  "  }\n" +
  "\n" +
  "  Diffuse  += occLight_Diffuse (theId) * aNdotL * anAtten;\n" +
  "  Specular += occLight_Specular(theId) * aSpecl * anAtten;\n" +
  "}\n";

        // This file has been automatically generated from resource file src/Shaders/PointLightAttenuation.glsl

        public const string Shaders_PointLightAttenuation_glsl =
          "//! Returns point light source attenuation factor\n" +
  "float occRangedPointLightAttenuation (in float theDistance, in float theRange)\n" +
  "{\n" +
  "  if (theDistance <= theRange)\n" +
  "  {\n" +
  "    float aResult = theDistance / theRange;\n" +
  "    aResult *= aResult;\n" +
  "    aResult *= aResult;\n" +
  "    aResult = 1.0 - aResult;\n" +
  "    aResult = clamp(aResult, 0.0, 1.0);\n" +
  "    aResult /= max(0.0001, theDistance * theDistance);\n" +
  "    return aResult;\n" +
  "  }\n" +
  "  return -1.0;\n" +
  "}\n" +
  "\n" +
  "//! Returns point light source attenuation factor with quadratic attenuation in case of zero range.\n" +
  "float occPointLightAttenuation (in float theDistance, in float theRange)\n" +
  "{\n" +
  "  if (theRange == 0.0)\n" +
  "  {\n" +
  "    return 1.0 / max(0.0001, theDistance * theDistance);\n" +
  "  }\n" +
  "  return occRangedPointLightAttenuation (theDistance, theRange);\n" +
  "}\n" +
  "\n" +
  "//! Returns point light source attenuation factor with linear attenuation in case of zero range.\n" +
  "float occPointLightAttenuation (in float theDistance, in float theRange, in float theLinearAttenuation, in float theConstAttenuation)\n" +
  "{\n" +
  "  if (theRange == 0.0)\n" +
  "  {\n" +
  "    return 1.0 / (theConstAttenuation + theLinearAttenuation * theDistance);\n" +
  "  }\n" +
  "  return occRangedPointLightAttenuation (theDistance, theRange);\n" +
  "}\n";

        // This file has been automatically generated from resource file src/Shaders/LightShadow.glsl

        public const string Shaders_LightShadow_glsl =
          "#if (__VERSION__ >= 120)\n" +
  "//! Coefficients for gathering close samples for antialiasing.\n" +
  "//! Use only with decent OpenGL (array constants cannot be initialized with GLSL 1.1 / GLSL ES 1.1)\n" +
  "const vec2 occPoissonDisk16[16] = vec2[](\n" +
  " vec2(-0.94201624,-0.39906216), vec2( 0.94558609,-0.76890725), vec2(-0.09418410,-0.92938870), vec2( 0.34495938, 0.29387760),\n" +
  " vec2(-0.91588581, 0.45771432), vec2(-0.81544232,-0.87912464), vec2(-0.38277543, 0.27676845), vec2( 0.97484398, 0.75648379),\n" +
  " vec2( 0.44323325,-0.97511554), vec2( 0.53742981,-0.47373420), vec2(-0.26496911,-0.41893023), vec2( 0.79197514, 0.19090188),\n" +
  " vec2(-0.24188840, 0.99706507), vec2(-0.81409955, 0.91437590), vec2( 0.19984126, 0.78641367), vec2( 0.14383161,-0.14100790)\n" +
  ");\n" +
  "#endif\n" +
  "\n" +
  "//! Function computes directional and spot light shadow attenuation (1.0 means no shadow).\n" +
  "float occLightShadow (in sampler2D theShadow,\n" +
  "                      in int  theId,\n" +
  "                      in vec3 theNormal)\n" +
  "{\n" +
  "  vec4 aPosLightSpace = PosLightSpace[occLight_Index(theId)];\n" +
  "  vec3 aLightDir = occLight_Position (theId);\n" +
  "  vec3 aProjCoords = (aPosLightSpace.xyz / aPosLightSpace.w);\n" +
  "#ifdef THE_ZERO_TO_ONE_DEPTH\n" +
  "  aProjCoords.xy = aProjCoords.xy * 0.5 + vec2 (0.5);\n" +
  "#else\n" +
  "  aProjCoords = aProjCoords * 0.5 + vec3 (0.5);\n" +
  "#endif\n" +
  "  float aCurrentDepth = aProjCoords.z;\n" +
  "  if (aProjCoords.x < 0.0 || aProjCoords.x > 1.0\n" +
  "   || aProjCoords.y < 0.0 || aProjCoords.y > 1.0\n" +
  "   || aCurrentDepth > 1.0)\n" +
  "  {\n" +
  "    return 1.0;\n" +
  "  }\n" +
  "\n" +
  "  vec2 aTexelSize = vec2 (occShadowMapSizeBias.x);\n" +
  "  float aBias = max (occShadowMapSizeBias.y * (1.0 - dot (theNormal, aLightDir)), occShadowMapSizeBias.y * 0.1);\n" +
  "#if (__VERSION__ >= 120)\n" +
  "  float aShadow = 0.0;\n" +
  "  for (int aPosIter = 0; aPosIter < 16; ++aPosIter)\n" +
  "  {\n" +
  "    float aClosestDepth = occTexture2D (theShadow, aProjCoords.xy + occPoissonDisk16[aPosIter] * aTexelSize).r;\n" +
  "    aShadow += (aCurrentDepth - aBias) > aClosestDepth ? 1.0 : 0.0;\n" +
  "  }\n" +
  "  return 1.0 - aShadow / 16.0;\n" +
  "#else\n" +
  "  float aClosestDepth = occTexture2D (theShadow, aProjCoords.xy).r;\n" +
  "  float aShadow = (aCurrentDepth - aBias) > aClosestDepth ? 1.0 : 0.0;\n" +
  "  return 1.0 - aShadow;\n" +
  "#endif\n" +
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
