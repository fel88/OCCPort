using System;
using System.Collections;
using System.Collections.Generic;

namespace OCCPort
{  
    //! Auxiliary class extending sequence iterator with index.
    internal class OpenGl_IndexedLayerIterator : IEnumerator<Graphic3d_Layer>
    {    
        public OpenGl_IndexedLayerIterator() { }
        public OpenGl_IndexedLayerIterator(List<Graphic3d_Layer> l)
        {           
            list = l;
        }
        int myIndex;
        //! Return index of current position.
       public int  Index()  { return myIndex; }
        public List<Graphic3d_Layer> list = new List<Graphic3d_Layer>();

        public Graphic3d_Layer Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        //! Move to the next position.
        public   void Next()        
        {
            //NCollection_List < Handle(Graphic3d_Layer) >::Iterator::Next();
            ++myIndex;
        }
        internal bool More()
        {
            return myIndex< list.Count; 
        }

        internal Graphic3d_Layer Value()
        {
            return list[myIndex];
        }

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            Next();
            return More();
        }

        public void Reset()
        {
            myIndex = 0;
        }
    }
}