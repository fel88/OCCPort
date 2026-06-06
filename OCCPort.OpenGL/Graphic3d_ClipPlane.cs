using System;
using TKMath;
using TKService;

namespace OCCPort.OpenGL
{
    //! Container for properties describing either a Clipping halfspace (single Clipping Plane),
    //! or a chain of Clipping Planes defining logical AND (conjunction) operation.
    //! The plane equation is specified in "world" coordinate system.
    public class Graphic3d_ClipPlane
    {

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