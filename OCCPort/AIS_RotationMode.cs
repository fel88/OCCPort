namespace OCCPort
{
    //! Camera rotation mode.
    public enum AIS_RotationMode
    {
        AIS_RotationMode_BndBoxActive, //!< default OCCT rotation
        AIS_RotationMode_PickLast,     //!< rotate around last picked point
        AIS_RotationMode_PickCenter,   //!< rotate around point at the center of window
        AIS_RotationMode_CameraAt,     //!< rotate around camera center
        AIS_RotationMode_BndBoxScene,  //!< rotate around scene center
    };

   

}
