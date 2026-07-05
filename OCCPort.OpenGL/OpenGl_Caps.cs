using OCCPort.Common;

namespace OCCPort
{
    public class OpenGl_Caps
    {

        public OpenGl_Caps()
        {
            //          sRGBDisable(Standard_False),
            //compressedTexturesDisable(Standard_False),
            //vboDisable(Standard_False),
            //pntSpritesDisable(Standard_False),
            //keepArrayData(Standard_False),
            //ffpEnable(Standard_False),
            //usePolygonMode(Standard_False),
            //useSystemBuffer(Standard_False),
            swapInterval = (1);
            //useZeroToOneDepth(Standard_False),
            //buffersNoSwap(Standard_False),
            buffersOpaqueAlpha = (true);
            //buffersDeepColor(Standard_False),
            //contextStereo(Standard_False),
            //contextDebug(Standard_False),
            //contextSyncDebug(Standard_False),
            //contextNoAccel(Standard_False),
            contextCompatible = (true);
            //contextNoExtensions(Standard_False),
            contextMajorVersionUpper = (-1);
            contextMinorVersionUpper = (-1);
            //isTopDownTextureUV(Standard_False),
            //glslWarnings(Standard_False),
            //suppressExtraMsg(Standard_True),
            //glslDumpLevel(OpenGl_ShaderProgramDumpLevel_Off)
        }


        /**
         * Synthetically restrict upper version of OpenGL functionality to be used.
         * Should be used for debugging purposes only!
         *
         * (-1, -1) by default, which means no restriction.
         */
        public int contextMajorVersionUpper;
        public int contextMinorVersionUpper;

        /**
         * Request backward-compatible GL context. This flag requires support in OpenGL driver.
         *
         * Backward-compatible profile includes deprecated functionality like FFP (fixed-function pipeline),
         * and might be useful for compatibility with application OpenGL code.
         *
         * Most drivers support all features within backward-compatibility profile,
         * but some limit functionality to OpenGL 2.1 (e.g. OS X) when core profile is not explicitly requested.
         *
         * Requires OpenGL 3.2+ drivers.
         * Has no effect on OpenGL ES 2.0+ drivers (which do not provide FFP compatibility).
         * Interacts with ffpEnable option, which should be disabled within core profile.
         *
         * ON by default.
         */
        public bool contextCompatible;
        /**
         * Specify that driver should not swap back/front buffers at the end of frame.
         * Useful when OCCT Viewer is integrated into existing OpenGL rendering pipeline as part,
         * thus swapping part is performed outside.
         *
         * OFF by default.
         */

        //! Print GLSL program compilation/linkage warnings, if any. OFF by default.
        public bool glslWarnings;

        public bool buffersNoSwap;
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
        /**
  * Specify whether alpha component within color buffer should be written or not.
  * With alpha write enabled, background is considered transparent by default
  * and overridden by alpha value of last drawn object
  * (e.g. it could be opaque or not in case of transparent material).
  * With alpha writes disabled, color buffer will be kept opaque.
  *
  * ON by default.
  */
        public bool buffersOpaqueAlpha;


        /**
         * Disallow using OpenGL extensions.
         * Should be used for debugging purposes only!
         *
         * OFF by default.
         */
        public bool contextNoExtensions;
    }
}