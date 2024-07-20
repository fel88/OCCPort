using System;

namespace OCCPort
{
	public class Graphic3d_ZLayerSettings
	{
		//! Return true if this layer should be drawn after all normal (non-immediate) layers.
		public bool IsImmediate() { return myIsImmediate; }

		//! Return the origin of all objects within the layer.
		public gp_XYZ Origin() { return myOrigin; }

		//! Returns TRUE if layer should be processed by ray-tracing renderer; TRUE by default.
		//! Note that this flag is IGNORED for layers with IsImmediate() flag.
		public bool IsRaytracable() { return myToRaytrace; }

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