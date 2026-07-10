using OCCPort.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TKernel;
using TKService;

namespace OCCPort.OpenGL
{
    //! OpenGL material definition
    public class OpenGl_Material
    {
        public OpenGl_Material()
        {
            
        }

        public static int NbOfVec4Common() { return 4 * 2; }
        public static int NbOfVec4Pbr() { return 3 * 2; }

        public OpenGl_MaterialCommon[] Common = new OpenGl_MaterialCommon[2] { new OpenGl_MaterialCommon(), new OpenGl_MaterialCommon() };
        public OpenGl_MaterialPBR[] Pbr = new OpenGl_MaterialPBR[2] { new OpenGl_MaterialPBR(), new OpenGl_MaterialPBR() };

        //! Set material color.
        public void SetColor(OpenGl_Vec3 theColor)
        {
            Common[0].SetColor(theColor);
            Common[1].SetColor(theColor);
            Pbr[0].SetColor(theColor);
            Pbr[1].SetColor(theColor);
        }


        //! Returns packed (serialized) representation of PBR material properties
        public OpenGl_Vec4[] PackedPbr() { return Pbr.SelectMany(z => z.ToArray()).ToArray(); }

        //! Returns packed (serialized) representation of common material properties
        public OpenGl_Vec4[] PackedCommon() { return Common.SelectMany(z => z.ToArray()).ToArray(); }

        internal void Init(OpenGl_Context theCtx,
            Graphic3d_MaterialAspect theFront,
            Quantity_Color theFrontColor,
            Graphic3d_MaterialAspect theBack,
            Quantity_Color theBackColor)
        {
            init(theCtx, theFront, theFrontColor, 0);
            if (theFront != theBack)
            {
                init(theCtx, theBack, theBackColor, 1);
            }
            else
            {
                Common[1] = Common[0];
                Pbr[1] = Pbr[0];
            }
        }

        void init(OpenGl_Context theCtx,
                                 Graphic3d_MaterialAspect theMat,
                                 Quantity_Color theInteriorColor,
                                 int theIndex)
        {
            OpenGl_MaterialCommon aCommon = Common[theIndex];
            OpenGl_MaterialPBR aPbr = Pbr[theIndex];
            aPbr.ChangeMetallic(theMat.PBRMaterial().Metallic());
            aPbr.ChangeRoughness(theMat.PBRMaterial().NormalizedRoughness());
            aPbr.EmissionIOR = new Graphic3d_Vec4(theMat.PBRMaterial().Emission(), theMat.PBRMaterial().IOR());


            OpenGl_Vec3 aSrcAmb = theMat.AmbientColor();
            OpenGl_Vec3 aSrcDif = theMat.DiffuseColor();
            OpenGl_Vec3 aSrcSpe = theMat.SpecularColor();
            OpenGl_Vec3 aSrcEms = theMat.EmissiveColor();
            aCommon.SpecularShininess.SetValues(aSrcSpe, 128.0f * theMat.Shininess()); // interior color is ignored for Specular
            switch (theMat.MaterialType())
            {
                case Graphic3d_TypeOfMaterial.Graphic3d_MATERIAL_ASPECT:
                    {
                        aCommon.Diffuse.SetValues(aSrcDif * theInteriorColor, theMat.Alpha());
                        aCommon.Ambient.SetValues(aSrcAmb * theInteriorColor, 1.0f);
                        aCommon.Emission.SetValues(aSrcEms * theInteriorColor, 1.0f);
                        aPbr.BaseColor.SetValues(theInteriorColor, theMat.Alpha());
                        break;
                    }
                case Graphic3d_TypeOfMaterial.Graphic3d_MATERIAL_PHYSIC:
                    {
                        aCommon.Diffuse.SetValues(aSrcDif, theMat.Alpha());
                        aCommon.Ambient.SetValues(aSrcAmb, 1.0f);
                        aCommon.Emission.SetValues(aSrcEms, 1.0f);
                        aPbr.BaseColor = theMat.PBRMaterial().Color();
                        break;
                    }
            }

            aCommon.Diffuse = theCtx.Vec4FromQuantityColor(aCommon.Diffuse);
            aCommon.Ambient = theCtx.Vec4FromQuantityColor(aCommon.Ambient);
            aCommon.SpecularShininess = theCtx.Vec4FromQuantityColor(aCommon.SpecularShininess);
            aCommon.Emission = theCtx.Vec4FromQuantityColor(aCommon.Emission);
        }
    }

}