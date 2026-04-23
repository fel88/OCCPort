namespace OCCPort
{
    public abstract class Geom_ElementarySurface : Geom_Surface
    {
        protected gp_Ax3 pos;

        //! Returns the local coordinates system of the surface.
        public gp_Ax3 Position() { return pos; }


    }
}