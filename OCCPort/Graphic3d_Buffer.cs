using System;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCCPort
{
    public class Graphic3d_Buffer : NCollection_Buffer
    {  //! Empty constructor.
        public Graphic3d_Buffer(NCollection_BaseAllocator theAlloc) : base(theAlloc)
        {
            Stride = (0);
            NbElements = (0);
            NbAttributes = (0);
        }


        public int Stride;       //!< the distance to the attributes of the next vertex within interleaved array
        public int NbElements;   //!< number of the elements (@sa NbMaxElements() specifying the number of initially allocated number of elements)
        public int NbAttributes; //!< number of vertex attributes
        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public void Validate()
        {

        }

        public bool IsMutable()
        {
            return false;
        }
        //! Flag indicating that attributes in the buffer are interleaved; TRUE by default.
        //! Requires sub-classing for creating a non-interleaved buffer (advanced usage).
        public virtual bool IsInterleaved() { return true; }

        //! Return the attribute data with stride size specific to this attribute.
        public byte[] AttributeData(Graphic3d_TypeOfAttribute theAttrib,
            int theAttribIndex, int theAttribStride)
        {
            int aDataPtr = 0;
            if (IsInterleaved())
            {
                for (int anAttribIter = 0; anAttribIter < NbAttributes; ++anAttribIter)
                {
                    Graphic3d_Attribute anAttrib = Attribute(anAttribIter);
                    int anAttribStride = Graphic3d_Attribute.Stride(anAttrib.DataType);
                    if (anAttrib.Id == theAttrib)
                    {
                        theAttribIndex = anAttribIter;
                        theAttribStride = Stride;
                        return Data().Skip(aDataPtr).ToArray();
                    }

                    aDataPtr += anAttribStride;
                }
            }
            else
            {
                int aNbMaxVerts = NbMaxElements();
                for (int anAttribIter = 0; anAttribIter < NbAttributes; ++anAttribIter)
                {
                    Graphic3d_Attribute anAttrib = Attribute(anAttribIter);
                    int anAttribStride = Graphic3d_Attribute.Stride(anAttrib.DataType);
                    if (anAttrib.Id == theAttrib)
                    {
                        theAttribIndex = anAttribIter;
                        theAttribStride = anAttribStride;
                        return Data().Skip(aDataPtr).ToArray();
                    }

                    aDataPtr += anAttribStride * aNbMaxVerts;
                }
            }
            return null;
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

        internal static NCollection_BaseAllocator DefaultAllocator()
        {
            return new NCollection_BaseAllocator();
        }


        //! Return number of initially allocated elements which can fit into this buffer,
        //! while NbElements can be overwritten to smaller value.
        public int NbMaxElements() { return Stride != 0 ? (int)(mySize / (int)(Stride)) : 0; }

    }


}