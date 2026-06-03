namespace TKMath
{
    //! Defines a non-persistent transformation in 3D space.
    //! This transformation is a general transformation.
    //! It can be a gp_Trsf, an affinity, or you can define
    //! your own transformation giving the matrix of transformation.
    //!
    //! With a gp_GTrsf you can transform only a triplet of coordinates gp_XYZ.
    //! It is not possible to transform other geometric objects
    //! because these transformations can change the nature of non-elementary geometric objects.
    //! The transformation gp_GTrsf can be represented as follow:
    //! @code
    //!    V1   V2   V3    T       XYZ        XYZ
    //! | a11  a12  a13   a14 |   | x |      | x'|
    //! | a21  a22  a23   a24 |   | y |      | y'|
    //! | a31  a32  a33   a34 |   | z |   =  | z'|
    //! |  0    0    0     1  |   | 1 |      | 1 |
    //! @endcode
    //! where {V1, V2, V3} define the vectorial part of the
    //! transformation and T defines the translation part of the transformation.
    //! Warning
    //! A gp_GTrsf transformation is only applicable to coordinates.
    //! Be careful if you apply such a transformation to all points of a geometric object,
    //! as this can change the nature of the object and thus render it incoherent!
    //! Typically, a circle is transformed into an ellipse by an affinity transformation.
    //! To avoid modifying the nature of an object, use a gp_Trsf transformation instead,
    //! as objects of this class respect the nature of geometric objects.
    public class gp_GTrsf
    {

        public gp_GTrsf(gp_Trsf theT)
        {
            shape = theT.Form();
            matrix = theT.matrix;
            loc = theT.TranslationPart();
            scale = theT.ScaleFactor();
        }

        gp_Mat matrix;
        gp_XYZ loc;
        gp_TrsfForm shape;
        double scale;

    }
}
