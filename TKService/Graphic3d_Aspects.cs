using OCCPort;
using OCCPort.Common;
using System;
using TKernel;

namespace TKService
{
    //! This class defines graphic attributes.
    public class Graphic3d_Aspects
    {

        public Graphic3d_Aspects()
        {
            myInteriorColor = new Quantity_ColorRGBA(new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_CYAN1));
            myBackInteriorColor = new Quantity_ColorRGBA(new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_CYAN1));
            //myEdgeColor(Quantity_NOC_WHITE),*/
            myInteriorStyle = Aspect_InteriorStyle.Aspect_IS_SOLID;
            myShadingModel = Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_DEFAULT;
            myFaceCulling = Graphic3d_TypeOfBackfacingModel.Graphic3d_TypeOfBackfacingModel_Auto;
            myAlphaMode = Graphic3d_AlphaMode.Graphic3d_AlphaMode_BlendAuto;
            myMarkerType = Aspect_TypeOfMarker.Aspect_TOM_POINT;

        }

        //! Return line type; Aspect_TOL_SOLID by default.
        public Aspect_TypeOfLine LineType() { return myLineType; }

        //! Return marker type; Aspect_TOM_POINT by default.
        public Aspect_TypeOfMarker MarkerType() { return myMarkerType; }

        //! Return interior color.
        public Quantity_ColorRGBA InteriorColorRGBA() { return myInteriorColor; }

        //! Return a multiplier for each bit in the line stipple pattern within [1, 256] range; 1 by default.
        public ushort LineStippleFactor()  { return myLineFactor; }

        Aspect_TypeOfMarker myMarkerType;

        //! Return edges line type (same as LineType()).
        public Aspect_TypeOfLine EdgeLineType() { return myLineType; }

        //! Sets polygon offsets settings.
        public void SetPolygonOffset(Graphic3d_PolygonOffset theOffset) { myPolygonOffset = theOffset; }

        //! Returns alpha cutoff threshold, for discarding fragments within Graphic3d_AlphaMode_Mask mode (0.5 by default).
        //! If the alpha value is greater than or equal to this value then it is rendered as fully opaque, otherwise, it is rendered as fully transparent.
        public float AlphaCutoff() { return myAlphaCutoff; }

        //! Return color of edges.
        public Quantity_ColorRGBA EdgeColorRGBA() { return myEdgeColor; }

        //! Returns true if mesh edges should be drawn (false by default).
        public bool ToDrawEdges() { return myToDrawEdges && myLineType != Aspect_TypeOfLine.Aspect_TOL_EMPTY; }

        //! Return interior color.
        public Quantity_Color InteriorColor() { return myInteriorColor.GetRGB(); }

        //! Return back interior color.
        public Quantity_Color BackInteriorColor() { return myBackInteriorColor.GetRGB(); }

        //! Returns current polygon offsets settings.
        public Graphic3d_PolygonOffset PolygonOffset() { return myPolygonOffset; }
        Graphic3d_PolygonOffset myPolygonOffset = new Graphic3d_PolygonOffset();


        //! Return face culling mode; Graphic3d_FaceCulling_BackClosed by default.
        //! A back-facing polygon is defined as a polygon whose
        //! vertices are in a clockwise order with respect to screen coordinates.
        public Graphic3d_TypeOfBackfacingModel FaceCulling() { return myFaceCulling; }

        //! Modifies the interior type used for rendering
        public void SetInteriorStyle(Aspect_InteriorStyle theStyle) { myInteriorStyle = theStyle; }

        //! Sets up OpenGL/GLSL shader program.
        public void SetShaderProgram(Graphic3d_ShaderProgram theProgram) { myProgram = theProgram; }

        //! Check for equality with another aspects.
        public bool IsEqual(Graphic3d_Aspects theOther)
        {
            if (this == theOther)
                return true;

            return myProgram == theOther.myProgram
                && myTextureSet == theOther.myTextureSet
                && myMarkerImage == theOther.myMarkerImage
                && myInteriorColor == theOther.myInteriorColor
                && myBackInteriorColor == theOther.myBackInteriorColor
                && myFrontMaterial == theOther.myFrontMaterial
                && myBackMaterial == theOther.myBackMaterial
                //   && myInteriorStyle == theOther.myInteriorStyle
                && myShadingModel == theOther.myShadingModel
                && myFaceCulling == theOther.myFaceCulling
                && myAlphaMode == theOther.myAlphaMode
                && myAlphaCutoff == theOther.myAlphaCutoff
                && myLineType == theOther.myLineType
                && myEdgeColor == theOther.myEdgeColor
                && myLineWidth == theOther.myLineWidth
                && myLineFactor == theOther.myLineFactor
                && myLinePattern == theOther.myLinePattern
                && myMarkerType == theOther.myMarkerType
                && myMarkerScale == theOther.myMarkerScale
                // && myHatchStyle == theOther.myHatchStyle
                && myTextFont == theOther.myTextFont
                && myPolygonOffset == theOther.myPolygonOffset
                //  && myTextStyle == theOther.myTextStyle
                //   && myTextDisplayType == theOther.myTextDisplayType
                //  && myTextFontAspect == theOther.myTextFontAspect
                && myTextAngle == theOther.myTextAngle
                && myToSkipFirstEdge == theOther.myToSkipFirstEdge
                && myToDistinguishMaterials == theOther.myToDistinguishMaterials
                && myToDrawEdges == theOther.myToDrawEdges
                && myToDrawSilhouette == theOther.myToDrawSilhouette
                && myToMapTexture == theOther.myToMapTexture
                && myIsTextZoomable == theOther.myIsTextZoomable;
        }

        Graphic3d_MaterialAspect myFrontMaterial = new Graphic3d_MaterialAspect();
        Graphic3d_MaterialAspect myBackMaterial = new Graphic3d_MaterialAspect();
        Aspect_InteriorStyle myInteriorStyle;

        //! Return true if texture mapping is enabled (false by default).
        public bool ToMapTexture() { return myToMapTexture; }
        bool myToSkipFirstEdge;
        bool myToDistinguishMaterials;
        bool myToDrawEdges;
        bool myToDrawSilhouette;
        bool myToMapTexture;
        bool myIsTextZoomable;
        Graphic3d_TextureSet myTextureSet;
        //! Return texture to be mapped.
        //Standard_DEPRECATED("Deprecated method, TextureSet() should be used instead")
        public Graphic3d_TextureMap TextureMap()
        {
            return myTextureSet != null && !myTextureSet.IsEmpty()
                  ? myTextureSet.First()
                  : new Graphic3d_TextureMap();
        }
        //! Return shader program.
        public Graphic3d_ShaderProgram ShaderProgram() { return myProgram; }
        //! Modifies the line type
        public void SetLineType(Aspect_TypeOfLine theType)
        {
            myLineType = theType;
            myLinePattern = DefaultLinePatternForType(theType);
        }
        //! Return stipple line pattern for line type.
        static ushort DefaultLinePatternForType(Aspect_TypeOfLine theType)
        {
            switch (theType)
            {
                case Aspect_TypeOfLine.Aspect_TOL_DASH: return 0xFFC0;
                case Aspect_TypeOfLine.Aspect_TOL_DOT: return 0xCCCC;
                case Aspect_TypeOfLine.Aspect_TOL_DOTDASH: return 0xFF18;
                case Aspect_TypeOfLine.Aspect_TOL_EMPTY: return 0x0000;
                case Aspect_TypeOfLine.Aspect_TOL_SOLID: return 0xFFFF;
                case Aspect_TypeOfLine.Aspect_TOL_USERDEFINED: return 0xFF24;
            }
            return 0xFFFF;
        }


        Aspect_TypeOfLine myLineType;

        //! Modifies the line thickness
        //! Warning: Raises Standard_OutOfRange if the width is a negative value.
        public void SetLineWidth(float theWidth)
        {
            if (theWidth <= 0.0f)
            {
                throw new Standard_OutOfRange("Bad value for EdgeLineWidth");
            }
            myLineWidth = theWidth;
        }

        //! Returns true if material properties should be distinguished for back and front faces (false by default).
        public bool Distinguish() { return myToDistinguishMaterials; }


        //! Returns the way how alpha value should be treated (Graphic3d_AlphaMode_BlendAuto by default, for backward compatibility).
        public Graphic3d_AlphaMode AlphaMode() { return myAlphaMode; }

        Graphic3d_AlphaMode myAlphaMode;

        //! Returns the surface material of external faces
        public Graphic3d_MaterialAspect FrontMaterial()
        {
            return myFrontMaterial;
        }

        //! Returns the surface material of internal faces
        public Graphic3d_MaterialAspect BackMaterial()
        {
            return myBackMaterial;
        }


        //! Returns shading model; Graphic3d_TypeOfShadingModel_DEFAULT by default.
        //! Graphic3d_TOSM_DEFAULT means that Shading Model set as default for entire Viewer will be used.
        public Graphic3d_TypeOfShadingModel ShadingModel() { return myShadingModel; }

        //! Return interior rendering style; Aspect_IS_SOLID by default.
        public Aspect_InteriorStyle InteriorStyle()
        {
            return myInteriorStyle;
        }


        //! Sets shading model
        public void SetShadingModel(Graphic3d_TypeOfShadingModel theShadingModel) { myShadingModel = theShadingModel; }


        Graphic3d_ShaderProgram myProgram;

        Graphic3d_MarkerImage myMarkerImage;
        //Graphic3d_HatchStyle     myHatchStyle;
        string myTextFont;
        // Graphic3d_MaterialAspect myFrontMaterial;
        //  Graphic3d_MaterialAspect myBackMaterial;

        protected Quantity_ColorRGBA myInteriorColor;
        protected Quantity_ColorRGBA myBackInteriorColor;
        protected Quantity_ColorRGBA myEdgeColor;

        //  Graphic3d_PolygonOffset myPolygonOffset;
        //  Aspect_InteriorStyle myInteriorStyle;
        protected Graphic3d_TypeOfShadingModel myShadingModel;
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



    }//! Definition of the color shading model.


    internal class Graphic3d_MarkerImage
    {
    }
}
