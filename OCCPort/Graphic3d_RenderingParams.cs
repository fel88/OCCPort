namespace OCCPort
{
    public class Graphic3d_RenderingParams
    {
        public bool ToEnableAlphaToCoverage { get; set; }
        public Graphic3d_RenderingMode Method { get; set; }
        public Graphic3d_StereoMode StereoMode;                  //!< stereoscopic output mode, Graphic3d_StereoMode_QuadBuffer by default
    }    //! This enumeration defines the list of stereoscopic output modes.
    //public enum { Graphic3d_StereoMode_NB = Graphic3d_StereoMode_OpenVR + 1 };


}