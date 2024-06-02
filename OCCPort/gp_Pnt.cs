using System;

namespace OCCPort
{
    public class gp_Pnt
    {
        public gp_XYZ coord;


        //! Creates a point with zero coordinates.
        public gp_Pnt() { }

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

        internal double Distance(gp_Pnt theCenter)
        {
            throw new NotImplementedException();
        }

        internal void Transform(gp_Trsf theTrsf)
        {
            throw new NotImplementedException();
        }
    }
}
