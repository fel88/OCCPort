namespace OCCPort
{
    //! This class must be provided by the user to use the approximation algorithm FittingCurve.
    public abstract class AppCont_Function
    {

        public AppCont_Function()
        {
            myNbPnt = -1;
            myNbPnt2d = -1;
        }

        protected int myNbPnt;
        protected int myNbPnt2d;

        //! Return information about peridicity in output paramateters space. 
        //! @param theDimIdx Defines index in output parameters space. 1 <= theDimIdx <= 3 * myNbPnt + 2 * myNbPnt2d.
        public virtual void PeriodInformation(int theDimIdx,
                                        out bool IsPeriodic,
                                  out double thePeriod)
        {
            IsPeriodic = false;
            thePeriod = 0.0;
        }


        //! Returns the point at parameter <theU>.
        public abstract bool Value(double theU,
                                            NCollection_Array1<gp_Pnt2d> thePnt2d,
                                      NCollection_Array1<gp_Pnt> thePnt);

        //! Returns the derivative at parameter <theU>.
        public abstract bool D1(double theU,
                                     NCollection_Array1<gp_Vec2d> theVec2d,
                               NCollection_Array1<gp_Vec> theVec);

        //! Get number of 3d and 2d points returned by "Value" and "D1" functions.
        public void GetNumberOfPoints(ref int theNbPnt, ref int theNbPnt2d)
        {
            theNbPnt = myNbPnt;
            theNbPnt2d = myNbPnt2d;
        }

        //! Returns the first parameter of the function.
        public abstract double FirstParameter();

        //! Returns the last parameter of the function.
        public abstract double LastParameter();

        //! Get number of 3d points returned by "Value" and "D1" functions.
        public int GetNbOf3dPoints()
        {
            return myNbPnt;
        }

        //! Get number of 2d points returned by "Value" and "D1" functions.
        public int GetNbOf2dPoints()
        {
            return myNbPnt2d;
        }
    }
}