using OpenTK.Graphics.OpenGL;
using System.Reflection.Metadata;

namespace OCCPort.OpenGL
{
    public abstract class OpenGl_Buffer
    {

        public OpenGl_Buffer()
        {
            myDataType = All.Float;
        }

        //! @return number of vertex attributes / number of vertices specified within ::Init()
        public int GetElemsNb() { return myElemsNb; }

        //! Return buffer target.
        public abstract BufferTarget GetTarget();
        //! @return offset to data, NULL by default
        public int GetDataOffset() { return myOffset; }

        //! Unbind this buffer object.
        public virtual void Unbind(OpenGl_Context theGlCtx)
        {
            theGlCtx.core15fwd.glBindBuffer(GetTarget(), NO_BUFFER);

        }

        //! Helpful constants
        const uint NO_BUFFER = 0;

        int myOffset;       //!< offset to data
        uint myBufferId;     //!< VBO name (index)
        uint myComponentsNb; //!< Number of components per generic vertex attribute, must be 1, 2, 3, or 4
        int myElemsNb;      //!< Number of vertex attributes / number of vertices
        All myDataType;     //!< Data type (GL_FLOAT, GL_UNSIGNED_INT, GL_UNSIGNED_BYTE etc.)

        //! Overrides the number of vertex attributes / number of vertexes.
        //! It is up to user specifying this number correct (e.g. below initial value)!
        public void SetElemsNb(int theNbElems) { myElemsNb = theNbElems; }
        //! @return data type of each component in the array.
        public All GetDataType() { return myDataType; }

        //! @return true if current object was initialized
        public bool IsValid() { return myBufferId != NO_BUFFER; }

        // =======================================================================
        // function : init
        // purpose  :
        // =======================================================================

        public bool Init(OpenGl_Context theGlCtx,
                          uint theComponentsNb,
                          int theElemsNb,
                          float[] theData)
        {

            if (myDataType == All.Float)
            {
                theGlCtx.core15fwd.glBufferData(GetTarget(), myElemsNb * sizeof(float), theData, OpenTK.Graphics.OpenGL.BufferUsageHint.StaticDraw);
            }
            return true;
        }
        public bool Create(OpenGl_Context theGlCtx)
        {
            if (myBufferId == NO_BUFFER && theGlCtx.core15fwd != null)
            {
                theGlCtx.core15fwd.glGenBuffers(1, ref myBufferId);
            }
            return myBufferId != NO_BUFFER;
        }
        public bool init(OpenGl_Context theGlCtx,
                                  uint theComponentsNb,
                                  int theElemsNb,
                                  byte[] theData,
                                  All theDataType,
                                  int theStride)
        {
            if (theDataType == All.UnsignedByte)
            {

            }
            if (!Create(theGlCtx))
            {
                return false;
            }

            Bind(theGlCtx);
            myDataType = theDataType;
            myComponentsNb = theComponentsNb;
            myElemsNb = theElemsNb;
            theGlCtx.core15fwd.glBufferData(GetTarget(), myElemsNb * theStride, theData, OpenTK.Graphics.OpenGL.BufferUsageHint.StaticDraw);
            return true;
        }
        public void Bind(OpenGl_Context theGlCtx)
        {
            theGlCtx.core15fwd.glBindBuffer(GetTarget(), myBufferId);
        }

    }
}