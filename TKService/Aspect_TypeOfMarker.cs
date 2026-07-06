namespace TKService
{
    //! Definition of types of markers
    public enum Aspect_TypeOfMarker
    {
        Aspect_TOM_EMPTY = -1,  //!< hidden
        Aspect_TOM_POINT = 0,  //!< point   .
        Aspect_TOM_PLUS,        //!< plus    +
        Aspect_TOM_STAR,        //!< star    *
        Aspect_TOM_X,           //!< cross   x
        Aspect_TOM_O,           //!< circle  O
        Aspect_TOM_O_POINT,     //!< a point in a circle
        Aspect_TOM_O_PLUS,      //!< a plus  in a circle
        Aspect_TOM_O_STAR,      //!< a star  in a circle
        Aspect_TOM_O_X,         //!< a cross in a circle
        Aspect_TOM_RING1,       //!< a large  ring
        Aspect_TOM_RING2,       //!< a medium ring
        Aspect_TOM_RING3,       //!< a small  ring
        Aspect_TOM_BALL,        //!< a ball with 1 color and different saturations
        Aspect_TOM_USERDEFINED  //!< defined by Users (custom image)
    };
}
