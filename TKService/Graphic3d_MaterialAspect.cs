using OCCPort.Common;
using TKernel;

namespace TKService
{
    public class Graphic3d_MaterialAspect
    {
        public Graphic3d_MaterialAspect()
        {
            myRequestedMaterialName = Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_DEFAULT;
            init(Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_DEFAULT);
        }
        public Graphic3d_MaterialAspect(Graphic3d_NameOfMaterial theName)
        {
            myRequestedMaterialName = (theName);
            init(theName);
        }
        //! Name list of standard materials (defined within enumeration).
        static RawMaterial[] THE_MATERIALS =
        {
  new   RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Brass,       "Brass"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Bronze,      "Bronze"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Copper,      "Copper"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Gold,        "Gold"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Pewter,      "Pewter"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Plastered,   "Plastered"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Plastified,  "Plastified"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Silver,      "Silver"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Steel,       "Steel"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Stone,       "Stone"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_ShinyPlastified, "Shiny_plastified"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Satin,       "Satined"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Metalized,   "Metalized"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Ionized,     "Ionized"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Chrome,      "Chrome"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Aluminum,    "Aluminium"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Obsidian,    "Obsidian"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Neon,        "Neon"),
 new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Jade,        "Jade"),
  new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Charcoal,    "Charcoal"),
  new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Water,       "Water"),
  new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Glass,       "Glass"),
  new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Diamond,     "Diamond"),
  new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Transparent, "Transparent"),
  new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_DEFAULT,     "Default"),
  new  RawMaterial (Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_UserDefined, "UserDefined")
  };
        void init(Graphic3d_NameOfMaterial theName)
        {
            RawMaterial aMat = THE_MATERIALS[(int)theName];
            //  myBSDF = aMat.BSDF;
            //  myPBRMaterial = aMat.PBRMaterial;
            //myStringName = aMat.StringName;
            myColors[(int)Graphic3d_TypeOfReflection.Graphic3d_TOR_AMBIENT] = aMat.Colors[(int)Graphic3d_TypeOfReflection.Graphic3d_TOR_AMBIENT];
            myColors[(int)Graphic3d_TypeOfReflection.Graphic3d_TOR_DIFFUSE] = aMat.Colors[(int)Graphic3d_TypeOfReflection.Graphic3d_TOR_DIFFUSE];
            myColors[(int)Graphic3d_TypeOfReflection.Graphic3d_TOR_SPECULAR] = aMat.Colors[(int)Graphic3d_TypeOfReflection.Graphic3d_TOR_SPECULAR];
            myColors[(int)Graphic3d_TypeOfReflection.Graphic3d_TOR_EMISSION] = aMat.Colors[(int)Graphic3d_TypeOfReflection.Graphic3d_TOR_EMISSION];
            myTransparencyCoef = aMat.TransparencyCoef;
            myRefractionIndex = aMat.RefractionIndex;
            myShininess = aMat.Shininess;
            myMaterialType = aMat.MaterialType;
            myMaterialName = theName;
            myRequestedMaterialName = theName;
        }
        float myTransparencyCoef;
        float myRefractionIndex;
        string myStringName;
        float myShininess;
        Graphic3d_NameOfMaterial myMaterialName;
        Graphic3d_TypeOfMaterial myMaterialType;

        Graphic3d_NameOfMaterial myRequestedMaterialName;

        //! Returns the transparency coefficient of the surface (1.0 - Alpha); 0.0 means opaque.
        public float Transparency() { return myTransparencyCoef; }


        //! Returns the alpha coefficient of the surface (1.0 - Transparency); 1.0 means opaque.
        public float Alpha()  { return 1.0f - myTransparencyCoef; }

        //! Returns TRUE if the reflection mode is active, FALSE otherwise.
        public bool ReflectionMode(Graphic3d_TypeOfReflection theType)
        {
            return !myColors[(int)theType].IsEqual(Quantity_NameOfColor.Quantity_NOC_BLACK);
        }
        const int Graphic3d_TypeOfReflection_NB = 4;
        Quantity_Color[] myColors = new Quantity_Color[Graphic3d_TypeOfReflection_NB];

    }//! Nature of the reflection of a material.


    //! Types of materials specifies if a material can change color.
    enum Graphic3d_TypeOfMaterial
    {
        Graphic3d_MATERIAL_ASPECT, //!< aspect   material definition with configurable color (like plastic)
        Graphic3d_MATERIAL_PHYSIC  //!< physical material definition with fixed color (like gold)
    };

    //! Raw material for defining list of standard materials
    struct RawMaterial
    {
        public string StringName;
        //  Graphic3d_BSDF BSDF;
        //Graphic3d_PBRMaterial PBRMaterial;
        const int Graphic3d_TypeOfReflection_NB = 4;
        public Quantity_Color[] Colors = new Quantity_Color[Graphic3d_TypeOfReflection_NB];
        public float TransparencyCoef;
        public float RefractionIndex;
        public float Shininess;
        public float AmbientCoef;  //!< coefficient for Graphic3d_MaterialAspect::SetColor()
        public float DiffuseCoef;  //!< coefficient for Graphic3d_MaterialAspect::SetColor()
        public Graphic3d_TypeOfMaterial MaterialType;
        public Graphic3d_NameOfMaterial MaterialName;

        public RawMaterial(Graphic3d_NameOfMaterial theName, string theStringName)
        {
        }


    };

}
