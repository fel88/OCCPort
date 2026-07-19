using TKBRep;
using TKernel;
using TKTopAlgo;

namespace TKPrim
{
    //! The abstract class MakeOneAxis is the root class of
    //! algorithms used to construct rotational primitives.
    public abstract class BRepPrimAPI_MakeOneAxis : BRepBuilderAPI_MakeShape
    {
        public override void Build(Message_ProgressRange theRange)
        {
            BRep_Builder B = new BRep_Builder();
            B.MakeSolid(TopoDS.Solid(myShape));
            B.Add(myShape, ((BRepPrim_OneAxis)OneAxis()).Shell());
            Done();
        }
        //! The inherited commands should provide the algorithm.
        //! Returned as a pointer.
        
        public abstract object OneAxis();
  
    }
}
