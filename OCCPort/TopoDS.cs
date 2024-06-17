using System;

namespace OCCPort
{
    public class TopoDS
    {

        //! Casts shape S to the more specialized return type, Solid.
        //! Exceptions
        //! Standard_TypeMismatch if S cannot be cast to this return type.
        public static TopoDS_Solid Solid(TopoDS_Shape S)
        {
            if (S is TopoDS_Solid)
            {
                return S as TopoDS_Solid;
            }
            throw new ArgumentException("\"TopoDS::Solid\"");
            //Standard_TypeMismatch_Raise_if(TopoDS_Mismatch(S, TopAbs_SOLID), "TopoDS::Solid");
            //return *(TopoDS_Solid*)&S;
        }

        internal static TopoDS_Edge Edge(TopoDS_Shape theShape)
        {
            throw new NotImplementedException();
        }

        internal static TopoDS_Shell Shell(TopoDS_Shape myShape)
        {
            throw new NotImplementedException();
        }
    }
}