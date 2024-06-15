using System;

namespace OCCPort
{
    public class BRepBuilderAPI_MakeShape : BRepBuilderAPI_Command
    {

        public TopoDS_Shape myShape;
        public TopoDS_Shape Shape()
        {
            if (!IsDone())
            {
                // the following is const cast away
                //((BRepBuilderAPI_MakeShape*)(void*)this)->Build();
                Build();
                Check();
            }
            return myShape;
        }

        
        


        //! This is  called by  Shape().  It does  nothing but
        //! may be redefined.
        public virtual void Build(Message_ProgressRange theRange = null)
        {
        }



    }
}