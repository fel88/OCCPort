using OCCPort.Common;
using OpenTK.Graphics.ES11;
using System;
using System.Linq;
using System.Reflection.Metadata;
using TKService;

namespace OCCPort.OpenGL
{
    public class OpenGl_VertexBufferT_OpenGl_VertexBuffer : OpenGl_VertexBuffer
    {

       public OpenGl_VertexBufferT_OpenGl_VertexBuffer()
        {

        }
        public Graphic3d_Attribute[] Attribs;
        public int Stride;

        
        public OpenGl_VertexBufferT_OpenGl_VertexBuffer(int nbAttributes, Graphic3d_Buffer theAttribs ) : base()
        {
            Stride = (theAttribs.IsInterleaved() ? theAttribs.Stride : 0);
            NbAttributes = nbAttributes;
            Attribs=new Graphic3d_Attribute[NbAttributes];
            //memcpy(Attribs, theAttribs.AttributesArray(), sizeof(Graphic3d_Attribute) * NbAttributes);
            Attribs = theAttribs.AttributesArray().ToArray ();

        }

        public override bool HasColorAttribute()
        {
            for (int anAttribIter = 0; anAttribIter < NbAttributes; ++anAttribIter)
            {
                Graphic3d_Attribute anAttrib = Attribs[anAttribIter];
                if (anAttrib.Id ==Graphic3d_TypeOfAttribute. Graphic3d_TOA_COLOR)
                {
                    return true;
                }
            }
            return false;
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