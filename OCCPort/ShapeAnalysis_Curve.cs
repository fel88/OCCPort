using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace OCCPort
{
    internal class ShapeAnalysis_Curve
    {

        //! Returns sample points which will serve as linearisation
        //! of the2d curve in range (first, last)
        //! The distribution of sample points is consystent with
        //! what is used by BRepTopAdaptor_FClass2d
        public static bool GetSamplePoints(Geom2d_Curve curve, double first, double last, TColgp_SequenceOfPnt2d seq)
        {
            Geom2dAdaptor_Curve C = new Geom2dAdaptor_Curve(curve, first, last);
            int nbs = Geom2dInt_Geom2dCurveTool.NbSamples(C);
            //-- Attention aux bsplines rationnelles de degree 3. (bouts de cercles entre autres)
            if (nbs > 2) nbs *= 4;
            double step = (last - first) / (double)(nbs - 1);
            for (int i = 0; i < nbs - 1; ++i)
                seq.Append(C.Value(first + step * i));
            seq.Append(C.Value(last));
            return true;
        }
    }
}