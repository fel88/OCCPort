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
        int myId;
        protected Graphic3d_RenderingParams myRenderParams;

        public virtual bool IsDefined()
        {
            return true;
        }

        internal Bnd_Box MinMaxValues()
        {
            return new Bnd_Box();
        }

	}

}