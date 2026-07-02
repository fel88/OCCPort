namespace OCCPort.Common
{
    public static class Exceptions
    {
        public static void Standard_ASSERT_RETURN(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        public static void Standard_NoSuchObject_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        public static void Standard_ProgramError_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        public static void math_NotSquare_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        public static void Standard_ConstructionError_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        public static void Standard_OutOfRange_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        public static void Standard_RangeError_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        public static void StdFail_NotDone_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        public static void Standard_ASSERT_RAISE(bool v1, string v2)
        {
            if (!v1)
                throw new Exception(v2);
        }

        public static void V3d_BadValue_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
            
        }
    }
}
