using OCCPort.Common;
using TKG3d;
using TKMath;

namespace TKGeomBase
{
    //! Projects elementary curves on a cylinder.
    public class ProjLib_Cylinder : ProjLib_Projector
    {
        public ProjLib_Cylinder(gp_Cylinder Cyl)
        {
            Init(Cyl);
        }

        void Init(gp_Cylinder Cyl)
        {
            myType = TKMath.GeomAbs_CurveType.GeomAbs_OtherCurve;
            myCylinder = Cyl;
            myIsPeriodic = false;
            isDone = false;
        }
        gp_Cylinder myCylinder;

        //=======================================================================
        //function : EvalPnt2d / EvalDir2d
        //purpose  : returns the Projected Pnt / Dir in the parametrization range
        //           of myPlane.
        //=======================================================================

        static gp_Pnt2d EvalPnt2d(gp_Pnt P, gp_Cylinder Cy)
        {
            gp_Vec OP = new(Cy.Location(), P);
            double X = OP.Dot(new gp_Vec(Cy.Position().XDirection()));
            double Y = OP.Dot(new gp_Vec(Cy.Position().YDirection()));
            double Z = OP.Dot(new gp_Vec(Cy.Position().Direction()));
            double U;

            if (Math.Abs(X) > Precision.PConfusion() ||
                 Math.Abs(Y) > Precision.PConfusion())
            {
                U = Math.Atan2(Y, X);
            }
            else
            {
                U = 0.0;
            }
            return new gp_Pnt2d(U, Z);
        }

        public override void Project(gp_Lin L)
        {
            // Check the line is parallel to the axis of cylinder.
            // In other cases, the projection is wrong.
            if (L.Direction().XYZ().CrossSquareMagnitude(myCylinder.Position().Direction().XYZ()) >
                Precision.Angular() * Precision.Angular())
                return;

            myType = GeomAbs_CurveType.GeomAbs_Line;

            gp_Pnt2d P2d = EvalPnt2d(L.Location(), myCylinder);
            if (P2d.X() < 0.0)
            {
                P2d.SetX(P2d.X() + 2 * Math.PI);
            }

            double Signe
              = L.Direction().Dot(myCylinder.Position().Direction());
            Signe = (Signe > 0.0) ? 1.0 : -1.0;
            gp_Dir2d D2d = new(0.0, Signe);

            myLin = new gp_Lin2d(P2d, D2d);
            isDone = true;
        }


    }
}
