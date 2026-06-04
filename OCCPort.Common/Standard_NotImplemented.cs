namespace OCCPort.Common
{
    [Serializable]
    public class Standard_NotImplemented : NotImplementedException
    {
        public Standard_NotImplemented()
        {
        }

        public Standard_NotImplemented(string message) : base(message)
        {
        }

        public Standard_NotImplemented(string message, Exception innerException) : base(message, innerException)
        {
        }


    }

}
