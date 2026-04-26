namespace OCCPort
{
    //! Defines a non-persistent vector in 2D space.
    public class gp_Vec2d
    {
        //! For this vector, returns its X  coordinate.
        public double X() { return coord.X(); }

        //! For this vector, returns its Y  coordinate.
        public double Y() { return coord.Y(); }
        //! Assigns the two coordinates of theCoord to this vector.
      public  void SetXY( gp_XY theCoord) { coord = theCoord; }

        gp_XY coord;


    }
}