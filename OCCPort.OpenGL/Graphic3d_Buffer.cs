using System;

namespace OCCPort.OpenGL
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

        internal void Validate()
        {
            throw new NotImplementedException();
        }

        internal bool IsMutable()
        {
            throw new NotImplementedException();
        }

        
    }
}