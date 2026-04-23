using System;

namespace OCCPort
{
    //! Defines a non-persistent vector in 3D space.

    public struct gp_Vec
    {
        gp_XYZ coord;
        //! Multiplies a vector by a scalar
        public gp_Vec Multiplied(double theScalar)
        {
            gp_Vec aV = this;
            aV.coord.Multiply(theScalar);
            return aV;
        }
        public void Rotate(gp_Ax1 theA1, double theAng)
        {
            gp_Trsf aT = new gp_Trsf();
            aT.SetRotation(theA1, theAng);
            coord.Multiply(aT.VectorialPart());
        }

        //! Returns True if Angle(<me>, theOther) <= theAngularTolerance or
        //! PI - Angle(<me>, theOther) <= theAngularTolerance
        //! This definition means that two parallel vectors cannot define
        //! a plane but two vectors with opposite directions are considered
        //! as parallel. Raises VectorWithNullMagnitude if <me>.Magnitude() <= Resolution or
        //! Other.Magnitude() <= Resolution from gp
        public bool IsParallel(gp_Vec theOther, double theAngularTolerance)
        {
            double anAng = Angle(theOther);
            return anAng <= theAngularTolerance || Math.PI - anAng <= theAngularTolerance;
        }
        public double Angle(gp_Vec theOther)
        {
            gp_VectorWithNullMagnitude_Raise_if(coord.Modulus() <= gp.Resolution() ||
                                                 theOther.coord.Modulus() <= gp.Resolution(), " ");
            return (new gp_Dir(coord)).Angle(theOther.To_gp_Dir());
        }

        private void gp_VectorWithNullMagnitude_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        //! computes the scalar product
        public double Dot(gp_Vec theOther) { return coord.Dot(theOther.coord); }

        //! computes the scalar product
        public double Dot(gp_XYZ theOther) { return coord.Dot(theOther); }



        public gp_XYZ XYZ()
        {
            return coord;
        }

        public gp_Vec Added(gp_Vec theOther)
        {
            gp_Vec aV = this;
            aV.coord.Add(theOther.coord);
            return aV;
        }

        public static gp_Vec operator *(gp_Vec v, double theScalar)
        {
            return v.Multiplied(theScalar);
        }
        public static gp_Vec operator ^(gp_Vec v, gp_Vec theRight)
        {
            return v.Crossed(theRight);
        }  //! computes the cross product between two vectors
        public gp_Vec Crossed(gp_Vec theRight)
        {
            this.coord.Cross(theRight.coord);
            return this;
        }
        public static gp_Vec operator *(double theScalar, gp_Vec v)
        {
            return v.Multiplied(theScalar);
        }
        public static gp_Vec operator -(gp_Vec f)
        {
            return f.Reversed();
        }
        //! Reverses the direction of a vector
        public gp_Vec Reversed()
        {
            this.coord.Reverse();
            return this;
        }

        public static gp_Vec operator +(gp_Vec v, gp_Vec theOther)
        {
            return v.Added(theOther);
        }


        //! Creates a vector with a triplet of coordinates.
        public gp_Vec(gp_XYZ theCoord)
        {
            coord = (theCoord);
        }


        //! Computes the magnitude of this vector.
        public double Magnitude() { return coord.Modulus(); }

        //! normalizes a vector
        //! Raises an exception if the magnitude of the vector is
        //! lower or equal to Resolution from gp.
        public void Normalize()
        {
            double aD = coord.Modulus();

            if (aD <= gp.Resolution())
                throw new Exception("gp_Vec::Normalize() - vector has zero norm");

            coord.Divide(aD);
        }

        public gp_Vec(gp_Dir theV)
        {
            coord = theV.XYZ();
        }

        public gp_Vec(gp_Pnt theP1, gp_Pnt theP2) : this()
        {

            coord = theP2.XYZ().Subtracted(theP1.XYZ());
        }

    }
}