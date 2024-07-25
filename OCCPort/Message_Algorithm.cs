using System.Reflection;

namespace OCCPort
{
	public class Message_Algorithm
	{
		public void SetStatus(Message_Status theStat)
		{
			myStatus.Set(theStat);

		}
		Message_ExecStatus myStatus;
		//Message_Messenger myMessenger;

	}

}