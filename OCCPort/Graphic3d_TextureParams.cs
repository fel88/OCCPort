using System;

namespace OCCPort
{
    public class Graphic3d_TextureParams
    {
        public void SetModulate(bool theToModulate)
        {
            myToModulate = theToModulate;
        }

        public void SetGenMode(Graphic3d_TypeOfTextureMode theMode,
                                            Graphic3d_Vec4 thePlaneS,
                                            Graphic3d_Vec4 thePlaneT)
        {
            myGenMode = theMode;
            myGenPlaneS = thePlaneS;
            myGenPlaneT = thePlaneT;
        }

        Graphic3d_Vec4 myGenPlaneS;       //!< texture coordinates generation plane S
        Graphic3d_Vec4 myGenPlaneT;       //!< texture coordinates generation plane T
        Graphic3d_Vec2 myScale;           //!< texture coordinates scale factor vector; (1,1) by default
        Graphic3d_Vec2 myTranslation;     //!< texture coordinates translation vector;  (0,0) by default
        uint mySamplerRevision; //!< modification counter of parameters related to sampler state
                                //  Graphic3d_TextureUnit myTextureUnit;     //!< default texture unit to bind texture; Graphic3d_TextureUnit_BaseColor by default
                                // Graphic3d_TypeOfTextureFilter myFilter;          //!< texture filter, Graphic3d_TOTF_NEAREST by default
                                //   Graphic3d_LevelOfTextureAnisotropy myAnisoLevel;      //!< level of anisotropy filter, Graphic3d_LOTA_OFF by default
        Graphic3d_TypeOfTextureMode myGenMode;         //!< texture coordinates generation mode, Graphic3d_TOTM_MANUAL by default
        int myBaseLevel;       //!< base texture mipmap level (0 by default)
        int myMaxLevel;        //!< maximum texture mipmap array level (1000 by default)
        float myRotAngle;        //!< texture coordinates rotation angle in degrees, 0 by default
        bool myToModulate;      //!< flag to modulate texture with material color, FALSE by default
        bool myToRepeat;        //!< flag to repeat (true) or wrap (false) texture coordinates out of [0,1] range


    } //! Type of the texture projection.

}