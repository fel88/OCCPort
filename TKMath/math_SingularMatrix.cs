
namespace TKMath
{
    [Serializable]
    internal class math_SingularMatrix : Exception
    {
        public math_SingularMatrix()
        {
        }

        public math_SingularMatrix(string? message) : base(message)
        {
        }

        public math_SingularMatrix(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}