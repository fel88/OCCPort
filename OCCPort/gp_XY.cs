namespace OCCPort
{

    //! This class describes a cartesian coordinate entity in 2D
    //! space {X,Y}. This class is non persistent. This entity used
    //! for algebraic calculation. An XY can be transformed with a
    //! Trsf2d or a  GTrsf2d from package gp.
    //! It is used in vectorial computations or for holding this type
    //! of information in data structures.
    public struct gp_XY

    {


        //! a number pair defined by the XY coordinates
        public gp_XY(double theX, double theY)
        {
            x = theX;
            y = theY;
        }

        //! Assigns the given value to the X coordinate of this number pair.
        public void SetX(double theX) { x = theX; }

        //! Assigns the given value to the Y  coordinate of this number pair.
        public void SetY(double theY) { y = theY; }

        double x;
        double y;

    }
}