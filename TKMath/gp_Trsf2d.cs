namespace TKMath
{
    //! Defines a non-persistent transformation in 2D space.
    //! The following transformations are implemented :
    //! - Translation, Rotation, Scale
    //! - Symmetry with respect to a point and a line.
    //! Complex transformations can be obtained by combining the
    //! previous elementary transformations using the method Multiply.
    //! The transformations can be represented as follow :
    //! @code
    //!    V1   V2   T       XY        XY
    //! | a11  a12  a13 |   | x |     | x'|
    //! | a21  a22  a23 |   | y |     | y'|
    //! |  0    0    1  |   | 1 |     | 1 |
    //! @endcode
    //! where {V1, V2} defines the vectorial part of the transformation
    //! and T defines the translation part of the transformation.
    //! This transformation never change the nature of the objects.
    public class gp_Trsf2d
    {
    }
    }

