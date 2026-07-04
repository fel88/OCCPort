using OCCPort.Common;
using System;
using System.Numerics;
using System.Reflection.Metadata;
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

        //! Remove the passed set of clipping planes from the context state.
        //! @param thePlanes [in] the planes to remove from list.
        void remove(Graphic3d_SequenceOfHClipPlane thePlanes,
                              int theStartIndex)
        {
            if (thePlanes == null)
            {
                return;
            }

            int aPlaneIndex = theStartIndex;
            for (Graphic3d_SequenceOfHClipPlane.Iterator aPlaneIt =new Graphic3d_SequenceOfHClipPlane.Iterator (thePlanes); aPlaneIt.More(); aPlaneIt.Next(), ++aPlaneIndex)
            {
                Graphic3d_ClipPlane aPlane = aPlaneIt.Value();
                if (!aPlane.IsOn()
                 || myDisabledPlanes.Value(aPlaneIndex))
                {
                    continue;
                }

                int aNbSubPlanes = aPlane.NbChainNextPlanes();
                myNbChains -= 1;
                if (aPlane.IsCapping())
                {
                    myNbCapping -= aNbSubPlanes;
                }
                else
                {
                    myNbClipping -= aNbSubPlanes;
                }
            }
        }

        //! Setup list of global (for entire view) clipping planes
        //! and clears local plane list if it was not released before.
        public void Reset(Graphic3d_SequenceOfHClipPlane thePlanes)
        {
            int aStartIndex = myPlanesGlobal == null ? 1 : myPlanesGlobal.Size() + 1;
            remove(myPlanesLocal, aStartIndex);
            remove(myPlanesGlobal, 1);

            myPlanesGlobal = thePlanes;
            myPlanesLocal = null;

            add(thePlanes, 1);
            myNbDisabled = 0;
            myCappedSubPlane = 0;
            myCappedChain = null;

            // Method ::add() implicitly extends myDisabledPlanes (NCollection_Vector::SetValue()),
            // however we do not reset myDisabledPlanes and mySkipFilter beforehand to avoid redundant memory re-allocations.
            // So once extended, they will never reduce their size to lower values.
            // This should not be a problem since overall number of clipping planes is expected to be quite small.
        }
        void add(Graphic3d_SequenceOfHClipPlane thePlanes,
                           int theStartIndex)
        {
            if (thePlanes == null)
            {
                return;
            }

            int aPlaneId = theStartIndex;
            for (Graphic3d_SequenceOfHClipPlane.Iterator aPlaneIt = new Graphic3d_SequenceOfHClipPlane.Iterator(thePlanes); aPlaneIt.More(); aPlaneIt.Next(), ++aPlaneId)
            {
                Graphic3d_ClipPlane aPlane = aPlaneIt.Value();
                myDisabledPlanes.SetValue(aPlaneId, false); // automatically resizes the vector
                if (!aPlane.IsOn())
                {
                    continue;
                }

                int aNbSubPlanes = aPlane.NbChainNextPlanes();
                myNbChains += 1;
                if (aPlane.IsCapping())
                {
                    myNbCapping += aNbSubPlanes;
                }
                else
                {
                    myNbClipping += aNbSubPlanes;
                }
            }
        }


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