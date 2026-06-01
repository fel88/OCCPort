namespace OCCPort
{
    public class Graphic3d_RenderingParams
    {
        //! Default pixels density.
        static uint THE_DEFAULT_RESOLUTION = 72u;
        public Graphic3d_RenderingParams()
        {
            Method = Graphic3d_RenderingMode.Graphic3d_RM_RASTERIZATION;
            ShadingModel = Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Phong;
            //TransparencyMethod=(Graphic3d_RTM_BLEND_UNORDERED),
            Resolution = (THE_DEFAULT_RESOLUTION);
            //FontHinting(Font_Hinting_Off),
            //LineFeather(1.0f),
            // PBR parameters
            //PbrEnvPow2Size(9),
            //PbrEnvSpecMapNbLevels(6),
            //PbrEnvBakingDiffNbSamples(1024),
            //PbrEnvBakingSpecNbSamples(256),
            //PbrEnvBakingProbability(0.99f),
            //
            //OitDepthFactor(0.0f),
            //NbOitDepthPeelingLayers(4),
            //NbMsaaSamples(0),
            RenderResolutionScale = (1.0f);
            /*ShadowMapResolution(1024),
            ShadowMapBias(0.005f),
            ToEnableDepthPrepass(Standard_False),
            ToEnableAlphaToCoverage(Standard_True),
            // ray tracing parameters
            IsGlobalIlluminationEnabled(Standard_False),
            SamplesPerPixel(0),
            RaytracingDepth(THE_DEFAULT_DEPTH),
            IsShadowEnabled(Standard_True),
            IsReflectionEnabled(Standard_False),
            IsAntialiasingEnabled(Standard_False),
            IsTransparentShadowEnabled(Standard_False),
            UseEnvironmentMapBackground(Standard_False),
            ToIgnoreNormalMapInRayTracing(Standard_False),
            CoherentPathTracingMode(Standard_False),
            AdaptiveScreenSampling(Standard_False),
            AdaptiveScreenSamplingAtomic(Standard_False),
            ShowSamplingTiles(Standard_False),
            TwoSidedBsdfModels(Standard_False),
            RadianceClampingValue(30.0),
            RebuildRayTracingShaders(Standard_False),
            RayTracingTileSize(32),
            NbRayTracingTiles(16 * 16),
            CameraApertureRadius(0.0f),
            CameraFocalPlaneDist(1.0f),
            FrustumCullingState(FrustumCulling_On),
            ToneMappingMethod(Graphic3d_ToneMappingMethod_Disabled),
            Exposure(0.f),
            WhitePoint(1.f),
            // stereoscopic parameters
            StereoMode(Graphic3d_StereoMode_QuadBuffer),
            HmdFov2d(30.0f),
            AnaglyphFilter(Anaglyph_RedCyan_Optimized),
            ToReverseStereo(Standard_False),
            ToSmoothInterlacing(Standard_True),
            ToMirrorComposer(Standard_True),
            //
            StatsPosition(new Graphic3d_TransformPers(Graphic3d_TMF_2d, Aspect_TOTP_LEFT_UPPER, Graphic3d_Vec2i(20, 20))),
            ChartPosition(new Graphic3d_TransformPers(Graphic3d_TMF_2d, Aspect_TOTP_RIGHT_UPPER, Graphic3d_Vec2i(20, 20))),
            ChartSize(-1, -1),
            StatsTextAspect(new Graphic3d_AspectText3d()),
            StatsUpdateInterval(1.0),
            StatsTextHeight(16),
            StatsNbFrames(1),
            StatsMaxChartTime(0.1f),
            CollectedStats(PerfCounters_Basic),
            ToShowStats(Standard_False)*/
        }

        //! Returns resolution ratio.
        public float ResolutionRatio()
        {
            return Resolution / (THE_DEFAULT_RESOLUTION);
        }

        public uint Resolution;                  //!< Pixels density (PPI), defines scaling factor for parameters like text size
                                                 //!  (when defined in screen-space units rather than in 3D) to be properly displayed
                                                 //!  on device (screen / printer). 72 is default value.
                                                 //!  Note that using difference resolution in different Views in same Viewer
                                                 //!  will lead to performance regression (for example, text will be recreated every time).
        public bool ToEnableAlphaToCoverage { get; set; }
        public Graphic3d_RenderingMode Method { get; set; }
        public int NbMsaaSamples;               //!< number of MSAA samples (should be within 0..GL_MAX_SAMPLES, power-of-two number), 0 by default
        public float RenderResolutionScale;       //!< rendering resolution scale factor, 1 by default;

        public Graphic3d_TypeOfShadingModel ShadingModel;                //!< specified default shading model, Graphic3d_TypeOfShadingModel_Phong by default
        public Graphic3d_StereoMode StereoMode;                  //!< stereoscopic output mode, Graphic3d_StereoMode_QuadBuffer by default

        public bool ToReverseStereo;             //!< flag to reverse stereo pair, FALSE by default
        public bool ToSmoothInterlacing;         //!< flag to smooth output on interlaced displays (improves text readability / reduces line aliasing), TRUE by default
        public bool ToMirrorComposer;            //!< if output device is an external composer - mirror rendering results in window in addition to sending frame to composer, TRUE by default

    }    //! This enumeration defines the list of stereoscopic output modes.
    //public enum { Graphic3d_StereoMode_NB = Graphic3d_StereoMode_OpenVR + 1 };




}