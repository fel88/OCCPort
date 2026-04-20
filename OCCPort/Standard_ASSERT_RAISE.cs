using System;

namespace OCCPort
{
    [Serializable]

    public class Standard_ASSERT_RAISE : Exception
	{

        public Standard_ASSERT_RAISE(string message) : base(message)
        {
        }

        public Standard_ASSERT_RAISE(bool v1, string v2)
        {
            if (v1)
                throw new Standard_ASSERT_RAISE(v2);
        }
    }
}