using System;

namespace TKMath
{
    public struct gp_Ax2
    {

        //! Creates an object corresponding to the reference
        //! coordinate system (OXYZ).
        public gp_Ax2()
        {
            vydir = new gp_Dir(0.0, 1.0, 0.0);
            // vxdir(1.,0.,0.) use default ctor of gp_Dir, as it creates the same dir(1,0,0)
        }

        //! Creates an axis placement with an origin P such that:
        //! -   N is the Direction, and
        //! -   the "X Direction" is normal to N, in the plane
        //! defined by the vectors (N, Vx): "X
        //! Direction" = (N ^ Vx) ^ N,
        //! Exception: raises ConstructionError if N and Vx are parallel (same or opposite orientation).
        public gp_Ax2(gp_Pnt P, gp_Dir N, gp_Dir Vx)
        {
            axis = new gp_Ax1(P, N);
            vydir = N;
            vxdir = N;
            vxdir.CrossCross(Vx, N);
            vydir.Cross(vxdir);
        }

        //! Returns the main axis of <me>. It is the "Location" point
        //! and the main "Direction".
        public gp_Ax1 Axis() { return axis; }


        //! Returns the main direction of <me>.
        public gp_Dir Direction() { return axis.Direction(); }

        //! Returns the "Location" point (origin) of <me>.
        public gp_Pnt Location() { return axis.Location(); }

        //! Returns the "XDirection" of <me>.


        gp_Ax1 axis;
        gp_Dir vydir;
        gp_Dir vxdir;

        public gp_Dir XDirection()
        {
            return vxdir;
        }

        public gp_Dir YDirection()
        {
            return vydir;
        }
    }
}