namespace OCCPort.Common
{
    [Serializable]
    public class Standard_DimensionError : Exception
    {
        public Standard_DimensionError()
        {
        }

        public Standard_DimensionError(string message) : base(message)
        {
        }

        public Standard_DimensionError(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
