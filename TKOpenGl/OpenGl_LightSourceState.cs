using OCCPort.Common;
using System;
using System.Reflection.Metadata;
using TKService;

namespace OCCPort.OpenGL
{
    //! Defines state of OCCT light sources.
    public class OpenGl_LightSourceState : OpenGl_StateInterface
    {

        //! Returns number of mipmap levels used in specular IBL map.
        //! 0 by default or in case of using non-PBR shading model.
        public int SpecIBLMapLevels()  { return mySpecIBLMapLevels; }


        //! Returns TRUE if shadowmap is set.

        public bool HasShadowMaps()
        {
            return myToCastShadows && myShadowMaps != null;
        }

        //! Sets new light sources.
        public void Set(Graphic3d_LightSet theLightSources) { myLightSources = theLightSources; }

        //! Returns current list of light sources.
        public Graphic3d_LightSet LightSources()
        {
            return myLightSources;
        }

        //! Returns shadowmap.
        internal OpenGl_ShadowMapArray ShadowMaps()
        {
            return myShadowMaps;
        }

        //! Sets shadowmap.
        public void SetShadowMaps(OpenGl_ShadowMapArray theMap) { myShadowMaps = theMap; }

        //! Sets number of mipmap levels used in specular IBL map.
        internal void SetSpecIBLMapLevels(int theSpecIBLMapLevels)
        {
            mySpecIBLMapLevels = theSpecIBLMapLevels;
        }
        int mySpecIBLMapLevels; //!< Number of mipmap levels used in specular IBL map (0 by default or in case of using non-PBR shading model)

        bool myToCastShadows;    //!< enable/disable shadowmap
        OpenGl_ShadowMapArray myShadowMaps;    //!< active shadowmap

        Graphic3d_LightSet myLightSources;     //!< List of OCCT light sources

    }
}