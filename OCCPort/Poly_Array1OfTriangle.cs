using System;

namespace OCCPort
{
    internal class Poly_Array1OfTriangle
    {
        Poly_Triangle[] triangles;
        
        internal void SetValue(int theIndex, Poly_Triangle theTriangle)
        {
            //todo: ???
            triangles[theIndex] = theTriangle;
        }
    }
}