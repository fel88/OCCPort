using System;
using TKernel;
using TKService;

namespace OCCPort.OpenGL
{
    public class OpenGl_TextureSet
    {

        //! Texture slot - combination of Texture and binding Unit.
        public struct TextureSlot
        {
         public    OpenGl_Texture Texture;
            public Graphic3d_TextureUnit Unit;
        }

        //! Class for iterating texture set.
        public class Iterator : NCollection_Array1<TextureSlot>.Iterator
        {//! Access texture unit.
         public   Graphic3d_TextureUnit Unit()  { return Value().Unit; }

           
        }
        internal bool HasNonPointSprite()
        {
            throw new NotImplementedException();
        }

        internal bool IsModulate()
        {
            throw new NotImplementedException();
        }

        internal int TextureSetBits()
        {
            throw new NotImplementedException();
        }
    }
}