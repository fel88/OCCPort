namespace OCCPort
{
    //! Vertex attribute definition.
    public struct Graphic3d_Attribute
    {

        public Graphic3d_TypeOfAttribute Id;       //!< attribute identifier in vertex shader, 0 is reserved for vertex position
        public Graphic3d_TypeOfData DataType; //!< vec2,vec3,vec4,vec4ub


        public int Stride() { return Stride(DataType); }

        //! @return size of attribute of specified data type
        public static int Stride(Graphic3d_TypeOfData theType)
        {
            switch (theType)
            {
                case Graphic3d_TypeOfData.Graphic3d_TOD_USHORT: return sizeof(ushort);
                case Graphic3d_TypeOfData.Graphic3d_TOD_UINT: return sizeof(uint);
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC2: return sizeof(float) * 2;
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC3: return sizeof(float) * 3;
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC4: return sizeof(float) * 4;
                //case Graphic3d_TypeOfData.Graphic3d_TOD_VEC4UB: return sizeof(Graphic3d_Vec4ub);
                case Graphic3d_TypeOfData.Graphic3d_TOD_FLOAT: return sizeof(float);
            }
            return 0;
        }


    }
}