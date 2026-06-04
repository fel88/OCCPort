namespace TKService
{
    //! Class holding array of textures to be mapped as a set.
    //! Textures should be defined in ascending order of texture units within the set.
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
