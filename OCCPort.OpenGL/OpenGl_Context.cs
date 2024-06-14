using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort
{
    public class OpenGl_Context
    {
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

        OpenGl_ResourcesMap mySharedResources; //!< shared resources with unique identification key
    }
}