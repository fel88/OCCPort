using System;

namespace OCCPort
{
    public static class Exceptions
    {
        public static void Standard_NoSuchObject_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        internal static void Standard_ConstructionError_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        internal static void Standard_RangeError_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }
    }

}