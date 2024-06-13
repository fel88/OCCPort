using System;

namespace OCCPort
{
    public class Graphic3d_Structure
    {
        internal Graphic3d_CStructure CStructure()
        {
            return myCStructure;
        }

		public Graphic3d_CStructure myCStructure;


        //! Returns the highlight indicator for this structure.
        public bool IsHighlighted()
        {
            return myCStructure != null && myCStructure.highlight != 0;
        }

        //! Returns Standard_True if the structure <me> is infinite.
        internal bool IsInfinite()
        {
            return IsDeleted()
       || myCStructure.IsInfinite;
        }

        private bool IsDeleted()
        {
            return myCStructure == null;
        }

        internal bool IsVisible()
        {
            return myCStructure != null
       && myCStructure.visible != 0;
        }

        internal Graphic3d_TransformPers TransformPersistence()
        {
            return myCStructure.TransformPersistence();
        }
    }
}
