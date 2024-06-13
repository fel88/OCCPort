using System;

namespace OCCPort
{
    internal struct gp_Ax3
    {
        

        //! Creates an object corresponding to the reference
        //! coordinate system (OXYZ).
        /*public gp_Ax3()
        {
            vydir = new gp_Dir(0.0, 1.0, 0.0);
            // vxdir(1.,0.,0.) use default ctor of gp_Dir, as it creates the same dir(1,0,0)
        }*/ 

        

        //! Creates a  right handed axis placement with the
        //! "Location" point theP and  two directions, theN gives the
        //! "Direction" and theVx gives the "XDirection".
        //! Raises ConstructionError if theN and theVx are parallel (same or opposite orientation).
        public gp_Ax3( gp_Pnt theP,  gp_Dir theN,  gp_Dir theVx)
        {
            axis = new gp_Ax1(theP, theN);
            vydir = (theN);
            vxdir = (theN);
        
            vxdir.CrossCross(theVx, theN);
            vydir.Cross(vxdir);
        }

        internal gp_Dir XDirection()
        {
            return vxdir;
        }

        gp_Ax1 axis;
        gp_Dir vydir;
        gp_Dir vxdir;

        internal gp_Dir YDirection()
        {
            return vydir;
        }
    }
}