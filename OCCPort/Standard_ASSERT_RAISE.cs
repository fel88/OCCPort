using System;

namespace OCCPort
{
    [Serializable]

    public class Standard_ASSERT_RAISE : Exception
	{

        public Standard_ASSERT_RAISE(string message) : base(message)
        {
        }
    }
}