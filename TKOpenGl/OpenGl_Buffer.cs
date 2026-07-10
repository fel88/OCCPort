using OCCPort.Common;
using OpenTK.Graphics.OpenGL;
using System;
using System.Linq;
using System.Reflection.Metadata;
using TKMath;

namespace OCCPort.OpenGL
{
    public abstract class OpenGl_Buffer
    {

        public OpenGl_Buffer()
        {
            myDataType = All.Float;
        }
        public void Release(OpenGl_Context theGlCtx)
        {
            if (myBufferId == NO_BUFFER)
            {
                return;
            }

            // application can not handle this case by exception - this is bug in code
            Exceptions.Standard_ASSERT_RETURN(theGlCtx != null,
              "OpenGl_Buffer destroyed without GL context! Possible GPU memory leakage...");

            if (theGlCtx.IsValid())
            {
                theGlCtx.core15fwd.glDeleteBuffers(1, ref myBufferId);
            }
            myOffset = 0;
            myBufferId = NO_BUFFER;
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

        protected int myOffset;       //!< offset to data
        protected uint myBufferId;     //!< VBO name (index)
        protected uint myComponentsNb; //!< Number of components per generic vertex attribute, must be 1, 2, 3, or 4
        protected int myElemsNb;      //!< Number of vertex attributes / number of vertices
        protected All myDataType;     //!< Data type (GL_FLOAT, GL_UNSIGNED_INT, GL_UNSIGNED_BYTE etc.)

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
                        byte[] theData)
        {
            return init(theGlCtx, theComponentsNb, theElemsNb, theData, (int)All.UnsignedShort);

        }
        int sizeOfGlType(int theType)
        {
            switch (theType)
            {
                case (int)All.Byte:
             //   case GL_UNSIGNED_BYTE: return sizeof(Standard_Byte);
                //case GL_SHORT:
                case (int)All.UnsignedShort: return sizeof(ushort);
//# ifdef GL_INT
//                case GL_INT:
//#endif
//                case GL_UNSIGNED_INT: return sizeof(unsigned int);
                case (int)All.Float: return sizeof(float);
//# ifdef GL_DOUBLE
//                case GL_DOUBLE: return sizeof(double);
//#endif
                default: return 0;
            }
        }
        //! Initialize buffer with new data.
        bool init(OpenGl_Context theGlCtx,
              uint theComponentsNb,
              int theElemsNb,
              byte[] theData,
               int theDataType)
        {
            return init(theGlCtx, theComponentsNb, theElemsNb, theData, (All)theDataType,
                         (int)(theComponentsNb) * (int)(sizeOfGlType(theDataType)));
        }

        public bool Init(OpenGl_Context theGlCtx,
                          uint theComponentsNb,
                          int theElemsNb,
                          float[] theData)
        {

            var data = theData.SelectMany(z => BitConverter.GetBytes(z)).ToArray();
            return init(theGlCtx, theComponentsNb, theElemsNb, data, (int)All.Float);
            /*if (myDataType == All.Float)
            {
                theGlCtx.core15fwd.glBufferData(GetTarget(), theElemsNb * sizeof(float), theData, OpenTK.Graphics.OpenGL.BufferUsageHint.StaticDraw);
            }*/
            //return true;
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
            if (!Create(theGlCtx))            
                return false;
            

            Bind(theGlCtx);
            myDataType = theDataType;
            myComponentsNb = theComponentsNb;
            myElemsNb = theElemsNb;
            theGlCtx.core15fwd.glBufferData(GetTarget(), myElemsNb * theStride, theData, OpenTK.Graphics.OpenGL.BufferUsageHint.StaticDraw);
             int anErr = theGlCtx.core15fwd.glGetError();
            if (anErr != (int)All.NoError
    && anErr != (int)All.OutOfMemory ) // pass-through out-of-memory error, but log unexpected errors
            {
                theGlCtx.PushMessage(All.DebugSourceApplication, All.DebugTypeError, 0, All.DebugSeverityHigh,
                           ("Error: glBufferData (")
                           //+ FormatTarget(GetTarget()) + ","
                           //+ OpenGl_Context.FormatSize(GLsizeiptr(myElemsNb) * theStride) + ","
                           //+ OpenGl_Context.FormatPointer(theData) + ") Id: " + (int)myBufferId
                           + " failed with " + OpenGl_Context.FormatGlError(anErr));
            }
                Unbind(theGlCtx);
            
            return anErr == (int)All.NoError ;
            
        }
        public void Bind(OpenGl_Context theGlCtx)
        {
            theGlCtx.core15fwd.glBindBuffer(GetTarget(), myBufferId);
        }

    }
}