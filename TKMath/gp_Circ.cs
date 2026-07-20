using OCCPort.Common;
using System.Threading;

namespace TKMath
{
    //! Describes a circle in 3D space.
    //! A circle is defined by its radius and positioned in space
    //! with a coordinate system (a gp_Ax2 object) as follows:
    //! -   the origin of the coordinate system is the center of the circle, and
    //! -   the origin, "X Direction" and "Y Direction" of the
    //! coordinate system define the plane of the circle.
    //! This positioning coordinate system is the "local
    //! coordinate system" of the circle. Its "main Direction"
    //! gives the normal vector to the plane of the circle. The
    //! "main Axis" of the coordinate system is referred to as
    //! the "Axis" of the circle.
    //! Note: when a gp_Circ circle is converted into a
    //! Geom_Circle circle, some implicit properties of the
    //! circle are used explicitly:
    //! -   the "main Direction" of the local coordinate system
    //! gives an implicit orientation to the circle (and defines
    //! its trigonometric sense),
    //! -   this orientation corresponds to the direction in
    //! which parameter values increase,
    //! -   the starting point for parameterization is that of the
    //! "X Axis" of the local coordinate system (i.e. the "X Axis" of the circle).
    //! See Also
    //! gce_MakeCirc which provides functions for more complex circle constructions
    //! Geom_Circle which provides additional functions for
    //! constructing circles and works, in particular, with the
    //! parametric equations of circles
    public class gp_Circ
    {

        //! Creates an indefinite circle.
        public gp_Circ()
        {
            radius = Standard_Real.RealLast();
        }

        //! A2 locates the circle and gives its orientation in 3D space.
        //! Warnings :
        //! It is not forbidden to create a circle with theRadius = 0.0  Raises ConstructionError if theRadius < 0.0
        public gp_Circ(gp_Ax2 theA2, double theRadius)
        {
            pos = (theA2);
            radius = theRadius;
            Exceptions.Standard_ConstructionError_Raise_if(theRadius < 0.0, "gp_Circ() - radius should be positive number");
        }

        //! Returns the main axis of the circle.
        //! It is the axis perpendicular to the plane of the circle,
        //! passing through the "Location" point (center) of the circle.
        public gp_Ax1 Axis() { return pos.Axis(); }

        //! Returns the position of the circle.
        //! It is the local coordinate system of the circle.
        public gp_Ax2 Position() { return pos; }

        //! Returns the radius of this circle.
        public double Radius() { return radius; }

        gp_Ax2 pos;
        double radius;
        public void Transform(gp_Trsf theT)
        {
            radius *= theT.ScaleFactor();
            if (radius < 0)
            {
                radius = -radius;
            }
            pos.Transform(theT);
        }

        //! Returns the "XAxis" of the circle.
        //! This axis is perpendicular to the axis of the conic.
        //! This axis and the "Yaxis" define the plane of the conic.
        public gp_Ax1 XAxis() { return new gp_Ax1(pos.Location(), pos.XDirection()); }

        //! Returns the "YAxis" of the circle.
        //! This axis and the "Xaxis" define the plane of the conic.
        //! The "YAxis" is perpendicular to the "Xaxis".
        public gp_Ax1 YAxis()
        {
            return new gp_Ax1(pos.Location(), pos.YDirection());
        }

        //! Returns the center of the circle. It is the
        //! "Location" point of the local coordinate system
        //! of the circle
        public gp_Pnt Location() { return pos.Location(); }


    }
}