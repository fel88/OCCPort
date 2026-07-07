global using ShaderVariableList = TKernel.NCollection_Sequence<TKService.Graphic3d_ShaderObject.ShaderVariable>;

using OCCPort;
using OCCPort.Enums;
using OCCPort.Common;
using OCCPort.Tester;
using OpenTK.Graphics.Egl;
using System;
using System.Linq;
using System.Reflection.Metadata;
using TKService;

namespace OCCPort.OpenGL
{
    //! This class is responsible for generation of shader programs.
    public class Graphic3d_ShaderManager
    {     //! Return GAPI version major number.
        public void SetGapiVersion(int theVerMajor,
                              int theVerMinor)
        {
            myGapiVersion.SetValues(theVerMajor, theVerMinor);
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



        protected OpenGl_ShaderProgramList myProgramList;        //!< The list of shader programs

        public virtual OpenGl_ShaderProgramList ShaderPrograms() { return myProgramList; }





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

        static string  Shaders_PBRFresnel_glsl =
 "//! Functions to calculate fresnel coefficient and approximate zero fresnel value.\n"
+  "vec3 occPBRFresnel (in vec3  theBaseColor,\n"
+  "                    in float theMetallic,\n"
+  "                    in float theIOR)\n"
+  "{\n"
+  "  theIOR = (1.0 - theIOR) / (1.0 + theIOR);\n"
+  "  theIOR *= theIOR;\n"
+  "  vec3 f0 = vec3(theIOR);\n"
+  "  f0 = mix (f0, theBaseColor.rgb, theMetallic);\n"
+  "  return f0;\n"
+  "}\n"
+  "\n"
+  "vec3 occPBRFresnel (in vec3  theBaseColor,\n"
+  "                    in float theMetallic,\n"
+  "                    in float theIOR,\n"
+  "                    in float theCosVH)\n"
+  "{\n"
+  "  vec3 f0 = occPBRFresnel (theBaseColor, theMetallic, theIOR);\n"
+  "  theCosVH = 1.0 - theCosVH;\n"
+  "  theCosVH *= theCosVH;\n"
+  "  theCosVH *= theCosVH * theCosVH * theCosVH * theCosVH;\n"
+  "  return f0 + (vec3 (1.0) - f0) * theCosVH;\n"
+  "}\n"
+  "\n"
+  "vec3 occPBRFresnel (in vec3  theBaseColor,\n"
+  "                    in float theMetallic,\n"
+  "                    in float theRoughness,\n"
+  "                    in float theIOR,\n"
+  "                    in float theCosV)\n"
+  "{\n"
+  "  vec3 f0 = occPBRFresnel (theBaseColor, theMetallic, theIOR);\n"
+  "  theCosV = 1.0 - theCosV;\n"
+  "  theCosV *= theCosV;\n"
+  "  theCosV *= theCosV * theCosV * theCosV * theCosV;\n"
+  "  return f0 + (max(vec3(1.0 - theRoughness), f0) - f0) * theCosV;\n"
+  "}\n";



        static string  Shaders_PBRGeometry_glsl =
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
+  "                          in float theRoughness)\n"
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
                    aLightsFunc += theIsPBR ? ShadersConstants. Shaders_PBRDirectionalLight_glsl : ShadersConstants.Shaders_PhongDirectionalLight_glsl;
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
                    aLightsFunc += theIsPBR ? ShadersConstants. Shaders_PBRSpotLight_glsl : ShadersConstants.Shaders_PhongSpotLight_glsl;
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
    //  enum
    // {
    //   Graphic3d_TextureUnit_NB = Graphic3d_TextureUnit_15 + 1,
    //  };
    //! GLSL syntax extensions.
    enum Graphic3d_GlslExtension
    {
        Graphic3d_GlslExtension_GL_OES_standard_derivatives, //!< OpenGL ES 2.0 extension GL_OES_standard_derivatives
        Graphic3d_GlslExtension_GL_EXT_shader_texture_lod,   //!< OpenGL ES 2.0 extension GL_EXT_shader_texture_lod
        Graphic3d_GlslExtension_GL_EXT_frag_depth,           //!< OpenGL ES 2.0 extension GL_EXT_frag_depth
        Graphic3d_GlslExtension_GL_EXT_gpu_shader4,          //!< OpenGL 2.0 extension GL_EXT_gpu_shader4

        Graphic3d_GlslExtension_NB = Graphic3d_GlslExtension_GL_EXT_gpu_shader4 + 1
    };

}
