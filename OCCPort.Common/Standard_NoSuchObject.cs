namespace OCCPort.Common
{
    [Serializable]
    public class Standard_NoSuchObject : Exception
    {
        public Standard_NoSuchObject()
        {
        }

        public Standard_NoSuchObject(string message) : base(message)
        {
        }

        public Standard_NoSuchObject(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
