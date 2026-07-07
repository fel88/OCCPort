using TKernel;

namespace TKService
{
    //! Helper class for keeping reference on world-view-projection state.
    //! Helpful for synchronizing state of WVP dependent data structures.
    public class Graphic3d_WorldViewProjState
    {
        bool myIsValid;

        object myCamera;
        int myProjectionState;
        int myWorldViewState;
        //! Initialize world view projection state.
        public void Initialize(object theCamera = null)
        {
            myIsValid = true;
            myCamera = (theCamera);
            myProjectionState = 0;
            myWorldViewState = 0;
        }

        //! Initialize world view projection state.
        public void Initialize(int theProjectionState,
                        int theWorldViewState,
                        object theCamera = null)
        {
            myIsValid = true;
            myCamera = (theCamera);
            myProjectionState = theProjectionState;
            myWorldViewState = theWorldViewState;
        }

        public static bool operator ==(Graphic3d_WorldViewProjState a, Graphic3d_WorldViewProjState b)
        {
            return !(a != b);
        }
        public static bool operator !=(Graphic3d_WorldViewProjState a, Graphic3d_WorldViewProjState b)
        {
            return a.myIsValid == b.myIsValid
        && a.myCamera == b.myCamera
        && a.myProjectionState == b.myProjectionState
        && a.myWorldViewState == b.myWorldViewState;
        }
    }
}
