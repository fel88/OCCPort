using System;

namespace OCCPort
{
    //! Root class for projection algorithms, stores the result.
    public class ProjLib_Projector
    {
        public ProjLib_Projector()
        {
            myIsPeriodic = (false);
            isDone = false;
            myType = GeomAbs_CurveType.GeomAbs_BSplineCurve;
        }
        public gp_Lin2d Line()
        {
            if (myType != GeomAbs_CurveType.GeomAbs_Line)
                throw new Standard_NoSuchObject("ProjLib_Projector::Line");
            return myLin;
        }

        public void SetType(GeomAbs_CurveType Type)
        {
            myType = Type;
        }
        public bool IsDone()
        {
            return isDone;
        }
        protected GeomAbs_CurveType myType;
        protected gp_Lin2d myLin;
        protected bool myIsPeriodic;
        protected bool isDone;
        public GeomAbs_CurveType _GetType()
        {
            return myType;
        }


        public virtual void Project(gp_Lin l)
        {
            myType = GeomAbs_CurveType.GeomAbs_OtherCurve;
        }

        internal void Done()
        {
            isDone = true;
        }
    }

}