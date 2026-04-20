namespace OCCPort
{
    //! Definition of the Trihedron position in the views.
    //! It is defined as a bitmask to simplify handling vertical and horizontal alignment independently.
    enum Aspect_TypeOfTriedronPosition
    {
        Aspect_TOTP_CENTER = 0x0000,             //!< at the center of the view
        Aspect_TOTP_TOP = 0x0001,             //!< at the middle of the top    side
        Aspect_TOTP_BOTTOM = 0x0002,             //!< at the middle of the bottom side
        Aspect_TOTP_LEFT = 0x0004,             //!< at the middle of the left   side
        Aspect_TOTP_RIGHT = 0x0008,             //!< at the middle of the right  side
        Aspect_TOTP_LEFT_LOWER = Aspect_TOTP_BOTTOM
                                | Aspect_TOTP_LEFT,   //!< at the left lower corner
        Aspect_TOTP_LEFT_UPPER = Aspect_TOTP_TOP
                                | Aspect_TOTP_LEFT,   //!< at the left upper corner
        Aspect_TOTP_RIGHT_LOWER = Aspect_TOTP_BOTTOM
                                | Aspect_TOTP_RIGHT,  //!< at the right lower corner
        Aspect_TOTP_RIGHT_UPPER = Aspect_TOTP_TOP
                                | Aspect_TOTP_RIGHT,  //!< at the right upper corner

    }
}