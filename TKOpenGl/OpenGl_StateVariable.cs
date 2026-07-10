namespace OCCPort.OpenGL
{
    //! The enumeration of OCCT-specific OpenGL/GLSL variables.
    public enum OpenGl_StateVariable
    {
        // OpenGL matrix state
        OpenGl_OCC_MODEL_WORLD_MATRIX,
        OpenGl_OCC_WORLD_VIEW_MATRIX,
        OpenGl_OCC_PROJECTION_MATRIX,
        OpenGl_OCC_MODEL_WORLD_MATRIX_INVERSE,
        OpenGl_OCC_WORLD_VIEW_MATRIX_INVERSE,
        OpenGl_OCC_PROJECTION_MATRIX_INVERSE,
        OpenGl_OCC_MODEL_WORLD_MATRIX_TRANSPOSE,
        OpenGl_OCC_WORLD_VIEW_MATRIX_TRANSPOSE,
        OpenGl_OCC_PROJECTION_MATRIX_TRANSPOSE,
        OpenGl_OCC_MODEL_WORLD_MATRIX_INVERSE_TRANSPOSE,
        OpenGl_OCC_WORLD_VIEW_MATRIX_INVERSE_TRANSPOSE,
        OpenGl_OCC_PROJECTION_MATRIX_INVERSE_TRANSPOSE,

        // OpenGL clip planes state
        OpenGl_OCC_CLIP_PLANE_EQUATIONS,
        OpenGl_OCC_CLIP_PLANE_CHAINS,
        OpenGl_OCC_CLIP_PLANE_COUNT,

        // OpenGL light state
        OpenGl_OCC_LIGHT_SOURCE_COUNT,
        OpenGl_OCC_LIGHT_SOURCE_TYPES,
        OpenGl_OCC_LIGHT_SOURCE_PARAMS,
        OpenGl_OCC_LIGHT_AMBIENT,
        OpenGl_OCC_LIGHT_SHADOWMAP_SIZE_BIAS,// occShadowMapSizeBias
        OpenGl_OCC_LIGHT_SHADOWMAP_SAMPLERS, // occShadowMapSamplers
        OpenGl_OCC_LIGHT_SHADOWMAP_MATRICES, // occShadowMapMatrices

        // Material state
        OpenGl_OCCT_TEXTURE_ENABLE,
        OpenGl_OCCT_DISTINGUISH_MODE,
        OpenGl_OCCT_PBR_MATERIAL,
        OpenGl_OCCT_COMMON_MATERIAL,
        OpenGl_OCCT_ALPHA_CUTOFF,
        OpenGl_OCCT_COLOR,

        // Weighted, Blended Order-Independent Transparency rendering state
        OpenGl_OCCT_OIT_OUTPUT,
        OpenGl_OCCT_OIT_DEPTH_FACTOR,

        // Context-dependent state
        OpenGl_OCCT_TEXTURE_TRSF2D,
        OpenGl_OCCT_POINT_SIZE,

        // Wireframe state
        OpenGl_OCCT_VIEWPORT,
        OpenGl_OCCT_LINE_WIDTH,
        OpenGl_OCCT_LINE_FEATHER,
        OpenGl_OCCT_LINE_STIPPLE_PATTERN, // occStipplePattern
        OpenGl_OCCT_LINE_STIPPLE_FACTOR,  // occStippleFactor
        OpenGl_OCCT_WIREFRAME_COLOR,
        OpenGl_OCCT_QUAD_MODE_STATE,

        // Parameters of outline (silhouette) shader
        OpenGl_OCCT_ORTHO_SCALE,
        OpenGl_OCCT_SILHOUETTE_THICKNESS,

        // PBR state
        OpenGl_OCCT_NB_SPEC_IBL_LEVELS,

        // DON'T MODIFY THIS ITEM (insert new items before it)
        OpenGl_OCCT_NUMBER_OF_STATE_VARIABLES
    }
}