using TKService;

namespace OCCPort.OpenGL
{
    //! Defines generic state of order-independent transparency rendering properties.
    public class OpenGl_OitState : OpenGl_StateInterface
    {

        //! Creates new uniform state.
        public OpenGl_OitState()
        {
            myOitMode = Graphic3d_RenderTransparentMethod.Graphic3d_RTM_BLEND_UNORDERED;
            myDepthFactor = (0.5f);
        }
        //! Returns flag indicating whether writing of output for OIT processing
        //! should be enabled/disabled.
        public Graphic3d_RenderTransparentMethod ActiveMode() { return myOitMode; }

        //! Returns factor defining influence of depth component of a fragment
        //! to its final coverage coefficient.
        public float DepthFactor() { return myDepthFactor; }
        Graphic3d_RenderTransparentMethod myOitMode;     //!< active OIT method for the main GLSL program
        float myDepthFactor; //!< factor of depth influence to coverage
    }
}