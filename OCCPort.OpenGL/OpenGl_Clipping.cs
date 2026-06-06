using System;
using TKernel;
using TKService;

namespace OCCPort.OpenGL
{
    //! This class contains logics related to tracking and modification of clipping plane
    //! state for particular OpenGl context. It contains information about enabled
    //! clipping planes and provides method to change clippings in context. The methods
    //! should be executed within OpenGl context associated with instance of this
    //! class.
    public class OpenGl_Clipping
    {

        //! Return TRUE if there are clipping chains in the list (defining more than 1 sub-plane)
        public bool HasClippingChains()
        {
            if (IsCappingDisableAllExcept()                 // all chains are disabled - only single (sub) plane is active;
             || myNbChains == (myNbClipping + myNbCapping)) // no sub-planes
            {
                return false;
            }
            return !IsCappingEnableAllExcept()
                 || myCappedChain.NbChainNextPlanes() == 1
                 || myNbChains > 1; // if capping filter ON - chains counter should be decremented
        }


        //! @return number of enabled clipping + capping planes
        public int NbClippingOrCappingOn()
        {
            if (IsCappingDisableAllExcept())
            {
                return 1; // all Chains are disabled - only single (sub) plane is active
            }
            return myNbClipping + myNbCapping
                + (IsCappingEnableAllExcept() ? -1 : 0); // exclude 1 plane with Capping filter turned ON
        }

        //! Return TRUE if capping algorithm is in state, when all clipping planes are temporarily disabled except currently processed one.
        public bool IsCappingDisableAllExcept() { return myCappedSubPlane > 0; }

        //! Return TRUE if capping algorithm is in state, when all clipping planes are enabled except currently rendered one.
        public bool IsCappingEnableAllExcept() { return myCappedSubPlane < 0; }

        Graphic3d_SequenceOfHClipPlane myPlanesGlobal;   //!< global clipping planes
        Graphic3d_SequenceOfHClipPlane myPlanesLocal;    //!< object clipping planes
        NCollection_Vector<bool> myDisabledPlanes = new NCollection_Vector<bool>(); //!< ids of disabled planes

        Graphic3d_ClipPlane myCappedChain;    //!< chain which is either temporary disabled or the only one enabled for Capping algorithm
        int myCappedSubPlane; //!< sub-plane index within filtered chain; positive number for DisableAllExcept and negative for EnableAllExcept

        int myNbClipping;     //!< number of enabled clipping-only planes (NOT capping)
        int myNbCapping;      //!< number of enabled capping  planes
        int myNbChains;       //!< number of enabled chains
        int myNbDisabled;     //!< number of defined but disabled planes
    }
}