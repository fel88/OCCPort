namespace OCCPort
{
    //! Base class for cubemaps.
    //! It is iterator over cubemap sides.

    public class Graphic3d_CubeMap : Graphic3d_TextureMap
    {
        //! Returns whether Z axis is inverted.
        public bool ZIsInverted()
        {
            return myZIsInverted;
        }

        Graphic3d_CubeMapSide myCurrentSide;  //!< Iterator state
        bool myEndIsReached; //!< Indicates whether end of iteration has been reached or hasn't
        bool myZIsInverted;  //!< Indicates whether Z axis is inverted that allows to synchronize vertical flip of cubemap

    }
}