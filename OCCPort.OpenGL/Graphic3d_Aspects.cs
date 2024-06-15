using System;

namespace OCCPort.OpenGL
{
    public class Graphic3d_Aspects
    { //! Return shader program.
        public Graphic3d_ShaderProgram ShaderProgram() { return myProgram; }

        internal bool ToMapTexture()
        {
            throw new NotImplementedException();
        }

        Graphic3d_ShaderProgram myProgram;
        Graphic3d_TextureSet myTextureSet;
        Graphic3d_MarkerImage myMarkerImage;
        //Graphic3d_HatchStyle     myHatchStyle;
        string myTextFont;
        // Graphic3d_MaterialAspect myFrontMaterial;
        //  Graphic3d_MaterialAspect myBackMaterial;

        //Quantity_ColorRGBA myInteriorColor;
        //Quantity_ColorRGBA myBackInteriorColor;
        //Quantity_ColorRGBA myEdgeColor;

        //  Graphic3d_PolygonOffset myPolygonOffset;
        //  Aspect_InteriorStyle myInteriorStyle;
        Graphic3d_TypeOfShadingModel myShadingModel;
        Graphic3d_TypeOfBackfacingModel myFaceCulling;
        //   Graphic3d_AlphaMode myAlphaMode;
        float myAlphaCutoff;

        //  Aspect_TypeOfLine myLineType;
        float myLineWidth;
        ushort myLineFactor;
        ushort myLinePattern;

        //  Aspect_TypeOfMarker myMarkerType;
        float myMarkerScale;

        //   Aspect_TypeOfStyleText myTextStyle;
        // Aspect_TypeOfDisplayText myTextDisplayType;
        // Font_FontAspect myTextFontAspect;
        float myTextAngle;

        bool myToSkipFirstEdge;
        bool myToDistinguishMaterials;
        bool myToDrawEdges;
        bool myToDrawSilhouette;
        bool myToMapTexture;
        bool myIsTextZoomable;

    }
}