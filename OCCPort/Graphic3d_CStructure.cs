using OCCPort.Tester;
using System;


namespace OCCPort
{
	public abstract class Graphic3d_CStructure
    {
		public Graphic3d_BndBox3d myBndBox;
        internal int highlight;
		public int visible;

        public bool IsInfinite { get; internal set; }
        public bool IsForHighlight;

        //! @return bounding box of this presentation

        internal Graphic3d_BndBox3d BoundingBox()
        {
            return myBndBox;
        }

		protected Graphic3d_SequenceOfGroup myGroups;
        Graphic3d_TransformPers myTrsfPers;
		internal int stick;

        internal Graphic3d_TransformPers TransformPersistence()
        {
            return myTrsfPers; 
        }

        public abstract Graphic3d_Group NewGroup(Graphic3d_Structure prs3d_Presentation);

        internal Graphic3d_SequenceOfGroup Groups()
        {
            return myGroups;
        }

		internal void OnVisibilityChanged()
		{
			throw new NotImplementedException();
		}
    }
}
