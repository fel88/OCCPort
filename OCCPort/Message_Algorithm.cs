using System.Reflection;

namespace OCCPort
{
    public class Message_Algorithm
    {
        public void SetStatus(Message_Status theStat)
        {
            myStatus.Set(theStat);

        }  //! Returns copy of exec status of algorithm
        public  Message_ExecStatus GetStatus()
        {
            return myStatus;


        }
        public void ClearStatus()
        {
            myStatus.Clear();
            /*myReportIntegers.Nullify();
            myReportStrings.Nullify();
            myReportMessages.Nullify();*/
        }
        Message_ExecStatus myStatus = new Message_ExecStatus();
        //Message_Messenger myMessenger;

    }

}