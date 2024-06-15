using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OCCPort.OpenGL
{
    public class OpenGl_MatrixState<T>
    {

        OpenGl_MatrixState()
        {
            myStack = new Graphic3d_Mat4[8];
        }
        //! Pushes current matrix into stack.
        public void Push()
        {
            if (++myStackHead >= myStack.Length)
            {
                myStack.Append(myCurrent);
            }
            else
            {
                //myStack.SetValue(myStackHead, myCurrent);
                myStack[myStackHead] = myCurrent;
            }
        }

        Graphic3d_Mat4[] myStack;     //!< Collection used to maintenance matrix stack
        Graphic3d_Mat4 myCurrent;   //!< Current matrix

        int myStackHead; //!< Index of stack head


        internal void SetCurrent(Graphic3d_Mat4 aWorldView)
        {
            throw new NotImplementedException();
        }
        //! Pops matrix from stack to current.
        public void Pop()
        {
            //Standard_ASSERT_RETURN(myStackHead != -1, "Matrix stack already empty when MatrixState.Pop() called.", );
            myCurrent = myStack[myStackHead--];
        }

        
        //! @return current matrix.
        public Graphic3d_Mat4 Current()
        {
            return myCurrent;
        }

    }
}