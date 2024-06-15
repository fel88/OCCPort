namespace OCCPort.OpenGL
{
    internal abstract class OpenGl_Buffer
    {
        //! Return buffer target.
        public abstract uint GetTarget();

        //! Helpful constants
         const uint NO_BUFFER = 0;

        //Standard_Byte* myOffset;       //!< offset to data
        uint myBufferId;     //!< VBO name (index)
        uint myComponentsNb; //!< Number of components per generic vertex attribute, must be 1, 2, 3, or 4
        int myElemsNb;      //!< Number of vertex attributes / number of vertices
        uint myDataType;     //!< Data type (GL_FLOAT, GL_UNSIGNED_INT, GL_UNSIGNED_BYTE etc.)



        //! @return true if current object was initialized
        public bool IsValid()  { return myBufferId != NO_BUFFER; }


}
}