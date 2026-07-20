using OCCPort;
using TKMath;

namespace TKGeomBase
{
    //! Projects elementary curves on a plane.
    public class ProjLib_Plane : ProjLib_Projector
    {
        //! Projection on the plane <Pl>.
        public ProjLib_Plane(gp_Pln Pl)
        {
            Init(Pl);

        }
        gp_Pln myPlane;
        public static gp_Pnt2d EvalPnt2d(gp_Pnt P,
                                  gp_Pln Pl)
        {
            gp_Vec OP = new gp_Vec(Pl.Location(), P);
            return new gp_Pnt2d(OP.Dot(new gp_Vec(Pl.Position().XDirection())),
                         OP.Dot(new gp_Vec(Pl.Position().YDirection())));
        }
        public override void Project(gp_Lin L)
        {
            myType = GeomAbs_CurveType.GeomAbs_Line;
            myLin = new gp_Lin2d(EvalPnt2d(L.Location(), myPlane),
                      EvalDir2d(L.Direction(), myPlane));
            isDone = true;
        }

        public override void Project(gp_Circ C)
        {            
            myType = GeomAbs_CurveType.GeomAbs_Circle;

            gp_Pnt2d P2d = EvalPnt2d(C.Location(), myPlane);
            gp_Dir2d X2d = EvalDir2d(C.Position().XDirection(), myPlane);
            gp_Dir2d Y2d = EvalDir2d(C.Position().YDirection(), myPlane);
            gp_Ax22d Ax = new(P2d, X2d, Y2d);

            myCirc = new gp_Circ2d(Ax, C.Radius());
            myIsPeriodic = true;
            isDone = true;
        }

        public static gp_Dir2d EvalDir2d(gp_Dir D, gp_Pln Pl)
        {
            return new gp_Dir2d(D.Dot(Pl.Position().XDirection()),
                         D.Dot(Pl.Position().YDirection()));
        }

        public void Init(gp_Pln Pl)
        {
            myType = GeomAbs_CurveType.GeomAbs_OtherCurve;
            isDone = false;
            myIsPeriodic = false;
            myPlane = Pl;
        }
    }
}
