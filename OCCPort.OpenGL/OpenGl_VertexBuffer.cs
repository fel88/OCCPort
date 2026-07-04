using OpenTK.Graphics.OpenGL;
using System;
using System.Reflection.Metadata;
using TKService;

namespace OCCPort.OpenGL
{
    //! Vertex Buffer Object - is a general storage object for vertex attributes (position, normal, color).
    //! Notice that you should use OpenGl_IndexBuffer specialization for array of indices.	

    public abstract class OpenGl_VertexBuffer : OpenGl_Buffer
    {
        public override BufferTarget GetTarget()
        {
            return BufferTarget.ArrayBuffer;
            //const int GL_ARRAY_BUFFER = 0x8892;			
        }

        public virtual bool HasColorAttribute()
        {
            return false;

        }

        public virtual bool HasNormalAttribute()
        {
            return false;
        }
        public virtual void BindAllAttributes(OpenGl_Context theGlCtx)
        {

        }

        //! Unbind all vertex attributes. Default implementation does nothing.
        public virtual void UnbindAllAttributes(OpenGl_Context aGlContext)
        {

        }

        internal void BindVertexAttrib(OpenGl_Context theGlCtx, uint theAttribLoc)
        {
            if (!IsValid() || theAttribLoc == -1)
            {
                return;
            }
            Bind(theGlCtx);
            theGlCtx.core20fwd.glEnableVertexAttribArray(theAttribLoc);
            theGlCtx.core20fwd.glVertexAttribPointer(theAttribLoc, myComponentsNb, myDataType, false, 0, myOffset);
        }

        public void UnbindVertexAttrib(OpenGl_Context theGlCtx,
                                              uint theAttribLoc)
        {
            if (!IsValid() || theAttribLoc == (-1))
            {
                return;
            }
            theGlCtx.core20fwd.glDisableVertexAttribArray(theAttribLoc);
            Unbind(theGlCtx);
        }
    }
}