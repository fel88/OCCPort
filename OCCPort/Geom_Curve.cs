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


    }
}