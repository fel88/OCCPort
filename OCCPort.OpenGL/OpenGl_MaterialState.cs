using System;

namespace OCCPort.OpenGL
{
    public class OpenGl_MaterialState : OpenGl_StateInterface
    {
        //! Return TRUE if alpha test should be enabled.
        internal bool HasAlphaCutoff()
        {
            return myAlphaCutoff <= 1.0f;

        }
        //! Sets new material aspect.
        public void Set(OpenGl_Material theMat,
               float theAlphaCutoff,
               bool theToDistinguish,
               bool theToMapTexture)
        {
            myMaterial = theMat;
            myAlphaCutoff = theAlphaCutoff;
            myToDistinguish = theToDistinguish;
            myToMapTexture = theToMapTexture;
        }

        //! Alpha cutoff value.
        public float AlphaCutoff() { return myAlphaCutoff; }

        OpenGl_Material myMaterial;      //!< material
        float myAlphaCutoff;   //!< alpha cutoff value
        bool myToDistinguish; //!< distinguish front/back flag
        bool myToMapTexture;  //!< flag for mapping a texture
    }

}