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
        public gp_XYZ Subtracted(gp_XYZ theOther)
        {
            return new gp_XYZ(x - theOther.x, y - theOther.y, z - theOther.z);
        }

        public static gp_XYZ operator -(gp_XYZ v, gp_XYZ y)
        {
            return v.Subtracted(y);
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
            x = -x;
            y = -y;
            z = -z;
        }

        internal void Add(object value)
        {
            throw new NotImplementedException();
        }

        internal void Multiply(gp_Mat theMatrix)
        {
            var aXresult = theMatrix.Value(1, 1) * x + theMatrix.Value(1, 2) * y + theMatrix.Value(1, 3) * z;
            var anYresult = theMatrix.Value(2, 1) * x + theMatrix.Value(2, 2) * y + theMatrix.Value(2, 3) * z;
            z = theMatrix.Value(3, 1) * x + theMatrix.Value(3, 2) * y + theMatrix.Value(3, 3) * z;
            x = aXresult;
            y = anYresult;
        }
        internal void Multiply(double theScalar)
        {
            x *= theScalar;
            y *= theScalar;
            z *= theScalar;
        }

        internal gp_XYZ Multiplied(gp_Mat theMatrix)
        {
            return new gp_XYZ(theMatrix.Value(1, 1) * x + theMatrix.Value(1, 2) * y + theMatrix.Value(1, 3) * z,
                   theMatrix.Value(2, 1) * x + theMatrix.Value(2, 2) * y + theMatrix.Value(2, 3) * z,
                   theMatrix.Value(3, 1) * x + theMatrix.Value(3, 2) * y + theMatrix.Value(3, 3) * z);
        }

        internal gp_XYZ Reversed()
        {
            return new gp_XYZ(-x, -y, -z);
        }

        internal gp_XYZ Normalized()
        {
            var aD = Modulus();
            //Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_XYZ::Normalized() - vector has zero norm");
            return new gp_XYZ(x / aD, y / aD, z / aD);
        }

        internal void CrossCross(gp_XYZ theCoord1, gp_XYZ theCoord2)
        {
            var aXresult = y * (theCoord1.x * theCoord2.y - theCoord1.y * theCoord2.x) -
                           z * (theCoord1.z * theCoord2.x - theCoord1.x * theCoord2.z);
            var anYresult = z * (theCoord1.y * theCoord2.z - theCoord1.z * theCoord2.y) -
                                      x * (theCoord1.x * theCoord2.y - theCoord1.y * theCoord2.x);
            z = x * (theCoord1.z * theCoord2.x - theCoord1.x * theCoord2.z) -
                y * (theCoord1.y * theCoord2.z - theCoord1.z * theCoord2.y);
            x = aXresult;
            y = anYresult;
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