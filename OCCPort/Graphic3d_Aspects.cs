using System;
using System.Net.Http.Headers;

namespace OCCPort
{
    public class Graphic3d_Aspects
    {

        public Graphic3d_Aspects()
        {
            myInteriorColor = new Quantity_ColorRGBA(new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_CYAN1));
            /*myBackInteriorColor(Quantity_NOC_CYAN1),
            myEdgeColor(Quantity_NOC_WHITE),
            myInteriorStyle(Aspect_IS_SOLID),*/
            myShadingModel = Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_DEFAULT;
            //myFaceCulling= (Graphic3d_TypeOfBackfacingModel_Auto),
            //myAlphaMode(Graphic3d_AlphaMod*/
        }

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

        protected Quantity_ColorRGBA myInteriorColor;
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

        public Graphic3d_MaterialAspect FrontMaterial()
        {
            throw new NotImplementedException();
        }

        public Graphic3d_MaterialAspect BackMaterial()
        {
            throw new NotImplementedException();
        }


        //! Returns shading model; Graphic3d_TypeOfShadingModel_DEFAULT by default.
        //! Graphic3d_TOSM_DEFAULT means that Shading Model set as default for entire Viewer will be used.
        public Graphic3d_TypeOfShadingModel ShadingModel() { return myShadingModel; }


        Graphic3d_ShaderProgram myProgram;

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
}