using OCCPort.Common;
using OpenTK.Graphics.OpenGL;
using System;
using System.Reflection.Metadata;
using TKernel;
using TKService;

namespace OCCPort.OpenGL
{
    //! Class holding array of textures to be mapped as a set.
    //! Textures should be defined in ascending order of texture units within the set.
    public class OpenGl_TextureSet
    {
        public OpenGl_TextureSet(OpenGl_Texture theTexture)
        {
            myTextures = new NCollection_Array1<TextureSlot>(0, 0);
            myTextureSetBits = (int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_NONE;

            if (theTexture != null)
            {
                myTextures.ChangeFirst().Texture = theTexture;
                myTextures.ChangeFirst().Unit = theTexture.Sampler().Parameters().TextureUnit();
            }
        }

        public NCollection_Array1<TextureSlot> myTextures;

        //! Texture slot - combination of Texture and binding Unit.
        public class TextureSlot
        {
            public OpenGl_Texture Texture;
            public Graphic3d_TextureUnit Unit;
        }

        //! Class for iterating texture set.
        public class Iterator : NCollection_Array1<TextureSlot>.Iterator
        {

            public Iterator(OpenGl_TextureSet set) : base(set.myTextures)
            {

            }

            //! Access texture unit.
            public Graphic3d_TextureUnit Unit() { return Value().Unit; }
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

        int myTextureSetBits;

        //! Return texture units declared within the program, @sa Graphic3d_TextureSetBits.
        public int ChangeTextureSetBits() { return myTextureSetBits; }
        public void ChangeTextureSetBits(int v) { myTextureSetBits = v; }


        //! Return TRUE if texture array is empty.
        public bool IsEmpty()  { return myTextures.IsEmpty(); }

        //! Return the first texture.
        public OpenGl_Texture First() { return myTextures.First().Texture; }

    }
}