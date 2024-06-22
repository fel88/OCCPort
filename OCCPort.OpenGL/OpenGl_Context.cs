using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using static System.Net.Mime.MediaTypeNames;


namespace OCCPort.OpenGL
{
	public class OpenGl_Context
	{
		public OpenGl_ArbFBO arbFBO;             //!< GL_ARB_framebuffer_object
		public OpenGl_ArbFBOBlit arbFBOBlit;         //!< glBlitFramebuffer function, moved out from OpenGl_ArbFBO structure for compatibility with OpenGL ES 2.0
		bool arbSampleShading;   //!< GL_ARB_sample_shading
		bool arbDepthClamp;      //!< GL_ARB_depth_clamp (on desktop 

		//! @name public properties tracking current state

		public OpenGl_MatrixState<float> ModelWorldState; //!< state of orientation matrix
		public OpenGl_MatrixState<float> WorldViewState;  //!< state of orientation matrix
		public OpenGl_MatrixState<float> ProjectionState; //!< state of projection  matrix

		internal bool GetResource<T>(string theShareKey, T theProgram)
		{
			throw new NotImplementedException();
		}

		internal bool ShareResource(string theKey,
			OpenGl_ShaderProgram theResource)
		{
			if (theKey.IsEmpty() || theResource == null)
			{
				return false;
			}
			return mySharedResources.Bind(theKey, theResource);
		}
		int myPolygonMode;
		internal int SetPolygonMode(int theMode)
		{
			if (myPolygonMode == theMode)
			{
				return myPolygonMode;
			}

			int anOldPolygonMode = myPolygonMode;
			myPolygonMode = theMode;
			//if (myGapi != Aspect_GraphicsLibrary_OpenGLES)
			{
				GL.PolygonMode(MaterialFace.FrontAndBack, (PolygonMode)theMode);
				//core11fwd->glPolygonMode(GL_FRONT_AND_BACK, (GLenum)theMode);
			}
			return anOldPolygonMode;

		}

		internal bool ToUseVbo()
		{
			return core15fwd != null
			   && !caps.vboDisable;
		}

		internal Aspect_GraphicsLibrary GraphicsLibrary()
		{
			throw new NotImplementedException();
		}

		//! Allow GL_SAMPLE_ALPHA_TO_COVERAGE usage.
		public void SetAllowSampleAlphaToCoverage(bool theToEnable) { myAllowAlphaToCov = theToEnable; }
		bool myAllowAlphaToCov; //!< flag allowing   GL_SAMPLE_ALPHA_TO_COVERAGE usage

		internal Graphic3d_Camera Camera()
		{
			throw new NotImplementedException();
		}

		internal void ApplyProjectionMatrix()
		{
			throw new NotImplementedException();
		}

		internal void ApplyModelViewMatrix()
		{
			throw new NotImplementedException();
		}

		//! Return cached viewport definition (x, y, width, height).
		public int[] Viewport() { return myViewport; }

		internal bool IsFeedback()
		{
			throw new NotImplementedException();
		}

		internal void SetFrameBufferSRGB(bool v)
		{
			throw new NotImplementedException();
		}

		internal object Clipping()
		{
			throw new NotImplementedException();
		}

		OpenGl_ShaderManager myShaderManager; //! support object for managing shader programs

		internal OpenGl_ShaderManager ShaderManager()
		{
			return myShaderManager;
		}

		internal bool ColorMask()
		{
			throw new NotImplementedException();
		}

		internal static bool CheckIsTransparent(OpenGl_Aspects theAspect,
			Graphic3d_PresentationAttributes theHighlight)
		{

			float anAlphaFront = 1.0f, anAlphaBack = 1.0f;
			return CheckIsTransparent(theAspect, theHighlight, anAlphaFront, anAlphaBack);
		}

		private static bool CheckIsTransparent(OpenGl_Aspects theAspect,
			Graphic3d_PresentationAttributes theHighlight,
			float theAlphaFront, float theAlphaBack)
		{
			Graphic3d_Aspects anAspect = (theHighlight != null && theHighlight.BasicFillAreaAspect() != null)
										 ? (Graphic3d_Aspects)theHighlight.BasicFillAreaAspect()
										   : theAspect.Aspect();

			bool toDistinguish = anAspect.Distinguish();
			Graphic3d_MaterialAspect aMatFrontSrc = anAspect.FrontMaterial();
			Graphic3d_MaterialAspect aMatBackSrc = toDistinguish
														? anAspect.BackMaterial()
														: aMatFrontSrc;

			// handling transparency
			if (theHighlight != null
			  && theHighlight.BasicFillAreaAspect() == null)
			{
				theAlphaFront = theHighlight.ColorRGBA().Alpha();
				theAlphaBack = theHighlight.ColorRGBA().Alpha();
			}
			else
			{
				theAlphaFront = aMatFrontSrc.Alpha();
				theAlphaBack = aMatBackSrc.Alpha();
			}

			if (anAspect.AlphaMode() == Graphic3d_AlphaMode.Graphic3d_AlphaMode_BlendAuto)
			{
				return theAlphaFront < 1.0f
					|| theAlphaBack < 1.0f;
			}
			// Graphic3d_AlphaMode_Mask and Graphic3d_AlphaMode_MaskBlend are not considered transparent here
			return anAspect.AlphaMode() == Graphic3d_AlphaMode.Graphic3d_AlphaMode_Blend;

		}

		internal bool IsCurrent()
		{
			throw new NotImplementedException();
		}

		int[] myViewport = new int[4];     //!< current viewport
		int[] myViewportVirt = new int[4]; //!< virtual viewport
		int myPointSpriteOrig; //!< GL_POINT_SPRITE_COORD_ORIGIN state (

		OpenGl_ResourcesMap mySharedResources; //!< shared resources with unique identification key
		public OpenGl_Caps caps; //!< context options
		internal _core11fwd core11fwd;
		internal _core15fwd core15fwd;


		//! Empty constructor. You should call Init() to perform initialization with bound GL context.

		public OpenGl_Context(OpenGl_Caps theCaps = null)
		{
			//core11ffp = null;

			core11fwd = new _core11fwd();
			core15fwd = new _core15fwd();

			caps = (theCaps != null ? theCaps : new OpenGl_Caps());

		}
	}
}