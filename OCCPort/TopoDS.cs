using System;

namespace OCCPort.Tester
{
    internal class TopoDS
    {

        //! Casts shape S to the more specialized return type, Solid.
        //! Exceptions
        //! Standard_TypeMismatch if S cannot be cast to this return type.
        public static TopoDS_Solid Solid(TopoDS_Shape S)
        {
            return null;
        }

        internal static TopoDS_Shell Shell(TopoDS_Shape myShape)
        {
            throw new NotImplementedException();
        }
    }
}