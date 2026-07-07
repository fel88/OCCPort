using System.Reflection.Metadata;

namespace TKService
{
    //! This class allows the definition of a manager to
    //! which the graphic objects are associated.
    //! It allows them to be globally manipulated.
    //! It defines the global attributes.
    public abstract class Graphic3d_DataStructureManager
    {
        //! Returns camera object of the view.
        public abstract Graphic3d_Camera Camera() ;

    }
}
