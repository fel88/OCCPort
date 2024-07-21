namespace OCCPort
{
    public enum Graphic3d_StereoMode
    {
        Graphic3d_StereoMode_QuadBuffer,       //!< OpenGL QuadBuffer
        Graphic3d_StereoMode_Anaglyph,         //!< Anaglyph glasses, the type should be specified in addition
        Graphic3d_StereoMode_RowInterlaced,    //!< Row-interlaced stereo
        Graphic3d_StereoMode_ColumnInterlaced, //!< Column-interlaced stereo
        Graphic3d_StereoMode_ChessBoard,       //!< chess-board stereo for DLP TVs
        Graphic3d_StereoMode_SideBySide,       //!< horizontal pair
        Graphic3d_StereoMode_OverUnder,        //!< vertical   pair
        Graphic3d_StereoMode_SoftPageFlip,     //!< software PageFlip for shutter glasses, should NOT be used!
        Graphic3d_StereoMode_OpenVR,           //!< OpenVR (HMD)
    };
    //public enum { Graphic3d_StereoMode_NB = Graphic3d_StereoMode_OpenVR + 1 };


}