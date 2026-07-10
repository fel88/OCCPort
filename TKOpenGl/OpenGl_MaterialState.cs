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

        //! Distinguish front/back flag.
        public bool ToDistinguish() { return myToDistinguish; }


        //! Return front material.
        public OpenGl_Material Material() { return myMaterial; }

        //! Flag for mapping a texture.
        public bool ToMapTexture() { return myToMapTexture; }


        OpenGl_Material myMaterial;      //!< material
        float myAlphaCutoff;   //!< alpha cutoff value
        bool myToDistinguish; //!< distinguish front/back flag
        bool myToMapTexture;  //!< flag for mapping a texture
    }

}