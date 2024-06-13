using System;

namespace OCCPort
{
	public abstract class Graphic3d_CStructure
    {
		public Graphic3d_BndBox3d myBndBox;
        internal int highlight;
		public int visible;

        public bool IsInfinite { get; internal set; }

        //! @return bounding box of this presentation

        internal Graphic3d_BndBox3d BoundingBox()
        {
            return myBndBox;
        }
        Graphic3d_TransformPers myTrsfPers;
        internal Graphic3d_TransformPers TransformPersistence()
        {
            return myTrsfPers; 
        }
    }
}
