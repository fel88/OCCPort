namespace TKService
{



    public partial class Graphic3d_Camera
    {
        //! Enumerates approaches to define stereographic focus.
        //! - FocusType_Absolute : focus is specified as absolute value.
        //! - FocusType_Relative : focus is specified relative to
        //! (as coefficient of) camera focal length.
        public enum FocusType
        {
            FocusType_Absolute,
            FocusType_Relative
        };

    }
}
