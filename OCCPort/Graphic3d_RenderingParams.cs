namespace OCCPort
{
    public class Graphic3d_RenderingParams
    {
        public bool ToEnableAlphaToCoverage { get; set; }
        public Graphic3d_RenderingMode Method { get; set; }
        public int NbMsaaSamples;               //!< number of MSAA samples (should be within 0..GL_MAX_SAMPLES, power-of-two number), 0 by default
        public float RenderResolutionScale;       //!< rendering resolution scale factor, 1 by default;

        public Graphic3d_TypeOfShadingModel ShadingModel;                //!< specified default shading model, Graphic3d_TypeOfShadingModel_Phong by default
        public Graphic3d_StereoMode StereoMode;                  //!< stereoscopic output mode, Graphic3d_StereoMode_QuadBuffer by default
    }    //! This enumeration defines the list of stereoscopic output modes.
    //public enum { Graphic3d_StereoMode_NB = Graphic3d_StereoMode_OpenVR + 1 };




}