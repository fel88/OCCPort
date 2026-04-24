namespace OCCPort
{
    //! \brief A cache class for Bezier and B-spline curves.
    //!
    //! Defines all data, that can be cached on a span of a curve.
    //! The data should be recalculated in going from span to span.
    public class BSplCLib_Cache
    {
        BSplCLib_CacheParams myParams;                //!< cache parameters

        public void D0(double theParameter, ref gp_Pnt thePoint)
        {
            double aNewParameter = myParams.PeriodicNormalization(theParameter);
            //aNewParameter = (aNewParameter - myParams.SpanStart) / myParams.SpanLength;

            //double[] aPolesArray = ConvertArray(myPolesWeights);
            //double[] aPoint = new double[4];
            //int aDimension = myPolesWeights.RowLength(); // number of columns

            //PLib.NoDerivativeEvalPolynomial(aNewParameter, myParams.Degree,
            //                                 aDimension, myParams.Degree * aDimension,
            //                                 aPolesArray[0], aPoint[0]);

            //thePoint.SetCoord(aPoint[0], aPoint[1], aPoint[2]);
            //if (myIsRational)
            //    thePoint.ChangeCoord().Divide(aPoint[3]);
        }

    }
}