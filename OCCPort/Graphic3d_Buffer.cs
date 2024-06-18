using System;
using System.Security.Policy;

namespace OCCPort
{
	public class Graphic3d_Buffer: NCollection_Buffer
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

		internal byte[] AttributeData(Graphic3d_TypeOfAttribute graphic3d_TOA_POS, int anAttribIndex, int anAttribStride)
		{
			throw new NotImplementedException();
		}

		//! @return attribute definition
		public Graphic3d_Attribute Attribute(int theAttribIndex)
		{
			return AttributesArray()[theAttribIndex];
		}
		//! @return array of attributes definitions
		public Graphic3d_Attribute[] AttributesArray()
		{
			return new Graphic3d_Attribute[0];
			//return (Graphic3d_Attribute)(myData + mySize);
		}
    }
}