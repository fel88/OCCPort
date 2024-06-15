using System.Security.Cryptography;

namespace OCCPort
{
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

    public bool IsDone()
        {
            return myDone;
        }



    }
}