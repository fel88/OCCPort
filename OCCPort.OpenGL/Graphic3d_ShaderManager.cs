using OCCPort.Tester;
using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;

namespace OCCPort.OpenGL
{
	//! This class is responsible for generation of shader programs.
	public class Graphic3d_ShaderManager
	{

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


		// =======================================================================
		// function : prepareStdProgramUnlit
		// purpose  :
		// =======================================================================
		public bool prepareStdProgramUnlit(OpenGl_ShaderProgram theProgram,
																	   int theBits,
																	   bool theIsOutline)
		{
			Graphic3d_ShaderProgram aProgramSrc = getStdProgramUnlit(theBits, theIsOutline);
			string aKey = "";
			if (!Create(aProgramSrc, ref aKey, theProgram))
			{
				theProgram = new OpenGl_ShaderProgram(); // just mark as invalid
				return false;
			}
			return true;
		}
		protected OpenGl_ShaderProgramList myProgramList;        //!< The list of shader programs

		public virtual OpenGl_ShaderProgramList ShaderPrograms() { return myProgramList; }

		protected OpenGl_Context myContext;            //!< OpenGL context

		private bool Create(Graphic3d_ShaderProgram theProxy,
				ref string theShareKey,
				OpenGl_ShaderProgram theProgram)
		{
			theProgram.Nullify();
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
				theProgram.Nullify();
				return false;
			}

			myProgramList.Append(theProgram);
			myContext.ShareResource(theShareKey, theProgram);
			return true;
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

		private void defaultGlslVersion(Graphic3d_ShaderProgram aProgramSrc, string v, int theBits)
		{
			throw new NotImplementedException();
		}
	}
}
