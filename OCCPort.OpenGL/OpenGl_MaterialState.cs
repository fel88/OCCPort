using System;

namespace OCCPort.OpenGL
{
    public class OpenGl_MaterialState: OpenGl_StateInterface
    {
        //! Return TRUE if alpha test should be enabled.
        internal bool HasAlphaCutoff()
        {
            return myAlphaCutoff <= 1.0f;

        }
        OpenGl_Material myMaterial;      //!< material
        float myAlphaCutoff;   //!< alpha cutoff value
        bool myToDistinguish; //!< distinguish front/back flag
        bool myToMapTexture;  //!< flag for mapping a texture
    }

}