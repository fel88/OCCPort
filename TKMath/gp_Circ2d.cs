using OCCPort.Common;

namespace TKMath
{
    //! Describes a circle in the plane (2D space).
    //! A circle is defined by its radius and positioned in the
    //! plane with a coordinate system (a gp_Ax22d object) as follows:
    //! -   the origin of the coordinate system is the center of the circle, and
    //! -   the orientation (direct or indirect) of the coordinate
    //! system gives an implicit orientation to the circle (and
    //! defines its trigonometric sense).
    //! This positioning coordinate system is the "local
    //! coordinate system" of the circle.
    //! Note: when a gp_Circ2d circle is converted into a
    //! Geom2d_Circle circle, some implicit properties of the
    //! circle are used explicitly:
    //! -   the implicit orientation corresponds to the direction in
    //! which parameter values increase,
    //! -   the starting point for parameterization is that of the "X
    //! Axis" of the local coordinate system (i.e. the "X Axis" of the circle).
    //! See Also
    //! GccAna and Geom2dGcc packages which provide
    //! functions for constructing circles defined by geometric constraints
    //! gce_MakeCirc2d which provides functions for more
    //! complex circle constructions
    //! Geom2d_Circle which provides additional functions for
    //! constructing circles and works, with the parametric
    //! equations of circles in particular  gp_Ax22d
    public class gp_Circ2d
    { //! The location point of theXAxis is the center of the circle.
      //! Warnings :
      //! It is not forbidden to create a circle with theRadius = 0.0   Raises ConstructionError if theRadius < 0.0.
      //! Raised if theRadius < 0.0.
        public gp_Circ2d(gp_Ax2d theXAxis, double theRadius, bool theIsSense = true)
        {
            radius = (theRadius);
            Exceptions.Standard_ConstructionError_Raise_if(theRadius < 0.0, "gp_Circ2d() - radius should be positive number");
            pos = new gp_Ax22d(theXAxis, theIsSense);
        }

        //! theAxis defines the Xaxis and Yaxis of the circle which defines
        //! the origin and the sense of parametrization.
        //! The location point of theAxis is the center of the circle.
        //! Warnings :
        //! It is not forbidden to create a circle with theRadius = 0.0 Raises ConstructionError if theRadius < 0.0.
        //! Raised if theRadius < 0.0.
        public gp_Circ2d(gp_Ax22d theAxis, double theRadius)
        {
            pos = (theAxis);
            radius = theRadius;

            Exceptions.Standard_ConstructionError_Raise_if(theRadius < 0.0, "gp_Circ2d() - radius should be positive number");
        }

        //! Returns the radius value of the circle.
        public double Radius() { return radius; }

        //! returns the position of the circle.
        public gp_Ax22d Axis() { return pos; }

        //! returns the position of the circle. Idem Axis(me).
        public gp_Ax22d Position() { return pos; }

        gp_Ax22d pos;
        double radius;
    }
}