using System.Reflection.Metadata;

namespace OCCPort
{
    //! The abstract class Geometry for 3D space is the root
    //! class of all geometric objects from the Geom
    //! package. It describes the common behavior of these objects when:
    //! - applying geometric transformations to objects, and
    //! - constructing objects by geometric transformation (including copying).
    //! Warning
    //! Only transformations which do not modify the nature
    //! of the geometry can be applied to Geom objects: this
    //! is the case with translations, rotations, symmetries
    //! and scales; this is also the case with gp_Trsf
    //! composite transformations which are used to define
    //! the geometric transformations applied using the
    //! Transform or Transformed functions.
    //! Note: Geometry defines the "prototype" of the
    //! abstract method Transform which is defined for each
    //! concrete type of derived object. All other
    //! transformations are implemented using the Transform method.
    public abstract class Geom_Geometry
    {
        public Geom_Geometry Transformed(gp_Trsf T)
        {
            Geom_Geometry G = Copy();
            G.Transform(T);
            return G;
        }

        Geom_Surface basisSurf;
        Geom_Surface equivSurf;
        double offsetValue;

        //! Returns the basis surface of this offset surface.
        //! Note: The basis surface can be an offset surface.
        public virtual Geom_Surface BasisSurface()
        { return basisSurf; }

        public abstract void Transform(gp_Trsf t);

        //! Creates a new object which is a copy of this geometric object.
        public abstract Geom_Geometry Copy();

    }

}