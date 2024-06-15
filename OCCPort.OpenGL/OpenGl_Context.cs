using OpenTK.Graphics.OpenGL;
using System;


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
            throw new NotImplementedException();
        }

        internal Aspect_GraphicsLibrary GraphicsLibrary()
        {
            throw new NotImplementedException();
        }

        internal void SetAllowSampleAlphaToCoverage(object value)
        {
            throw new NotImplementedException();
        }

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

        int[] myViewport = new int[4];     //!< current viewport
        int[] myViewportVirt = new int[4]; //!< virtual viewport
        int myPointSpriteOrig; //!< GL_POINT_SPRITE_COORD_ORIGIN state (

        OpenGl_ResourcesMap mySharedResources; //!< shared resources with unique identification key
        public OpenGl_Caps caps; //!< context options
        internal _core11fwd core11fwd;
    }

    public class _core11fwd
    {
        internal void glDisable(int v)
        {
            GL.Disable((EnableCap)v);
        }

        internal void glEnable(All v)
        {
            GL.Enable((EnableCap)v);
        }
    }
}