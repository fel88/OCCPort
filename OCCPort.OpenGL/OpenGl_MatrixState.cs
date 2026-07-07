using OCCPort.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OCCPort.OpenGL
{
    public class OpenGl_MatrixState<T>
    {

        public OpenGl_MatrixState()
        {
            myStack = new Stack<TKernel.NCollection_Mat4<float>>();
            

        }
        //! Pushes current matrix into stack.
        public void Push()
        {
            myStack.Push(myCurrent);

        }

        Stack<Graphic3d_Mat4> myStack = new Stack<TKernel.NCollection_Mat4<float>>();     //!< Collection used to maintenance matrix stack
        Graphic3d_Mat4 myCurrent = new TKernel.NCollection_Mat4<float>();   //!< Current matrix

        int myStackHead; //!< Index of stack head
                         //! Sets current matrix to identity.
        public void SetIdentity()
        {

            myCurrent = new Graphic3d_Mat4();
        }

        internal void SetCurrent(Graphic3d_Mat4 theNewCurrent)
        {
            myCurrent = theNewCurrent;

        }
        //! Pops matrix from stack to current.
        public void Pop()
        {
            if (myStack.Count == 0)
                //Exceptions.Standard_ASSERT_RETURN(myStackHead != -1, "Matrix stack already empty when MatrixState.Pop() called." );
                Exceptions.Standard_ASSERT_RETURN(myStack.Count > 0, "Matrix stack already empty when MatrixState.Pop() called.");
            myCurrent = myStack.Pop();            
        }


        //! @return current matrix.
        public Graphic3d_Mat4 Current()
        {
            return myCurrent;
        }

        internal Graphic3d_Mat4 ChangeCurrent()
        {
            return myCurrent;
        }
    }
}