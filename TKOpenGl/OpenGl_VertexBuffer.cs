using OCCPort.Common;
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

        public static void bindFixed(OpenGl_Context theCtx,
                                       Graphic3d_TypeOfAttribute theMode,
                                       int theNbComp,
                                       int theDataType,
                                       int theStride,
                                       int theOffset)
        {
            switch (theMode)
            {
                case Graphic3d_TypeOfAttribute.Graphic3d_TOA_POS:
                    {
                        theCtx.core11ffp.glEnableClientState(ArrayCap.VertexArray);
                        theCtx.core11ffp.glVertexPointer(theNbComp, theDataType, theStride, theOffset);
                        return;
                    }
                case Graphic3d_TypeOfAttribute.Graphic3d_TOA_NORM:
                    {
                        theCtx.core11ffp.glEnableClientState(ArrayCap.NormalArray);
                        theCtx.core11ffp.glNormalPointer(theDataType, theStride, theOffset);
                        return;
                    }
                case Graphic3d_TypeOfAttribute.Graphic3d_TOA_UV:
                    {
                        theCtx.core11ffp.glEnableClientState(ArrayCap.TextureCoordArray);
                        theCtx.core11ffp.glTexCoordPointer(theNbComp, theDataType, theStride, theOffset);
                        return;
                    }
                case Graphic3d_TypeOfAttribute.Graphic3d_TOA_COLOR:
                    {
                        theCtx.core11ffp.glEnableClientState(ArrayCap.ColorArray);
                        theCtx.core11ffp.glColorPointer(theNbComp, theDataType, theStride, theOffset);
                        theCtx.core11ffp.glColorMaterial(TriangleFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
                        theCtx.core11fwd.glEnable(EnableCap.ColorMaterial);
                        return;
                    }
                case Graphic3d_TypeOfAttribute.Graphic3d_TOA_CUSTOM:
                    return;
            }
        }

        public static void unbindFixed(OpenGl_Context theCtx,
                                          Graphic3d_TypeOfAttribute theMode)
        {
            switch (theMode)
            {
                case Graphic3d_TypeOfAttribute.Graphic3d_TOA_POS: theCtx.core11ffp.glDisableClientState(ArrayCap.VertexArray); return;
                case Graphic3d_TypeOfAttribute.Graphic3d_TOA_NORM: theCtx.core11ffp.glDisableClientState(ArrayCap.NormalArray); return;
                case Graphic3d_TypeOfAttribute.Graphic3d_TOA_UV: theCtx.core11ffp.glDisableClientState(ArrayCap.TextureCoordArray); return;
                case Graphic3d_TypeOfAttribute.Graphic3d_TOA_COLOR: unbindFixedColor(theCtx); return;
                case Graphic3d_TypeOfAttribute.Graphic3d_TOA_CUSTOM:
                    {
                        return;
                    }
            }
        }
        public static void unbindFixedColor(OpenGl_Context theCtx)
        {
            theCtx.core11ffp.glDisableClientState(ArrayCap.ColorArray);
            theCtx.core11fwd.glDisable(EnableCap.ColorMaterial);

            // invalidate FFP material state after GL_COLOR_MATERIAL has modified it (took values from the vertex color)
            theCtx.ShaderManager().UpdateMaterialState();
        }
        public static void unbindAttribute(OpenGl_Context theCtx,
                                                      Graphic3d_TypeOfAttribute theAttribute)
        {
            if (theCtx.ActiveProgram() == null)
            {
                if (theCtx.core11ffp != null)
                {
                    unbindFixed(theCtx, theAttribute);
                }
                return;
            }

            theCtx.core20fwd.glDisableVertexAttribArray((int)theAttribute);
        }

        public static void bindAttribute(OpenGl_Context theCtx,
                                           Graphic3d_TypeOfAttribute theAttribute,
                                           int theNbComp,
                                           uint theDataType,
                                           int theStride,
                                           int theOffset)
        {
            if (theCtx.ActiveProgram() == null)
            {
                if (theCtx.core11ffp != null)
                {
                    bindFixed(theCtx, theAttribute, theNbComp, (int)theDataType, theStride, theOffset);
                }
                else
                {
                    // OpenGL handles vertex attribute setup independently from active GLSL program,
                    // but OCCT historically requires program to be bound beforehand (this check could be removed in future).
                    Message.SendFail("Error: OpenGl_VertexBuffer::bindAttribute() does nothing without active GLSL program");
                }
                return;
            }

            theCtx.core20fwd.glEnableVertexAttribArray((int)theAttribute);
            theCtx.core20fwd.glVertexAttribPointer((int)theAttribute, theNbComp, (int)theDataType, theDataType != (int)All.Float, theStride, theOffset);
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