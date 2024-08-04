using System;

namespace OCCPort
{
    //! Auxiliary class representing a part of the global progress scale allocated by
    //! a step of the progress scope, see Message_ProgressScope::Next().
    //!
    //! A range object takes responsibility of advancing the progress by the size of
    //! allocated step, which is then performed depending on how it is used:
    //!
    //! - If Message_ProgressScope object is created using this range as argument, then
    //!   this respondibility is taken over by that scope.
    //!
    //! - Otherwise, a range advances progress directly upon destruction.
    //!
    //! A range object can be copied, the responsibility for progress advancement is 
    //! then taken by the copy.
    //! The same range object may be used (either copied or used to create scope) only once.
    //! Any consequent attempts to use range will give no result on the progress;
    //! in debug mode, an assert message will be generated.
    //!
    //! @sa Message_ProgressScope for more details
    public class Message_ProgressRange
    { //! Constructor is private
        public Message_ProgressRange(Message_ProgressScope theParent,
                         double theStart, double theDelta)

        {
            myParentScope = (theParent);
            myStart = (theStart);
            myDelta = (theDelta);
            myWasUsed = (false);
        }

        //! Constructor of the empty range
        public Message_ProgressRange()
        {
            myParentScope = null;
            myStart = (0.0);
            myDelta = (0.0);
            myWasUsed = (false);
        }

        Message_ProgressScope myParentScope;  //!< Pointer to parent scope
        double myStart;        //!< Start point on the global scale
        double myDelta;        //!< Step of incrementation on the global scale
        bool myWasUsed;      //!< Flag indicating that this range

        internal bool More()
        {
            throw new NotImplementedException();
        }
        //!  was used to create a new scope
    }
}