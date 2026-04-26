namespace OCCPort
{
    //! Default implementation of curve data model entity.
    public class BRepMeshData_Curve : IMeshData_Curve
    {
        public void Clear(bool isKeepEndPoints)
        {
            if (!isKeepEndPoints)
            {
                myPoints.Clear();
                myParameters.Clear();
            }
            else if (ParametersNb() > 2)
            {
                myPoints.erase( 1,  (myPoints.size() - 1));
                myParameters.erase(1, (myParameters.size() - 1));
            }
        }

        public double GetParameter(int theIndex)
        {
            return myParameters[theIndex];

        }

        SequenceOfPnt myPoints;
        SequenceOfReal myParameters;

        public BRepMeshData_Curve()
        {
            myParameters = new SequenceOfReal();
            myPoints = new SequenceOfPnt();
        }

        public gp_Pnt GetPoint(int theIndex)
        {
            return myPoints[theIndex];

        }

        public int ParametersNb()
        {
            return myParameters.Count;

        }

        public void AddPoint(gp_Pnt thePoint, double theParamOnPCurve)
        {
            myPoints.Add(thePoint);
            myParameters.Add(theParamOnPCurve);
        }

        public void InsertPoint(int thePosition, gp_Pnt thePoint, double theParamOnPCurve)
        {
            myPoints.Insert( thePosition, thePoint);
            myParameters.Insert( thePosition, theParamOnPCurve);
        }
    }
}