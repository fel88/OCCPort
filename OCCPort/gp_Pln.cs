using OpenTK.Audio.OpenAL;
using System;
using System.Threading;

namespace OCCPort
{
    public class gp_Pln
    {

        //! Returns the local coordinate system of the plane .
        public gp_Ax3 Position() { return pos; }

        private gp_Ax3 pos;

        //! Returns the plane's normal Axis.
        public gp_Ax1 Axis() { return pos.Axis(); }

        //! Transforms a plane with the transformation theT from class Trsf.
        //! The transformation is performed on the "Location"
        //! point, on the "XAxis" and the "YAxis".
        //! The resulting normal direction is the cross product between
        //! the "XDirection" and the "YDirection" after transformation.
        public gp_Pln Transformed(gp_Trsf theT)
        {
            gp_Pln aPl = this;
            aPl.pos.Transform(theT);
            return aPl;
        }
        //! The coordinate system of the plane is defined with the axis
        //! placement theA3.
        //! The "Direction" of theA3 defines the normal to the plane.
        //! The "Location" of theA3 defines the location (origin) of the plane.
        //! The "XDirection" and "YDirection" of theA3 define the "XAxis" and
        //! the "YAxis" of the plane used to parametrize the plane.
        public gp_Pln(gp_Ax3 theA3)

        {
            pos = (theA3);
        }

        public gp_Pln(gp_Pnt P, gp_Dir V)
        {
            double A = V.X();
            double B = V.Y();
            double C = V.Z();
            double Aabs = A;
            if (Aabs < 0) Aabs = -Aabs;
            double Babs = B;
            if (Babs < 0) Babs = -Babs;
            double Cabs = C;
            if (Cabs < 0) Cabs = -Cabs;

            //  pour determiner l'axe X :
            //  on dit que le produit scalaire Vx.V = 0. 
            //  et on recherche le max(A,B,C) pour faire la division.
            //  l'une des coordonnees du vecteur est nulle. 

            if (Babs <= Aabs && Babs <= Cabs)
            {
                if (Aabs > Cabs) pos = new gp_Ax3(P, V, new gp_Dir(-C, 0.0, A));
                else pos = new gp_Ax3(P, V, new gp_Dir(C, 0.0, -A));
            }
            else if (Aabs <= Babs && Aabs <= Cabs)
            {
                if (Babs > Cabs) pos = new gp_Ax3(P, V, new gp_Dir(0.0, -C, B));
                else pos = new gp_Ax3(P, V, new gp_Dir(0.0, C, -B));
            }
            else
            {
                if (Aabs > Babs) pos = new gp_Ax3(P, V, new gp_Dir(-B, A, 0.0));
                else pos = new gp_Ax3(P, V, new gp_Dir(B, -A, 0.0));
            }
        }



        //! Returns the X axis of the plane.
        public gp_Ax1 XAxis()
        {
            return new gp_Ax1(pos.Location(), pos.XDirection());
        }

        //! Returns the Y axis  of the plane.
        public gp_Ax1 YAxis()
        {
            return new gp_Ax1(pos.Location(), pos.YDirection());
        }

        //! Computes the distance between <me> and the point <theP>.
        public double Distance(gp_Pnt theP)
        {

            {
                gp_Pnt aLoc = pos.Location();
                gp_Dir aDir = pos.Direction();
                double aD = (aDir.X() * (theP.X() - aLoc.X()) +
                                    aDir.Y() * (theP.Y() - aLoc.Y()) +
                                    aDir.Z() * (theP.Z() - aLoc.Z()));
                if (aD < 0)
                {
                    aD = -aD;
                }
                return aD;
            }
        }
    }
}