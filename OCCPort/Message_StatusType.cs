namespace OCCPort
{
	//! Definition of types of execution status supported by
	//! the class Message_ExecStatus

	enum Message_StatusType
	{
		Message_DONE = 0x00000100,
		Message_WARN = 0x00000200,
		Message_ALARM = 0x00000400,
		Message_FAIL = 0x00000800
	}

}