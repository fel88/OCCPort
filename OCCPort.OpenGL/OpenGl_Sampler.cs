using OCCPort.Common;
using OCCPort.OpenGL;
using System;
using System.Reflection.Metadata;
using TKService;

namespace OCCPort
{
    //! Class implements OpenGL sampler object resource that
    //! stores the sampling parameters for a texture access.
    public class OpenGl_Sampler : OpenGl_Resource
    {
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

       public  void Release(OpenGl_Context theCtx)
        {
            myIsImmutable = false;
            mySamplerRevision = myParams.SamplerRevision() - 1;
            if (!isValidSampler())
            {
                return;
            }

            // application can not handle this case by exception - this is bug in code
           Exceptions. Standard_ASSERT_RETURN(theCtx != null,
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

        //! Helpful constant defining invalid sampler identifier
        const uint NO_SAMPLER = 0;

        protected Graphic3d_TextureParams myParams;          //!< texture parameters
        protected uint mySamplerRevision; //!< modification counter of parameters related to sampler state
        protected uint mySamplerID;       //!< OpenGL sampler object ID
        protected bool myIsImmutable;     //!< immutable flag preventing further modifications of sampler parameters, FALSE by default

    }
}