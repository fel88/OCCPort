using OCCPort.Common;
using OCCPort.OpenGL;
using OpenTK.Graphics.ES11;
using System;
using System.Reflection.Metadata;
using TKService;

namespace OCCPort
{
    //! Class implements OpenGL sampler object resource that
    //! stores the sampling parameters for a texture access.
    public class OpenGl_Sampler : OpenGl_Resource
    {
        //! Returns true if current object was initialized.
        public bool IsValid() 
        {
    return isValidSampler();
  }

        //! Sets specific sampler parameter.
        public static void setParameter(OpenGl_Context theContext,
                                            OpenGl_Sampler theSampler,
                                            uint theTarget,
                                            uint theParam,
                                            int theValue)
        {

        }

        //! Apply sampler parameters.
        //! @param theCtx     [in] active OpenGL context
        //! @param theParams  [in] texture parameters to apply
        //! @param theSampler [in] apply parameters to Texture object (NULL)
        //!                        or to specified Sampler object (non-NULL, sampler is not required to be bound)
        //! @param theTarget  [in] OpenGL texture target
        //! @param theMaxMipLevel [in] maximum mipmap level defined within the texture
        public static void applySamplerParams(OpenGl_Context theCtx,
                                                  Graphic3d_TextureParams theParams,
                                                  OpenGl_Sampler theSampler,
                                                  uint theTarget,
                                                  int theMaxMipLevels)
        {
            if (theSampler != null && theSampler.Parameters() == theParams)
            {
                theSampler.mySamplerRevision = theParams.SamplerRevision();
            }

            // setup texture filtering
            var aFilter = (theParams.Filter() == Graphic3d_TypeOfTextureFilter.Graphic3d_TOTF_NEAREST) ? All.Nearest : All.Linear;
            var aFilterMin = aFilter;
            if (theMaxMipLevels > 0)
            {
                aFilterMin = All.NearestMipmapNearest;
                if (theParams.Filter() == Graphic3d_TypeOfTextureFilter.Graphic3d_TOTF_BILINEAR)
                {
                    aFilterMin = All.LinearMipmapNearest;
                }
                else if (theParams.Filter() == Graphic3d_TypeOfTextureFilter.Graphic3d_TOTF_TRILINEAR)
                {
                    aFilterMin = All.LinearMipmapLinear;
                }
            }

            setParameter(theCtx, theSampler, theTarget, (int)All.TextureMinFilter , (int) aFilterMin);
            setParameter(theCtx, theSampler, theTarget, (int)All.TextureMagFilter , (int)aFilter);

            // setup texture wrapping
            var aWrapMode = theParams.IsRepeat() ?(int) All.Repeat : theCtx.TextureWrapClamp();
            setParameter(theCtx, theSampler, theTarget,(int) All.TextureWrapS , aWrapMode);
            if (theTarget == (int)All.Texture1D)
            {
                return;
            }

            setParameter(theCtx, theSampler, theTarget, (int)All.TextureWrapT, aWrapMode);
            if (theTarget == (int)All.Texture3D 
             || theTarget ==  (int)All.TextureCubeMap)
            {
                if (theCtx.HasTextureBaseLevel())
                {
                    setParameter(theCtx, theSampler, theTarget, (int)All.TextureWrapR, aWrapMode);
                }
                return;
            }

            if (theCtx.extAnis)
            {
                // setup degree of anisotropy filter
                var aMaxDegree = theCtx.MaxDegreeOfAnisotropy();
                GLint aDegree;
                switch (theParams.AnisoFilter())
                {
                    case Graphic3d_LevelOfTextureAnisotropy.Graphic3d_LOTA_QUALITY:
                        {
                            aDegree = aMaxDegree;
                            break;
                        }
                    case Graphic3d_LevelOfTextureAnisotropy.Graphic3d_LOTA_MIDDLE:
                        {
                            aDegree = (aMaxDegree <= 4) ? 2 : (aMaxDegree / 2);
                            break;
                        }
                    case Graphic3d_LevelOfTextureAnisotropy.Graphic3d_LOTA_FAST:
                        {
                            aDegree = 2;
                            break;
                        }
                    case Graphic3d_LevelOfTextureAnisotropy.Graphic3d_LOTA_OFF:
                    default:
                        {
                            aDegree = 1;
                            break;
                        }
                }

                setParameter(theCtx, theSampler, theTarget, (int)All.TextureMaxAnisotropyExt , aDegree);
            }

            if (theCtx.HasTextureBaseLevel()
             && (theSampler == null || !theSampler.isValidSampler()))
            {
                int aMaxLevel = Math.Min(theMaxMipLevels, theParams.MaxLevel());
                setParameter(theCtx, theSampler, theTarget, (int)All.TextureBaseLevel , theParams.BaseLevel());
                setParameter(theCtx, theSampler, theTarget, (int)All.TextureMaxLevel , aMaxLevel);
            }
        }

        //! Creates and initializes sampler object.
        //! Existing object will be reused if possible, however if existing Sampler Object has Immutable flag
        //! and texture parameters should be re-initialized, then Sampler Object will be recreated.
        public bool Init(OpenGl_Context theCtx,
                                          OpenGl_Texture theTexture)
        {
            if (isValidSampler())
            {
                if (!ToUpdateParameters())
                {
                    return true;
                }
                else if (!myIsImmutable)
                {
                    applySamplerParams(theCtx, myParams, this, theTexture.GetTarget(), theTexture.MaxMipmapLevel());
                    return true;
                }
                Release(theCtx);
            }

            if (!Create(theCtx))
            {
                return false;
            }

            applySamplerParams(theCtx, myParams, this, theTexture.GetTarget(), theTexture.MaxMipmapLevel());
            return true;
        }


        //! Creates an uninitialized sampler object.
        public  bool Create(OpenGl_Context theCtx)
        {
            if (isValidSampler())
            {
                return true;
            }
            else if (theCtx.arbSamplerObject == null)
            {
                return false;
            }

            theCtx.arbSamplerObject.glGenSampler(ref mySamplerID);
            return true;
        }


        //! Creates new sampler object.
        public OpenGl_Sampler(Graphic3d_TextureParams theParams)
        {
            myParams = (theParams);
            mySamplerRevision = (0);
            mySamplerID = NO_SAMPLER;
            myIsImmutable = false;

            if (myParams == null)
            {
                myParams = new Graphic3d_TextureParams();
            }
        }

        //! Returns texture parameters initialization state.
        public bool ToUpdateParameters() { return mySamplerRevision != myParams.SamplerRevision(); }


        public void Release(OpenGl_Context theCtx)
        {
            myIsImmutable = false;
            mySamplerRevision = myParams.SamplerRevision() - 1;
            if (!isValidSampler())
            {
                return;
            }

            // application can not handle this case by exception - this is bug in code
            Exceptions.Standard_ASSERT_RETURN(theCtx != null,
               "OpenGl_Sampler destroyed without GL context! Possible GPU memory leakage...");

            if (theCtx.IsValid())
            {
                theCtx.arbSamplerObject.glDeleteSamplers(1, [mySamplerID]);
            }

            mySamplerID = NO_SAMPLER;
        }
        public void Unbind(OpenGl_Context theCtx,
                              Graphic3d_TextureUnit theUnit)
        {
            if (isValidSampler())
                theCtx.arbSamplerObject.glBindSampler(theUnit, NO_SAMPLER);
        }


        //! Returns texture parameters.
        public Graphic3d_TextureParams Parameters() { return myParams; }


        //! Binds sampler object to the given texture unit.
        public void Bind(OpenGl_Context theCtx,
                               Graphic3d_TextureUnit theUnit)
        {
            if (isValidSampler())
            {
                theCtx.arbSamplerObject.glBindSampler(theUnit, mySamplerID);
            }
        }

        //! Checks if sampler object is valid.
        public bool isValidSampler()
        {
            return mySamplerID != NO_SAMPLER;
        }


        //! Return immutable flag preventing further modifications of sampler parameters, FALSE by default.
        //! Immutable flag might be set when Sampler Object is used within Bindless Texture.
        public bool IsImmutable() { return myIsImmutable; }

        //! Helpful constant defining invalid sampler identifier
        const uint NO_SAMPLER = 0;

        protected Graphic3d_TextureParams myParams;          //!< texture parameters
        protected uint mySamplerRevision; //!< modification counter of parameters related to sampler state
        protected uint mySamplerID;       //!< OpenGL sampler object ID
        protected bool myIsImmutable;     //!< immutable flag preventing further modifications of sampler parameters, FALSE by default

    }
}