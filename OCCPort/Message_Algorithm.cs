using System.Reflection;

namespace OCCPort
{
    public class Message_Algorithm
    {
        public void SetStatus(Message_Status theStat)
        {
            myStatus.Set(theStat);

        }
        public void ClearStatus()
        {
            /*myStatus.Clear();
            myReportIntegers.Nullify();
            myReportStrings.Nullify();
            myReportMessages.Nullify();*/
        }
        Message_ExecStatus myStatus;
        //Message_Messenger myMessenger;

    }

}