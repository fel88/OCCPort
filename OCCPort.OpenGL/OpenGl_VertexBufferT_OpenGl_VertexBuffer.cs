using OCCPort.Common;
using OpenTK.Graphics.ES11;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
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
        public override bool HasNormalAttribute()
        {
            for (int anAttribIter = 0; anAttribIter < NbAttributes; ++anAttribIter)
            {
                Graphic3d_Attribute anAttrib = Attribs[anAttribIter];
                if (anAttrib.Id == Graphic3d_TypeOfAttribute.Graphic3d_TOA_NORM)
                    return true;
            }
            return false;
        }

        public OpenGl_VertexBufferT_OpenGl_VertexBuffer(int nbAttributes, Graphic3d_Buffer theAttribs) : base()
        {
            Stride = (theAttribs.IsInterleaved() ? theAttribs.Stride : 0);
            NbAttributes = nbAttributes;
            Attribs = new Graphic3d_Attribute[NbAttributes];
            //memcpy(Attribs, theAttribs.AttributesArray(), sizeof(Graphic3d_Attribute) * NbAttributes);
            Attribs = theAttribs.AttributesArray().ToArray();

        }

        public override bool HasColorAttribute()
        {
            for (int anAttribIter = 0; anAttribIter < NbAttributes; ++anAttribIter)
            {
                Graphic3d_Attribute anAttrib = Attribs[anAttribIter];
                if (anAttrib.Id == Graphic3d_TypeOfAttribute.Graphic3d_TOA_COLOR)
                {
                    return true;
                }
            }
            return false;
        }

        //! Convert data type to GL info
        int toGlDataType(Graphic3d_TypeOfData theType,
                                   ref int theNbComp)
        {
            switch (theType)
            {
                case Graphic3d_TypeOfData.Graphic3d_TOD_USHORT:
                    theNbComp = 1;
                    return (int)All.UnsignedShort;
                case Graphic3d_TypeOfData.Graphic3d_TOD_UINT:
                    theNbComp = 1;
                    return (int)All.UnsignedInt;
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC2:
                    theNbComp = 2;
                    return (int)All.Float;
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC3:
                    theNbComp = 3;
                    return (int)All.Float;
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC4:
                    theNbComp = 4;
                    return (int)All.Float;
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC4UB:
                    theNbComp = 4;
                    return (int)All.UnsignedByte;
                case Graphic3d_TypeOfData.Graphic3d_TOD_FLOAT:
                    theNbComp = 1;
                    return (int)All.Float;
            }
            theNbComp = 0;
            return (int)All.None;
        }

        int NbAttributes;

        public override void BindAllAttributes(OpenGl_Context theGlCtx)
        {
            if (!IsValid())
                return;

            Bind(theGlCtx);
            GLint aNbComp = 0;
            int anOffset = myOffset;
            int aMuliplier = Stride != 0 ? 1 : myElemsNb;
            for (int anAttribIter = 0; anAttribIter < NbAttributes; ++anAttribIter)
            {
                Graphic3d_Attribute anAttrib = Attribs[anAttribIter];
                var aDataType = toGlDataType(anAttrib.DataType, ref aNbComp);
                if (aDataType != (int)All.None)
                {
                    OpenGl_VertexBuffer.bindAttribute(theGlCtx, anAttrib.Id, aNbComp, (uint)aDataType, Stride, anOffset);
                }
                anOffset += aMuliplier * Graphic3d_Attribute.Stride(anAttrib.DataType);
            }
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
                Graphic3d_Attribute anAttrib = Attribs[anAttribIter];
                 OpenGl_VertexBuffer.unbindAttribute(theGlCtx, anAttrib.Id);
            }
        }
    }
}