using System;
using System.Collections;
using System.Collections.Generic;
using TKernel;
using TKService;

namespace OCCPort
{
    //! Stack of references to existing layers of predefined maximum size.

    internal class OpenGl_LayerStack
    {
        public class iterator
        {
            public NCollection_Array1<OpenGl_Layer> collection;
            public int position;
        }

        //! Returns iterator to the origin of the stack.
        public Stack<OpenGl_Layer> Origin() { return stack.Count == 0 ? null : stack; }

        //! Returns iterator to the back of the stack (after last item added).
        public OpenGl_Layer Back() { return stack.Peek(); }

        //! Returns true if nothing has been pushed into the stack.
        public bool IsEmpty() { return stack.Count == 0; }

        //! Clear stack.
        public void Clear()
        {
            stack = new Stack<OpenGl_Layer>();
            //myStackSpace.Init(null);
            //myBackPtr = new iterator() { collection = myStackSpace };
        }

        Stack<OpenGl_Layer> stack = new Stack<OpenGl_Layer>();
        
        internal void Push(OpenGl_Layer aLayer)
        {
            stack.Push(aLayer);
            //(*myBackPtr++) = theLayer;

        }

        //NCollection_Array1<OpenGl_Layer> myStackSpace=new NCollection_Array1<OpenGl_Layer> 
        //NCollection_Array1< Graphic3d_Layer>.iterator myBackPtr;
        //iterator myBackPtr;


    }
}