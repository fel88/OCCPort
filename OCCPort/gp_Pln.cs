using System;

namespace OCCPort.Tester
{
    internal class gp_Pln
    {

        //! Returns the local coordinate system of the plane .
        public gp_Ax3 Position() { return pos; }

        private gp_Ax3 pos;

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
                else pos =new  gp_Ax3(P, V, new gp_Dir(C, 0.0, -A));
            }
            else if (Aabs <= Babs && Aabs <= Cabs)
            {
                if (Babs > Cabs) pos = new gp_Ax3(P, V, new gp_Dir(0.0, -C, B));
                else pos =new gp_Ax3(P, V, new gp_Dir(0.0, C, -B));
            }
            else
            {
                if (Aabs > Babs) pos =new gp_Ax3(P, V, new gp_Dir(-B, A, 0.0));
                else pos = new gp_Ax3(P, V, new gp_Dir(B, -A, 0.0));
            }
        }


        internal gp_Ax2 XAxis()
        {
            throw new NotImplementedException();
        }

        internal gp_Ax2 YAxis()
        {
            throw new NotImplementedException();
        }
    }
}