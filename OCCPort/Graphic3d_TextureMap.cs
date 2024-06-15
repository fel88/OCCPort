namespace OCCPort
{

    //! This is an abstract class for managing texture applyable on polygons.
    public class Graphic3d_TextureMap : Graphic3d_TextureRoot

    {
        Graphic3d_TextureParams myParams;     //!< associated texture parameters
        string myTexId;      //!< unique identifier of this resource (for sharing graphic resource); should never be modified outside constructor
        Image_PixMap myPixMap;     //!< image pixmap - as one of the ways for defining the texture source
                                   //OSD_Path myPath;       //!< image file path - as one of the ways for defining the texture source
                                   //Standard_Size myRevision;   //!< image revision - for signaling changes in the texture source (e.g. file update, pixmap update)
        Graphic3d_TypeOfTexture myType;       //!< texture type
        bool myIsColorMap; //!< flag indicating color nature of values within the texture
        bool myIsTopDown;  //!< Stores rows's memory layout
        bool myHasMipmaps; //!< Indicates whether mipmaps should be generated or not

        //! Returns whether row's memory layout is top-down.
        public bool IsTopDown() { return myIsTopDown; }
    }
}