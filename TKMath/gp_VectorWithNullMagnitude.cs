using System;

namespace TKMath
{
    [Serializable]
    internal class gp_VectorWithNullMagnitude : Exception
    {
        public gp_VectorWithNullMagnitude()
        {
        }

        public gp_VectorWithNullMagnitude(string message) : base(message)
        {
        }

        public gp_VectorWithNullMagnitude(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}