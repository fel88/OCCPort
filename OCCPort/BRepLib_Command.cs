namespace OCCPort
{
    //! Root class for all commands in BRepLib.
    //!
    //! Provides :
    //!
    //! * Managements of the notDone flag.
    //!
    //! * Catching of exceptions (not implemented).
    //!
    //! * Logging (not implemented).
    public class BRepLib_Command
    {
        public bool IsDone()
        {
            return myDone;
        }

        public void Check()
        {
            if (!myDone)
                throw new StdFail_NotDone("BRep_API: command not done");
        }

        public void Done()
        {
            myDone = true;
        }


        bool myDone;
    }
}