namespace OCCPort
{
    public class OpenGl_Caps
    {
        public bool sRGBDisable;       //!< Disables sRGB rendering (OFF by default)
        public bool compressedTexturesDisable; //!< Disables uploading of compressed texture formats native to GPU (OFF by default)
        public bool vboDisable;        //!< disallow VBO usage for debugging purposes (OFF by default)
        public bool pntSpritesDisable; //!< flag permits Point Sprites usage, will significantly affect performance (OFF by default)
        public bool keepArrayData;     //!< Disables freeing CPU memory after building VBOs (OFF by default)
        public bool ffpEnable;         //!< Enables FFP (fixed-function pipeline), do not use built-in GLSL programs (OFF by default)
        public bool usePolygonMode;    //!< Enables Polygon Mode instead of built-in GLSL programs (OFF by default; unsupported on OpenGL ES)
        public bool useSystemBuffer;   //!< Enables usage of system backbuffer for blitting (OFF by default on desktop OpenGL and ON on OpenGL ES for testing)
        public int swapInterval;      //!< controls swap interval - 0 for VSync off and 1 for VSync on, 1 by default
        public bool useZeroToOneDepth; //!< use [0, 1] depth range instead of [-1, 1] range, when possible (OFF by default)

    }
}