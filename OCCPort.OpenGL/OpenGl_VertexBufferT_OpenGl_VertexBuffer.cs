using OpenTK.Graphics.ES11;
using System;
using System.Reflection.Metadata;

namespace OCCPort.OpenGL
{
    internal class OpenGl_VertexBufferT_OpenGl_VertexBuffer : OpenGl_VertexBuffer
    {


        public OpenGl_VertexBufferT_OpenGl_VertexBuffer(int nbAttributes, object attribs) : base()
        {
            NbAttributes = nbAttributes;
        }

        int NbAttributes;
        public override void BindAllAttributes(OpenGl_Context theGlCtx)
        {
            if (!IsValid())
            {
                return;
            }

            Bind(theGlCtx);
            int aNbComp;
            //const GLubyte* anOffset = myOffset;
            //int aMuliplier = Stride != 0 ? 1 : myElemsNb;
            //for (int anAttribIter = 0; anAttribIter < NbAttributes; ++anAttribIter)
            //{
            //    Graphic3d_Attribute anAttrib = Attribs[anAttribIter];
            //    int aDataType = toGlDataType(anAttrib.DataType, aNbComp);
            //    if (aDataType != All.None)
            //    {
            //        bindAttribute(theGlCtx, anAttrib.Id, aNbComp, aDataType, Stride, anOffset);
            //    }
            //    anOffset += aMuliplier * Graphic3d_Attribute::Stride(anAttrib.DataType);
            //}
        }
        public override void UnbindAllAttributes(OpenGl_Context theGlCtx)
        {
            if (!IsValid())
            {
                return;
            }
            Unbind(theGlCtx);

            for (int anAttribIter = 0; anAttribIter < NbAttributes; ++anAttribIter)
            {
                //Graphic3d_Attribute anAttrib = Attribs[anAttribIter];
               // base.unbindAttribute(theGlCtx, anAttrib.Id);
            }
        }
    }
}