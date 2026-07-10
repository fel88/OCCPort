using OCCPort;
using OCCPort.Common;

namespace TKService
{
    //! Class implementing Metallic-Roughness physically based material definition
    public class Graphic3d_PBRMaterial
    {
        //! Returns index of refraction in [1, 3] range.
        public float IOR() { return myIOR; }

        //! Returns light intensity emitted by material.
        //! Values are greater or equal 0.
        public Graphic3d_Vec3 Emission() { return myEmission; }

        //! Returns roughness mapping parameter in [0, 1] range.
        //! Roughness is defined in [0, 1] for handful material settings
        //! and is mapped to [MinRoughness, 1] for calculations.
        public float NormalizedRoughness() { return myRoughness; }

        //! Returns material's metallic coefficient in [0, 1] range.
        //! 1 for metals and 0 for dielectrics.
        //! It is preferable to be exactly 0 or 1. Average values are needed for textures mixing in shader.
        public float Metallic() { return myMetallic; }


        //! Returns albedo color with alpha component of material.
        public Quantity_ColorRGBA Color() { return myColor; }


        Quantity_ColorRGBA myColor = new Quantity_ColorRGBA();     //!< albedo color with alpha component [0, 1]
        float myMetallic;  //!< metallic coefficient of material [0, 1]
        float myRoughness; //!< roughness coefficient of material [0, 1]
        Graphic3d_Vec3 myEmission = new TKernel.NCollection_Vec3<float>();  //!< light intensity emitted by material [>= 0]
        float myIOR;       //!< index of refraction [1, 3]


    }
}
