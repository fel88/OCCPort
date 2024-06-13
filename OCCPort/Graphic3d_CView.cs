using System;
using System.Collections.Generic;

namespace OCCPort
{
	public abstract class Graphic3d_CView : Graphic3d_DataStructureManager

    {
		public List<Graphic3d_MapOfStructure> Items = new List<Graphic3d_MapOfStructure>();

        internal void DisplayedStructures(out Graphic3d_MapOfStructure[] aSetOfStructures)
        {
			aSetOfStructures = Items.ToArray();
        }
		public abstract Graphic3d_Layer[] Layers();


		public abstract Aspect_Window Window();

        internal void Invalidate()
        {
            
        }

        public virtual bool IsDefined()
        {
            return true;
        }

        internal Bnd_Box MinMaxValues()
        {
            return new Bnd_Box();
        }

	}
	public class OpenGl_View : Graphic3d_CView
	{

		public OpenGl_View()
		{
			myZLayers = new OpenGl_LayerList();
		}

		//! Returns True if the window associated to the view is defined.
		public override bool IsDefined()
		{ return myWindow != null; }
		OpenGl_LayerList myZLayers; //!< main list of displayed structure, sorted by layers

		public override Graphic3d_Layer[] Layers()
		{
			return myZLayers.Layers();

		}
		OpenGl_Window myWindow;

		public override Aspect_Window Window()
		{
			return myWindow.SizeWindow();

		}

	}
	internal class OpenGl_LayerList
	{
		List<Graphic3d_Layer> myLayers = new List<Graphic3d_Layer>();
		internal Graphic3d_Layer[] Layers()
		{
			return myLayers.ToArray();
		}

	}
	public class Graphic3d_Layer
	{
		internal Bnd_Box BoundingBox(object v, object aCamera, object value1, object value2, bool theToIncludeAuxiliary)
		{
			throw new NotImplementedException();
		}

		internal void InvalidateBoundingBox()
		{
			throw new NotImplementedException();
		}

		internal int NbOfTransformPersistenceObjects()
		{
			throw new NotImplementedException();
		}

	}
	internal class OpenGl_Window
	{
		internal Aspect_Window SizeWindow()
		{
			throw new NotImplementedException();
		}

    }

}