using System;

namespace OCCPort
{
    public abstract class Geom2d_Curve : Geom2d_Geometry
    {

        //! Returns the value of the first parameter.
        //! Warnings :
        //! It can be RealFirst or RealLast from package Standard
        //! if the curve is infinite
        public abstract double FirstParameter();

        //! Value of the last parameter.
        //! Warnings :
        //! It can be RealFirst or RealLast from package Standard
        //! if the curve is infinite

        public abstract double LastParameter();

    }
}