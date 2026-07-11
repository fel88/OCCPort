using OCCPort;
using TKBRep;
using TKMath;
using TKTopAlgo;

namespace TKPrim
{
    //! Describes functions to build parallelepiped boxes.
    //! A MakeBox object provides a framework for:
    //! -   defining the construction of a box,
    //! -   implementing the construction algorithm, and
    //! -   consulting the result.
    //! Constructs a box such that its sides are parallel to the axes of
    //! -   the global coordinate system, or
    //! -   the local coordinate system Axis. and
    //! -   with a corner at (0, 0, 0) and of size (dx, dy, dz), or
    //! -   with a corner at point P and of size (dx, dy, dz), or
    //! -   with corners at points P1 and P2.
    //! Exceptions
    //! Standard_DomainError if: dx, dy, dz are less than or equal to
    //! Precision::Confusion(), or
    //! -   the vector joining the points P1 and P2 has a
    //! component projected onto the global coordinate
    //! system less than or equal to Precision::Confusion().
    //! In these cases, the box would be flat.
    public class BRepPrimAPI_MakeBox : BRepBuilderAPI_MakeShape
    {
        BRepPrim_Wedge myWedge;
        public BRepPrimAPI_MakeBox()
        {
            myShape = new TopoDS_Solid();
        }
        public gp_Pnt pmin(gp_Pnt p,
            double dx,
            double dy,
            double dz)
        {
            gp_Pnt P = p;
            if (dx < 0) P.SetX(P.X() + dx);
            if (dy < 0) P.SetY(P.Y() + dy);
            if (dz < 0) P.SetZ(P.Z() + dz);
            return P;
        }

        gp_Pnt pmin(gp_Pnt p1, gp_Pnt p2)
        {
            return new gp_Pnt(Math.Min(p1.X(), p2.X()), Math.Min(p1.Y(), p2.Y()), Math.Min(p1.Z(), p2.Z()));
        }

        //=======================================================================
        //function : Init
        //purpose  :
        //=======================================================================
        public void Init(double theDX, double theDY, double theDZ)
        {
            myWedge = new BRepPrim_Wedge(new gp_Ax2(pmin(new gp_Pnt(0, 0, 0), theDX, theDY, theDZ), new gp_Dir(0, 0, 1), new gp_Dir(1, 0, 0)),
                                     Math.Abs(theDX), Math.Abs(theDY), Math.Abs(theDZ));
        }


        public BRepPrimAPI_MakeBox(gp_Pnt P1, gp_Pnt P2)
        {
            myWedge = new BRepPrim_Wedge(new gp_Ax2(pmin(P1, P2), new gp_Dir(0, 0, 1), new gp_Dir(1, 0, 0)),
                Math.Abs(P2.X() - P1.X()),
                Math.Abs(P2.Y() - P1.Y()),
                Math.Abs(P2.Z() - P1.Z()));
            myShape = new TopoDS_Solid();
        }

        public TopoDS_Shell Shell()
        {
            myShape = myWedge.Shell();
            Done();
            return TopoDS.Shell(myShape);
        }


        public void Build()
        {
            Solid();
        }
        //=======================================================================
        //function : Solid
        //purpose  : 
        //=======================================================================

        public TopoDS_Solid Solid()
        {
            BRep_Builder B = new BRep_Builder();
            B.MakeSolid(TopoDS.Solid(myShape));
            B.Add(myShape, myWedge.Shell());
            Done();
            return TopoDS.Solid(myShape);
        }


    }


}
