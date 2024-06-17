using System;

namespace OCCPort
{
    public struct gp_Pnt
    {
        public gp_XYZ coord;


        //! Creates a point with zero coordinates.
        //public gp_Pnt() { }

        //! Creates a point from a XYZ object.
        public gp_Pnt(gp_XYZ theCoord)

        {
            coord = new gp_XYZ(theCoord);
        }


        public gp_Pnt(double x, double y, double z)
        {
            this.coord = new gp_XYZ(x, y, z);
        }

        //! For this point, returns its X coordinate.
        public double X() { return coord.X(); }

        //! For this point, returns its Y coordinate.
        public double Y() { return coord.Y(); }

        //! For this point, returns its Z coordinate.
        public double Z() { return coord.Z(); }

        //! For this point, returns its three coordinates as a XYZ object.
        public gp_XYZ XYZ() { return coord; }

        internal bool IsEqual(gp_Pnt theEye, double v)
        {
            return coord.IsEqual(theEye.coord, v);
        }

        internal double Distance(gp_Pnt theOther)
        {
            double aD = 0, aDD;
            gp_XYZ aXYZ = theOther.coord;
            aDD = coord.X(); aDD -= aXYZ.X(); aDD *= aDD; aD += aDD;
            aDD = coord.Y(); aDD -= aXYZ.Y(); aDD *= aDD; aD += aDD;
            aDD = coord.Z(); aDD -= aXYZ.Z(); aDD *= aDD; aD += aDD;
            return Math.Sqrt(aD);
        }

        internal void Transform(gp_Trsf T)
        {
            if (T.Form() == gp_TrsfForm.gp_Identity) { }
            else if (T.Form() == gp_TrsfForm.gp_Translation) { coord.Add(T.TranslationPart()); }
            else if (T.Form() == gp_TrsfForm.gp_Scale)
            {
                coord.Multiply(T.ScaleFactor());
                coord.Add(T.TranslationPart());
            }
            else if (T.Form() == gp_TrsfForm.gp_PntMirror)
            {
                coord.Reverse();
                coord.Add(T.TranslationPart());
            }
            else { T.Transforms(ref coord); }
        }
        public void SetX(double v)
        {
            throw new NotImplementedException();
        }

        public void SetY(double v)
        {
            throw new NotImplementedException();
        }

        public void SetZ(double v)
        {
            throw new NotImplementedException();
        }

        public void Translate(gp_Vec theV)
        {            
            coord.Add(theV.XYZ());
        }

        public gp_Pnt Transformed(gp_Trsf theT)
        {

            gp_Pnt aP = this;
            aP.Transform(theT);
            return aP;


        }
    }
}
