using OCCPort;
using OCCPort.Common;
using TKBRep;
using TKMath;

namespace TKTopAlgo
{
    //! Root class for all commands in BRepBuilderAPI.
    //!
    //! Provides :
    //!
    //! * Managements of the notDone flag.
    //!
    //! * Catching of exceptions (not implemented).
    //!
    //! * Logging (not implemented).
    public class BRepBuilderAPI_Command
    {//=======================================================================
     //function : Done
     //purpose  : 
     //=======================================================================
        bool myDone;
        public void Done()
        {
            myDone = true;
        }
        public void Check()
        {
            if (!myDone)
                throw new StdFail_NotDone("BRep_API: command not done");
        }

        public void NotDone()
        {
            myDone = false;
        }


        public bool IsDone()
        {
            return myDone;
        }



    }

    //! Tool to estimate deflection of the given UV point
    //! with regard to its representation in 3D space.
    public struct EvalDeflection
    {

        BRepAdaptor_Surface Surface;

        //! Initializes tool with the given face.
        public EvalDeflection(TopoDS_Face theFace)

        {
            Surface = new BRepAdaptor_Surface(theFace);
        }

        //! Evaluates deflection of the given 2d point from its 3d representation.
        public double Eval(gp_Pnt2d thePoint2d, gp_Pnt thePoint3d)
        {
            gp_Pnt aPnt = new gp_Pnt();
            Surface.D0(thePoint2d.X(), thePoint2d.Y(), ref aPnt);
            return (thePoint3d.XYZ() - aPnt.XYZ()).SquareModulus();
        }
    }
}
