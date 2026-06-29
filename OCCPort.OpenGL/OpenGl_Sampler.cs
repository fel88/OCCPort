using OCCPort.OpenGL;
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

            if (myParams==null)
            {
                myParams = new Graphic3d_TextureParams();
            }
        }


        //! Returns texture parameters.
        public Graphic3d_TextureParams Parameters() { return myParams; }

        //! Helpful constant defining invalid sampler identifier
        const uint NO_SAMPLER = 0;

        protected Graphic3d_TextureParams myParams;          //!< texture parameters
        protected uint mySamplerRevision; //!< modification counter of parameters related to sampler state
        protected uint mySamplerID;       //!< OpenGL sampler object ID
        protected bool myIsImmutable;     //!< immutable flag preventing further modifications of sampler parameters, FALSE by default

    }
}