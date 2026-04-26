using System;

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

        public void SetLinearForm(double theA1, gp_XY theXY1,
                              gp_XY theXY2)
        {
            x = theA1 * theXY1.x + theXY2.x;
            y = theA1 * theXY1.y + theXY2.y;
        }

        //! @code
        //! <me>.X() = -<me>.X()
        //! <me>.Y() = -<me>.Y()
        public void Reverse()
        {
            x = -x;
            y = -y;
        }

        //! Returns the X coordinate of this number pair.
        public double X() { return x; }

        //! Returns the Y coordinate of this number pair.
        public double Y() { return y; }

        //! a number pair defined by the XY coordinates
        public gp_XY(double theX, double theY)
        {
            x = theX;
            y = theY;
        }

        //! Computes X*X + Y*Y where X and Y are the two coordinates of this number pair.
        public double SquareModulus() { return x * x + y * y; }

        public static gp_XY operator -(gp_XY vv, gp_XY v2)
        {
            return new gp_XY(vv.x - v2.x,
                    vv.y - v2.y);
        }
        public static gp_XY operator +(gp_XY vv, gp_XY v2)
        {
            return new gp_XY(vv.x + v2.x,
                    vv.y + v2.y);
        }
        public static gp_XY operator *(gp_XY vv, double v2)
        {
            return new gp_XY(vv.x * v2,
                    vv.y * v2);
        }
        //! Assigns the given value to the X coordinate of this number pair.
        public void SetX(double theX) { x = theX; }

        //! Assigns the given value to the Y  coordinate of this number pair.
        public void SetY(double theY) { y = theY; }

        //! Computes the scalar product between <me> and theOther
        public double Dot(gp_XY theOther) { return x * theOther.x + y * theOther.y; }

        //! @code
        //! double D = <me>.X() * theOther.Y() - <me>.Y() * theOther.X()
        //! @endcode
        public double Crossed(gp_XY theOther) { return x * theOther.y - y * theOther.x; }

        //! For this number pair, returns its coordinates X and Y.
        internal void Coord(ref double theX, ref double theY)
        {
            theX = x;
            theY = y;
        }

        double x;
        double y;

    }
}