using System.Linq;

namespace OCCPort
{
	public abstract class Graphic3d_GraphicDriver
	{
		public Graphic3d_GraphicDriver(Aspect_DisplayConnection theDisp)
		{
			myDisplayConnection = (theDisp);
			{
				Graphic3d_ZLayerSettings aSettings = new Graphic3d_ZLayerSettings();
				/*aSettings.SetName("DEFAULT");
				aSettings.SetImmediate(Standard_False);
				aSettings.SetRaytracable(Standard_True);
				aSettings.SetEnvironmentTexture(Standard_True);
				aSettings.SetEnableDepthTest(Standard_True);
				aSettings.SetEnableDepthWrite(Standard_True);
				aSettings.SetClearDepth(Standard_False);
				aSettings.SetPolygonOffset(Graphic3d_PolygonOffset());*/
				Graphic3d_Layer aLayer = new Graphic3d_Layer(Graphic3d_ZLayerId.Graphic3d_ZLayerId_Default, new Select3D_BVHBuilder3d());
				aLayer.SetLayerSettings(aSettings);
				myLayers.Append(aLayer);
				myLayerIds.Bind(aLayer.LayerId(), aLayer);
			}
		}

		Aspect_DisplayConnection myDisplayConnection;
		MyLayersDic myLayerIds = new MyLayersDic();

		//! Creates new view for this graphic driver.
		public abstract Graphic3d_CView CreateView(Graphic3d_StructureManager theMgr);

		public abstract Graphic3d_CStructure CreateStructure(Graphic3d_StructureManager theManager);

		internal int NewIdentification()
		{
			return myStructGenId.Next();
		}

		protected LayersCollection myLayers = new LayersCollection();
		Aspect_GenId myStructGenId = new Aspect_GenId();
	}
}