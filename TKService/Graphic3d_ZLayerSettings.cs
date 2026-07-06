using OCCPort.Common;
using System.Reflection.Metadata;
using TKMath;

namespace TKService
{
    //! Structure defines list of ZLayer properties.
    public class Graphic3d_ZLayerSettings
    {

        public Graphic3d_ZLayerSettings()
        {
            myCullingDistance = (Precision.Infinite());
            myCullingSize = (Precision.Infinite());
            myIsImmediate = (false);
            myToRaytrace = (true);
            myUseEnvironmentTexture = (true);
            //myToEnableDepthTest = (true);
            //myToEnableDepthWrite = (true);
            myToClearDepth = (true);
            myToRenderInDepthPrepass = (true);

        }
        //! Return TRUE if layer should be rendered within depth pre-pass; TRUE by default.
        public bool ToRenderInDepthPrepass() { return myToRenderInDepthPrepass; }
        bool myToRenderInDepthPrepass;//!< option to render layer within depth pre-pass

        //! Set the flag indicating the immediate layer, which should be drawn after all normal (non-immediate) layers.
        public void SetImmediate(bool theValue) { myIsImmediate = theValue; }

        //! Return true if this layer should be drawn after all normal (non-immediate) layers.
        public bool IsImmediate() { return myIsImmediate; }

        //! Return the origin of all objects within the layer.
        public gp_XYZ Origin() { return myOrigin; }

        //! Return the transformation to the origin.
        public TopLoc_Datum3D OriginTransformation() { return myOriginTrsf; }

        //! Returns TRUE if layer should be processed by ray-tracing renderer; TRUE by default.
        //! Note that this flag is IGNORED for layers with IsImmediate() flag.
        public bool IsRaytracable() { return myToRaytrace; }


        //! Return true if depth values should be cleared before drawing the layer.
        public bool ToClearDepth() { return myToClearDepth; }

        bool myToClearDepth;          //!< option to clear depth values before drawing the layer

        bool myToRaytrace;            //!< option to render layer within ray-tracing engine
        bool myUseEnvironmentTexture; //!< flag to allow/prevent environment texture mapping usage for specific layer


        string myName;                  //!< user-provided name
        Graphic3d_LightSet myLights;                //!< lights list
        TopLoc_Datum3D myOriginTrsf;            //!< transformation to the origin
        gp_XYZ myOrigin;                //!< the origin of all objects within the layer
        double myCullingDistance;       //!< distance to discard objects
        double myCullingSize;           //!< size to discard objects
        Graphic3d_PolygonOffset myPolygonOffset;         //!< glPolygonOffset() arguments
        bool myIsImmediate;           //!< immediate layer will be drawn after all normal layers

    }
}
