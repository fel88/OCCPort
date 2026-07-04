
using OCCPort.Common;
using System.Reflection;

namespace TKService
{
    //! This class describes texture parameters.
    public class Graphic3d_TextureParams
    {

        public Graphic3d_TextureParams()
        {
            //          : myGenPlaneS(0.0f, 0.0f, 0.0f, 0.0f),
            //myGenPlaneT(0.0f, 0.0f, 0.0f, 0.0f),
            myScale = new TKernel.NCollection_Vec2<float> (1.0f, 1.0f);
            //myTranslation(0.0f, 0.0f),
            //mySamplerRevision(0),
            myTextureUnit = Graphic3d_TextureUnit.Graphic3d_TextureUnit_BaseColor;
            myFilter = Graphic3d_TypeOfTextureFilter.Graphic3d_TOTF_NEAREST;
            myAnisoLevel = Graphic3d_LevelOfTextureAnisotropy.Graphic3d_LOTA_OFF;
            myGenMode = Graphic3d_TypeOfTextureMode.Graphic3d_TOTM_MANUAL;
            myBaseLevel = (0);
            myMaxLevel = (1000);
            //myRotAngle(0.0f),
            //myToModulate(Standard_False),
            //myToRepeat(Standard_False)
        }

        //! @return TRUE if the texture repeat is enabled.
        //! Default value is FALSE.
        public bool IsRepeat()  { return myToRepeat; }
  

        //! Return maximum texture mipmap array level; 1000 by default.
        //! Real rendering limit will take into account mipmap generation flags and presence of mipmaps in loaded image.
        public int  MaxLevel() { return myMaxLevel; }

        //! @return base texture mipmap level; 0 by default.
        public int BaseLevel()  { return myBaseLevel; }

        Graphic3d_LevelOfTextureAnisotropy myAnisoLevel;      //!< level of anisotropy filter, Graphic3d_LOTA_OFF by default


        //! @return level of anisontropy texture filter.
        //! Default value is Graphic3d_LOTA_OFF.
        public Graphic3d_LevelOfTextureAnisotropy AnisoFilter() { return myAnisoLevel; }
  
        //! @return texture interpolation filter.
        //! Default value is Graphic3d_TOTF_NEAREST.
        public Graphic3d_TypeOfTextureFilter Filter() { return myFilter; }
        Graphic3d_TypeOfTextureFilter myFilter;          //!< texture filter, Graphic3d_TOTF_NEAREST by default
        //! Return modification counter of parameters related to sampler state.
        public uint SamplerRevision() { return mySamplerRevision; }

        public void SetModulate(bool theToModulate)
        {
            myToModulate = theToModulate;
        }

        //! Default texture unit to be used, default is Graphic3d_TextureUnit_BaseColor.
        public Graphic3d_TextureUnit TextureUnit() { return myTextureUnit; }
        Graphic3d_TextureUnit myTextureUnit;     //!< default texture unit to bind texture; Graphic3d_TextureUnit_BaseColor by default

        public void SetGenMode(Graphic3d_TypeOfTextureMode theMode,
                                            Graphic3d_Vec4 thePlaneS,
                                            Graphic3d_Vec4 thePlaneT)
        {
            myGenMode = theMode;
            myGenPlaneS = thePlaneS;
            myGenPlaneT = thePlaneT;
        }

        public void SetFilter(Graphic3d_TypeOfTextureFilter theFilter)
        {
            if (myFilter != theFilter)
            {
                myFilter = theFilter;
                updateSamplerRevision();
            }
        }

        //! Increment revision.
        void updateSamplerRevision() { ++mySamplerRevision; }

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


    //! Level of anisotropy filter.
    //! Notice that actual quality depends on hardware capabilities!
    public enum Graphic3d_LevelOfTextureAnisotropy
    {
        Graphic3d_LOTA_OFF,
        Graphic3d_LOTA_FAST,
        Graphic3d_LOTA_MIDDLE,
        Graphic3d_LOTA_QUALITY
    };
}



