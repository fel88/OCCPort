using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography;

namespace OCCPort
{
    //! This package provides an implementation of algorithms to do
    //! the conversion between equivalent geometric entities from
    //! package Geom2d.
    //! It gives the possibility :
    //! . to obtain the B-spline representation of bounded curves.
    //! . to split a B-spline curve into several B-spline curves
    //! with some constraints of continuity,
    //! . to convert a B-spline curve into several Bezier curves
    //! or surfaces.
    //! All the geometric entities used in this package are bounded.
    //! References :
    //! . Generating the Bezier Points of B-spline curves and surfaces
    //! (Wolfgang Bohm) CAGD volume 13 number 6 november 1981
    //! . On NURBS: A Survey  (Leslie Piegl) IEEE Computer Graphics and
    //! Application January 1991
    //! . Curve and surface construction using rational B-splines
    //! (Leslie Piegl and Wayne Tiller) CAD Volume 19 number 9 november
    //! 1987
    //! . A survey of curve and surface methods in CAGD (Wolfgang BOHM)
    //! CAGD 1 1984
    public class Geom2dConvert
    {

        //! This function converts a non infinite curve from
        //! Geom into a  B-spline curve.  C must  be  an ellipse or a
        //! circle or a trimmed conic  or a trimmed  line or a Bezier
        //! curve or a trimmed  Bezier curve or a  BSpline curve or  a
        //! trimmed BSpline   curve  or an  Offset  curve or a  trimmed
        //! Offset curve.
        //! The returned B-spline is not periodic except if C is a
        //! Circle or an Ellipse.
        //! ParameterisationType applies only if the curve is a Circle
        //! or an ellipse :
        //! TgtThetaOver2,
        //! TgtThetaOver2_1,
        //! TgtThetaOver2_2,
        //! TgtThetaOver2_3,
        //! TgtThetaOver2_4,
        //! Purpose: this is the classical rational parameterisation
        //! 2
        //! 1 - t
        //! cos(theta) = ------
        //! 2
        //! 1 + t
        //!
        //! 2t
        //! sin(theta) = ------
        //! 2
        //! 1 + t
        //!
        //! t = tan (theta/2)
        //!
        //! with TgtThetaOver2  the routine will compute the number of spans
        //! using the rule num_spans = [ (ULast - UFirst) / 1.2 ] + 1
        //! with TgtThetaOver2_N, N  spans will be forced: an error will
        //! be raized if (ULast - UFirst) >= PI and N = 1,
        //! ULast - UFirst >= 2 PI and N = 2
        //!
        //! QuasiAngular,
        //! here t is a rational function that approximates
        //! theta ----> tan(theta/2).
        //! Nevetheless the composing with above function yields exact
        //! functions whose square sum up to 1
        //! RationalC1 ;
        //! t is replaced by a polynomial function of u so as to grant
        //! C1 contiuity across knots.
        //! Exceptions
        //! Standard_DomainError if the curve C is infinite.
        //! Standard_ConstructionError:
        //! -   if C is a complete circle or ellipse, and if
        //! Parameterisation is not equal to
        //! Convert_TgtThetaOver2 or to Convert_RationalC1, or
        //! -   if C is a trimmed circle or ellipse and if
        //! Parameterisation is equal to
        //! Convert_TgtThetaOver2_1 and if U2 - U1 >
        //! 0.9999 * Pi where U1 and U2 are
        //! respectively the first and the last parameters of the
        //! trimmed curve (this method of parameterization
        //! cannot be used to convert a half-circle or a
        //! half-ellipse, for example), or
        //! -   if C is a trimmed circle or ellipse and
        //! Parameterisation is equal to
        //! Convert_TgtThetaOver2_2 and U2 - U1 >
        //! 1.9999 * Pi where U1 and U2 are
        //! respectively the first and the last parameters of the
        //! trimmed curve (this method of parameterization
        //! cannot be used to convert a quasi-complete circle or ellipse).
        public static Geom2d_BSplineCurve CurveToBSplineCurve(Geom2d_Curve C,
                                                                           Convert_ParameterisationType Parameterisation = Convert_ParameterisationType.Convert_TgtThetaOver2)
        {

            Geom2d_BSplineCurve TheCurve;

            if (C is Geom2d_TrimmedCurve)
            {
                Geom2d_Curve Curv;
                Geom2d_TrimmedCurve Ctrim = (Geom2d_TrimmedCurve)C;
                Curv = Ctrim.BasisCurve();
                double U1 = Ctrim.FirstParameter();
                double U2 = Ctrim.LastParameter();

                // Si la courbe n'est pas vraiment restreinte, on ne risque pas 
                // le Raise dans le BS->Segment.
                if (!Curv.IsPeriodic())
                {
                    if (U1 < Curv.FirstParameter())
                        U1 = Curv.FirstParameter();
                    if (U2 > Curv.LastParameter())
                        U2 = Curv.LastParameter();
                }

                if (Curv is Geom2d_Line)
                {
                    //gp_Pnt2d Pdeb = Ctrim.StartPoint();
                    //gp_Pnt2d Pfin = Ctrim.EndPoint();
                    //Array1OfPnt2d Poles=new Array1OfPnt2d (1, 2);
                    //Poles(1) = Pdeb;
                    //Poles(2) = Pfin;
                    //Array1OfReal Knots=new Array1OfReal (1, 2);
                    //Knots(1) = Ctrim->FirstParameter();
                    //Knots(2) = Ctrim->LastParameter();
                    //Array1OfInteger Mults(1, 2);
                    //Mults(1) = 2;
                    //Mults(2) = 2;
                    //Standard_Integer Degree = 1;
                    //TheCurve = new Geom2d_BSplineCurve(Poles, Knots, Mults, Degree);
                }

            }
            return null;
        }
    }
}
