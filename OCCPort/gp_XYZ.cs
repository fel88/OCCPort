using System;
using System.ComponentModel;

namespace OCCPort
{
    public class gp_XYZ
    {
        private double x;
        private double y;
        private double z;

        //! @code
        //! <me>.X() = <me>.X() + theOther.X()
        //! <me>.Y() = <me>.Y() + theOther.Y()
        //! <me>.Z() = <me>.Z() + theOther.Z()
        //! @endcode
        public void Add(gp_XYZ theOther)
        {
            x += theOther.x;
            y += theOther.y;
            z += theOther.z;
        }


        public bool IsEqual(gp_XYZ Other,
                          double Tolerance)
        {
            double val;
            val = x - Other.x;
            if (val < 0) val = -val;
            if (val > Tolerance) return false;
            val = y - Other.y;
            if (val < 0) val = -val;
            if (val > Tolerance) return false;
            val = z - Other.z;
            if (val < 0) val = -val;
            if (val > Tolerance) return false;
            return true;
        }

        //! Returns the X coordinate
        public double X() { return x; }

        //! Returns the Y coordinate
        public double Y() { return y; }

        //! Returns the Z coordinate
        public double Z() { return z; }

        public static gp_XYZ operator *(gp_XYZ v, double y)
        {
            return v.Multiplied(y);
        }

        //! @code
        //! new.X() = <me>.X() + theOther.X()
        //! new.Y() = <me>.Y() + theOther.Y()
        //! new.Z() = <me>.Z() + theOther.Z()
        //! @endcode
        public gp_XYZ Added(gp_XYZ theOther)
        {
            return new gp_XYZ(x + theOther.x, y + theOther.y, z + theOther.z);
        }

        public static gp_XYZ operator +(gp_XYZ v, gp_XYZ y)
        {
            return v.Added(y);
        }
        public static gp_XYZ operator -(gp_XYZ v, gp_XYZ y)
        {
            throw new NotImplementedException();
            //return v.Added(y);
        }
        //! @code
        //! New.X() = <me>.X() * theScalar;
        //! New.Y() = <me>.Y() * theScalar;
        //! New.Z() = <me>.Z() * theScalar;
        //! @endcode
        public gp_XYZ Multiplied(double theScalar)
        {
            return new gp_XYZ(x * theScalar, y * theScalar, z * theScalar);
        }

        //! computes the scalar product between <me> and theOther
        public double Dot(gp_XYZ theOther)
        {
            return (x * theOther.x + y * theOther.y + z * theOther.z);
        }
        //! @code
        //! new.X() = <me>.Y() * theOther.Z() - <me>.Z() * theOther.Y()
        //! new.Y() = <me>.Z() * theOther.X() - <me>.X() * theOther.Z()
        //! new.Z() = <me>.X() * theOther.Y() - <me>.Y() * theOther.X()
        //! @endcode
        public gp_XYZ Crossed(gp_XYZ theOther)
        {
            return new gp_XYZ(y * theOther.z - z * theOther.y,
                           z * theOther.x - x * theOther.z,
                           x * theOther.y - y * theOther.x);
        }

        //! computes Sqrt (X*X + Y*Y + Z*Z) where X, Y and Z are the three coordinates of this XYZ object.
        public double Modulus()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        internal void Cross(gp_XYZ theRight)
        {
            double aXresult = y * theRight.z - z * theRight.y;
            double aYresult = z * theRight.x - x * theRight.z;
            z = x * theRight.y - y * theRight.x;
            x = aXresult;
            y = aYresult;
        }

        internal void Divide(double theScalar)
        {
            x /= theScalar;
            y /= theScalar;
            z /= theScalar;
        }

        internal void Reverse()
        {
            throw new NotImplementedException();
        }

        internal void Add(object value)
        {
            throw new NotImplementedException();
        }

        internal void Multiply(gp_Mat matrix)
        {
            throw new NotImplementedException();
        }
        internal void Multiply(double theScalar)
        {
            x *= theScalar;
            y *= theScalar;
            z *= theScalar;
        }

        internal gp_XYZ Multiplied(gp_Mat matrix)
        {
            throw new NotImplementedException();
        }

        internal gp_XYZ Reversed()
        {
            throw new NotImplementedException();
        }

        internal gp_XYZ Normalized()
        {
            throw new NotImplementedException();
        }

        public gp_XYZ(double v1, double v2, double v3)
        {
            this.x = v1;
            this.y = v2;
            this.z = v3;
        }
        public gp_XYZ(gp_XYZ v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }
    }
}