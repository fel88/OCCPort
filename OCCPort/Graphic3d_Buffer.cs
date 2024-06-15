using System;

namespace OCCPort
{
    public class Graphic3d_Buffer
    {
        public int Stride;       //!< the distance to the attributes of the next vertex within interleaved array
        public int NbElements;   //!< number of the elements (@sa NbMaxElements() specifying the number of initially allocated number of elements)
        public int NbAttributes; //!< number of vertex attributes
        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public  void Validate()
        {
            throw new NotImplementedException();
        }

        public  bool IsMutable()
        {
            throw new NotImplementedException();
        }


    }
}