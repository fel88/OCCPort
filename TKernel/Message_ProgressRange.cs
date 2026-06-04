namespace TKernel
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
        //! Returns true if ProgressIndicator signals UserBreak
        bool UserBreak() => myParentScope != null && myParentScope.myProgress != null && myParentScope.myProgress.UserBreak();

        Message_ProgressScope myParentScope;  //!< Pointer to parent scope
        double myStart;        //!< Start point on the global scale
        double myDelta;        //!< Step of incrementation on the global scale
        bool myWasUsed;      //!< Flag indicating that this range

        public bool More()
        {
            return !UserBreak();

        }
        //!  was used to create a new scope
    }
    public class Message_ProgressScope
    {  //! Creates a new scope taking responsibility of the part of the progress 
       //! scale described by theRange. The new scope has own range from 0 to 
       //! theMax, which is mapped to the given range.
       //!
       //! The topmost scope is created and owned by Message_ProgressIndicator
       //! and its pointer is contained in the Message_ProgressRange returned by the Start() method of progress indicator.
       //!
       //! @param theRange [in][out] range to fill (will be disarmed)
       //! @param theName  [in]      new scope name
       //! @param theMax   [in]      number of steps in scope
       //! @param isInfinite [in]    infinite flag
        public Message_ProgressScope(Message_ProgressRange theRange,
                          string theName,
                         double theMax,
                         bool isInfinite = false)
        {
            /*myProgress(theRange.myParentScope != NULL ? theRange.myParentScope->myProgress : NULL),
  myParent(theRange.myParentScope),
  myName(NULL),
  myStart(theRange.myStart),
  myPortion(theRange.myDelta),
  myMax(Max(1.e - 6, theMax)), // protection against zero range
  myValue(0.),
  myIsActive(myProgress != NULL && !theRange.myWasUsed),
  myIsOwnName(false),
  myIsInfinite(isInfinite);*/
            //SetName(theName);
            //Standard_ASSERT_VOID(!theRange.myWasUsed, "Message_ProgressRange is used to initialize more than one scope");
            // theRange.myWasUsed = true; // Disarm the range
        }  //! Advances position by specified step and returns the range
           //! covering this step
        public Message_ProgressRange Next(double theStep = 1.0)
        {
            if (myIsActive && theStep > 0.0)
            {
                double aCurr = localToGlobal(myValue);
                double aNext = localToGlobal(myValue += theStep);
                double aDelta = aNext - aCurr;
                if (aDelta > 0.0)
                {
                    return new Message_ProgressRange(this, myStart + aCurr, aDelta);
                }
            }
            return new Message_ProgressRange();

        }
        string myName;        //!< Name of the operation being done in this scope, or null

        double myStart;       //!< Start position on the global scale [0, 1]
        double myPortion;     //!< The portion of the global scale covered by this scope [0, 1]

        double myMax;         //!< Maximal value of progress in this scope
        double myValue;       //!< Current position advanced within this scope [0, Max]

        bool myIsActive;    //!< flag indicating armed/disarmed state
        bool myIsOwnName;   //!< flag indicating if name was allocated or not
        bool myIsInfinite;  //!< Option to advance by hyperbolic law

        public Message_ProgressIndicator myProgress; //!< Pointer to progress indicator instance
        double RealSmall()
        { return double.MinValue; }

        public double localToGlobal(double theVal)
        {
            if (theVal <= 0.0)
                return 0.0;

            if (!myIsInfinite)
            {
                if (myMax - theVal < RealSmall())
                    return myPortion;
                return myPortion * theVal / myMax;
            }

            double x = theVal / myMax;
            // return myPortion * ( 1. - std::exp ( -x ) ); // exponent
            return myPortion * x / (1.0 + x);  // hyperbola
        }

        //! Returns true if ProgressIndicator signals UserBreak
        public bool UserBreak()
        {
            return myProgress != null && myProgress.UserBreak();
        }


        //! Returns false if ProgressIndicator signals UserBreak
        public bool More()
        {
            return !UserBreak();
        }
    }
    public class Message_ProgressIndicator
    {
        public virtual bool UserBreak()
        {
            return false;
        }

    }
}

