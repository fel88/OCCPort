namespace OCCPort
{
    public abstract class Geom_Curve: Geom_Geometry
    {  //! Warnings :
       //! It can be RealFirst from package Standard
       //! if the curve is infinite
        public abstract double FirstParameter();

        //! Returns the value of the last parameter.
        //! Warnings :
        //! It can be RealLast from package Standard
        //! if the curve is infinite
        public abstract double LastParameter();
        //! Returns in P the point of parameter U.
        //! If the curve is periodic  then the returned point is P(U) with
        //! U = Ustart + (U - Uend)  where Ustart and Uend are the
        //! parametric bounds of the curve.
        //!
        //! Raised only for the "OffsetCurve" if it is not possible to
        //! compute the current point. For example when the first
        //! derivative on the basis curve and the offset direction
        //! are parallel.
        public abstract void D0(double U, ref gp_Pnt P);


    }
}