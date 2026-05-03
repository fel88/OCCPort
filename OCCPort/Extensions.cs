using System.Collections.Generic;

namespace OCCPort
{
    public static class Extensions
    {
        
     
        public static gp_Pnt2d To_gp_Pnt2d(this gp_XY z)
        {
            return new gp_Pnt2d(z);
        }
        public static int Value(this List<int> s, int ind)
        {
            return s[ind - 1];
        }
        public static T Value<T>(this List<T> s, int ind)
        {
            return s[ind - 1];
        }
        public static void InsertAfter<T>(this List<T> s, int ind, T val)
        {
            s.Insert(ind - 1, val);
        }
        public static int Length<T>(this List<T> s)
        {
            return s.Count;
        }
    }

}