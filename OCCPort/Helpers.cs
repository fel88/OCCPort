namespace OCCPort
{
    public static class Helpers
    {
        public static string Prepend(this string str, string s)
        {
            return s + str;
        }
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        public static void Clear(this string str)
        {
            str = "";
        }
        public static char Value(this string str, int index)
        {
            return str[index];
        }
        public static int Length(this string str)
        {
            return str.Length();
        }
        public static string Token(this string str, string ch, int n)
        {
            return str.Substring(str.IndexOf(ch), n);
        }
    }

}