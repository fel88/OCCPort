using System;
using System.Collections;
using System.Collections.Generic;
using TKernel;
using TKService;

namespace OCCPort
{  
    //! Auxiliary class extending sequence iterator with index.
    internal class OpenGl_IndexedLayerIterator : NCollection_List<Graphic3d_Layer>.Iterator
    {    
        
        public OpenGl_IndexedLayerIterator(NCollection_List<Graphic3d_Layer> theSeq):base(theSeq)
        {
            myIndex = 1;
            
        }
        public int myIndex;
        //! Return index of current position.
       public int  Index()  { return myIndex; }
        

        public Graphic3d_Layer Current => throw new NotImplementedException();

        

        //! Move to the next position.
        //public   void Next()        
        //{
        //    //NCollection_List < Handle(Graphic3d_Layer) >::Iterator::Next();
        //    ++myIndex;
        //}
        //internal bool More()
        //{
        //    return myIndex< list.Count; 
        //}

        //internal Graphic3d_Layer Value()
        //{
        //    return list[myIndex];
        //}

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