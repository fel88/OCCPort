using TKernel;

namespace TKMath
{
    public class Poly_ListOfTriangulation : NCollection_List<Poly_Triangulation>
    {


        public class Iterator
        {



            Poly_ListOfTriangulation target;
            public Iterator(Poly_ListOfTriangulation theTriangulations)
            {
                target = theTriangulations;
            }

            public bool More()
            {
                return index < target.Count - 1;
            }
            int index = 0;
            public Poly_Triangulation Next()
            {
                index++;
                return target[index];
            }

            public Poly_Triangulation Value()
            {
                return target[index];
            }
        }
        //      public void Append(Poly_Triangulation p)
        //      {
        //          this.Add(p);
        //      }

        public void Set(int index, Poly_Triangulation p)
        {
            this[index] = p;
        }

        //      public bool IsEmpty()
        //{
        //	return this.Count == 0;
        //}

        //internal Poly_Triangulation First()
        //{
        //	return this[0];
        //}

        //      internal int Size()
        //      {
        //          return Count;
        //      }
    }
}
