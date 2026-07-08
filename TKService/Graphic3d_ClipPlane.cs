using System.Reflection.Metadata;
using TKMath;

namespace TKService
{
    //! Container for properties describing either a Clipping halfspace (single Clipping Plane),
    //! or a chain of Clipping Planes defining logical AND (conjunction) operation.
    //! The plane equation is specified in "world" coordinate system.
    public class Graphic3d_ClipPlane
    {


        //! Return TRUE if this item defines a conjunction (logical AND) between a set of Planes.
        //! Graphic3d_ClipPlane item defines either a Clipping halfspace (single Clipping Plane)
        //! or a Clipping volume defined by a logical AND (conjunction) operation between a set of Planes defined as a Chain
        //! (so that the volume cuts a space only in case if check fails for ALL Planes in the Chain).
        //!
        //! Note that Graphic3d_ClipPlane item cannot:
        //! - Define a Chain with logical OR (disjunction) operation;
        //!   this should be done through Graphic3d_SequenceOfHClipPlane.
        //! - Define nested Chains.
        //! - Disable Chain items; only entire Chain can be disabled (by disabled a head of Chain).
        //!
        //! The head of a Chain defines all visual properties of the Chain,
        //! so that Graphic3d_ClipPlane of next items in a Chain merely defines only geometrical definition of the plane.
        public bool IsChain()  { return myNextInChain!=null; }


        //! Return the next plane in a Chain of Planes defining logical AND operation,
        //! or NULL if there is no chain or it is a last element in chain.

        public Graphic3d_ClipPlane ChainNextPlane() { return myNextInChain; }

        //! Get 4-component equation vector for clipping plane.
        //! @return clipping plane equation vector.
        public Graphic3d_Vec4d ReversedEquation()  { return myEquationRev; }

        //! Get geometrical definition.
        //! @return geometrical definition of clipping plane
        public gp_Pln ToPlane()  { return myPlane; }
        //! Get 4-component equation vector for clipping plane.
        //! @return clipping plane equation vector.
        public Graphic3d_Vec4d GetEquation()  { return myEquation; }

        //! Check state of capping surface rendering.
        //! @return true (turned on) or false depending on the state.
        public bool IsCapping() 
        {
    return myIsCapping;
  }
        //! Check that the clipping plane is turned on.
        //! @return boolean flag indicating whether the plane is in on or off state.
        public bool IsOn()
        {
            return myIsOn;
        }

        //! Return the number of chains in forward direction (including this item, so it is always >= 1).
        //! For a head of Chain - returns the length of entire Chain.
        public int NbChainNextPlanes() { return myChainLenFwd; }
        Graphic3d_AspectFillArea3d myAspect;    //!< fill area aspect
        Graphic3d_ClipPlane myNextInChain;    //!< next     plane in a chain of planes defining logical AND operation
        Graphic3d_ClipPlane myPrevInChain;    //!< previous plane in a chain of planes defining logical AND operation
        string myId;                   //!< resource id
        gp_Pln myPlane;                //!< plane definition
        Graphic3d_Vec4d myEquation;             //!< plane equation vector
        Graphic3d_Vec4d myEquationRev;          //!< reversed plane equation
        int myChainLenFwd;          //!< chain length in forward direction (including this item)
        uint myFlags;                //!< capping flags
        uint myEquationMod;          //!< modification counter for equation
        uint myAspectMod;            //!< modification counter of aspect
        bool myIsOn;                 //!< state of the clipping plane
        bool myIsCapping;            //!< state of graphic driver capping
    }
}
