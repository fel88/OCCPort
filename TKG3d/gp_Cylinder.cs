using OCCPort.Common;
using System.Threading;
using TKMath;

namespace TKG3d
{
    //! Describes an infinite cylindrical surface.
    //! A cylinder is defined by its radius and positioned in space
    //! with a coordinate system (a gp_Ax3 object), the "main
    //! Axis" of which is the axis of the cylinder. This coordinate
    //! system is the "local coordinate system" of the cylinder.
    //! Note: when a gp_Cylinder cylinder is converted into a
    //! Geom_CylindricalSurface cylinder, some implicit
    //! properties of its local coordinate system are used explicitly:
    //! -   its origin, "X Direction", "Y Direction" and "main
    //! Direction" are used directly to define the parametric
    //! directions on the cylinder and the origin of the parameters,
    //! -   its implicit orientation (right-handed or left-handed)
    //! gives an orientation (direct or indirect) to the
    //! Geom_CylindricalSurface cylinder.
    //! See Also
    //! gce_MakeCylinder which provides functions for more
    //! complex cylinder constructions
    //! Geom_CylindricalSurface which provides additional
    //! functions for constructing cylinders and works, in
    //! particular, with the parametric equations of cylinders gp_Ax3
    public class gp_Cylinder
    {
        //! Creates a indefinite cylinder.
        public gp_Cylinder() { radius = Standard_Real.RealLast(); }
        //! Creates a cylinder of radius Radius, whose axis is the "main
        //! Axis" of theA3. theA3 is the local coordinate system of the cylinder.   Raises ConstructionErrord if theRadius < 0.0
        public gp_Cylinder(gp_Ax3 theA3, double theRadius)
        {
            pos = (theA3);
            radius = (theRadius);
            Exceptions.Standard_ConstructionError_Raise_if(theRadius < 0.0, "gp_Cylinder() - radius should be positive number");
        }
        //! Returns the "Location" point of the cylinder.
        public gp_Pnt Location() { return pos.Location(); }

        //! Returns the local coordinate system of the cylinder.
        public gp_Ax3 Position() { return pos; }

        [NotOrigin]
        public gp_Cylinder(gp_Cylinder gp_Cylinder)
        {
            pos = gp_Cylinder.pos;
            radius = gp_Cylinder.radius;
        }

        public gp_Cylinder Transformed(gp_Trsf theT)
        {
            gp_Cylinder aCyl = new gp_Cylinder(this);
            aCyl.pos.Transform(theT);
            aCyl.radius *= theT.ScaleFactor();
            if (aCyl.radius < 0)
            {
                aCyl.radius = -aCyl.radius;
            }
            return aCyl;
        }
        gp_Ax3 pos;
        double radius;

    }
}
