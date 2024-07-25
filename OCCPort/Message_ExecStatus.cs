namespace OCCPort
{
	/**
	 * Tiny class for extended handling of error / execution
	 * status of algorithm in universal way.
	 *
	 * It is in fact a set of integers represented as a collection of bit flags
	 * for each of four types of status; each status flag has its own symbolic 
	 * name and can be set/tested individually.
	 *
	 * The flags are grouped in semantic groups: 
	 * - No flags means nothing done
	 * - Done flags correspond to some operation successfully completed
	 * - Warning flags correspond to warning messages on some 
	 *   potentially wrong situation, not harming algorithm execution
	 * - Alarm flags correspond to more severe warnings about incorrect
	 *   user data, while not breaking algorithm execution
	 * - Fail flags correspond to cases when algorithm failed to complete
	 */
	class Message_ExecStatus
	{

		//! Returns status type (DONE, WARN, ALARM, or FAIL) 
		static Message_StatusType TypeOfStatus(Message_Status theStatus)
		{
			return (Message_StatusType)((uint)theStatus & (uint)StatusMask.MType);
		}
		static int getBitFlag(int theStatus)
		{
			return 0x1 << (theStatus & (int)StatusMask.MIndex);
		}

		int myDone;
		int myWarn;
		int myAlarm;
		int myFail;

		//! Create empty execution status
		public Message_ExecStatus()

		{
			myDone = (int)Message_Status.Message_None;
			myWarn = (int)Message_Status.Message_None;
			myAlarm = (int)Message_Status.Message_None;
			myFail = (int)Message_Status.Message_None;
		}

		//! Sets a status flag
		public void Set(Message_Status theStatus)
		{
			switch (TypeOfStatus(theStatus))
			{
				case Message_StatusType.Message_DONE: myDone |= (getBitFlag((int)theStatus)); break;
				case Message_StatusType.Message_WARN: myWarn |= (getBitFlag((int)theStatus)); break;
				case Message_StatusType.Message_ALARM: myAlarm |= (getBitFlag((int)theStatus)); break;
				case Message_StatusType.Message_FAIL: myFail |= (getBitFlag((int)theStatus)); break;
			}
		}

		//! Mask to separate bits indicating status type and index within the type  
		enum StatusMask
		{
			MType = 0x0000ff00,
			MIndex = 0x000000ff
		};

	}

}