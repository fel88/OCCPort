namespace OCCPort
{
    //! This class describes a MultiCurve approximating a Multiline.
    //! As a Multiline is a set of n lines, a MultiCurve is a set
    //! of n curves. These curves are Bezier curves.
    //! A MultiCurve is composed of m MultiPoint.
    //! The approximating degree of these n curves is the same for
    //! each one.
    //!
    //! Example of a MultiCurve composed of MultiPoints:
    //!
    //! P1______P2_____P3______P4________........_____PNbMPoints
    //!
    //! Q1______Q2_____Q3______Q4________........_____QNbMPoints
    //! .                                               .
    //! .                                               .
    //! .                                               .
    //! R1______R2_____R3______R4________........_____RNbMPoints
    //!
    //! Pi, Qi, ..., Ri are points of dimension 2 or 3.
    //!
    //! (Pi, Qi, ...Ri), i= 1,...NbPoles are MultiPoints.
    //! each MultiPoint has got NbPol Poles.
    public class AppParCurves_MultiCurve
    {
    }
    }