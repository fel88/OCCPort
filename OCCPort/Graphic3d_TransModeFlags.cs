namespace OCCPort
{
	//! Transform Persistence Mode defining whether to lock in object position, rotation and / or zooming relative to camera position.
	public enum Graphic3d_TransModeFlags
	{
		Graphic3d_TMF_None = 0x0000,                  //!< no persistence attributes (normal 3D object)
		Graphic3d_TMF_ZoomPers = 0x0002,                  //!< object does not resize
		Graphic3d_TMF_RotatePers = 0x0008,                  //!< object does not rotate;
		Graphic3d_TMF_TriedronPers = 0x0020,                  //!< object behaves like trihedron - it is fixed at the corner of view and does not resizing (but rotating)
		Graphic3d_TMF_2d = 0x0040,                  //!< object is defined in 2D screen coordinates (pixels) and does not resize, pan and rotate
		Graphic3d_TMF_CameraPers = 0x0080,                  //!< object is in front of the camera
		Graphic3d_TMF_ZoomRotatePers = Graphic3d_TMF_ZoomPers
									 | Graphic3d_TMF_RotatePers //!< object doesn't resize and rotate
	};


}