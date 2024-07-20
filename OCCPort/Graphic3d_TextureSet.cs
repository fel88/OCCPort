using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class Graphic3d_TextureSet
    {
        internal Graphic3d_TextureMap First()
        {
            return myTextures[0];
        }

        List<Graphic3d_TextureMap> myTextures;
        //! Return TRUE if texture array is empty.
        public bool IsEmpty() { return myTextures.Count == 0; }
    }
}