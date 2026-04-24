namespace OCCPort
{
    //! Simple structure containing parameters describing parameterization
    //! of a B-spline curve or a surface in one direction (U or V),
    //! and data of the current span for its caching
    struct BSplCLib_CacheParams
    {
        int Degree;      ///< degree of Bezier/B-spline
        bool IsPeriodic;  ///< true of the B-spline is periodic
        double FirstParameter; ///< first valid parameter
        double LastParameter;  ///< last valid parameter


        //! Normalizes the parameter for periodic B-splines
        //! \param theParameter the value to be normalized into the knots array
        public double PeriodicNormalization(double theParameter)
        {
            if (IsPeriodic)
            {
                if (theParameter < FirstParameter)
                {
                    double aPeriod = LastParameter - FirstParameter;
                    double  aScale =  Standard_Real. IntegerPart((FirstParameter - theParameter) / aPeriod);
                    return theParameter + aPeriod * (aScale + 1.0);
                }
                if (theParameter > LastParameter)
                {
                    double aPeriod = LastParameter - FirstParameter;
                    double  aScale = Standard_Real.IntegerPart((theParameter - LastParameter) / aPeriod);
                    return theParameter - aPeriod * (aScale + 1.0);
                }
            }
            return theParameter;
        }
    }
}