
using TKService;

namespace OCCPort.OpenGL
{
    //! Point sprite resource. On modern hardware it will be texture with extra parameters.
    //! On ancient hardware sprites will be drawn using bitmaps.
    public class OpenGl_PointSprite : OpenGl_Texture
    {
        public OpenGl_PointSprite(string theResourceId, Graphic3d_TextureParams theParams = null) : base(theResourceId, theParams)
        {
            myBitmapList = (0);

            //mySampler->Parameters()->SetFilter (Graphic3d_TOTF_NEAREST);
            mySampler.Parameters().SetModulate(false);
            mySampler.Parameters().SetGenMode(Graphic3d_TypeOfTextureMode.Graphic3d_TOTM_SPRITE,
                                              new Graphic3d_Vec4(0.0f, 0.0f, 0.0f, 0.0f),
                                             new Graphic3d_Vec4(0.0f, 0.0f, 0.0f, 0.0f));
        }

        uint myBitmapList; //!< if of display list to draw sprite using glBitmap (for backward compatibility)


    }
    
   
}