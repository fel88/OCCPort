
namespace TKV3d
{
    [Serializable]
    internal class Standard_TypeMismatch : Exception
    {
        public Standard_TypeMismatch()
        {
        }

        public Standard_TypeMismatch(string? message) : base(message)
        {
        }

        public Standard_TypeMismatch(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}