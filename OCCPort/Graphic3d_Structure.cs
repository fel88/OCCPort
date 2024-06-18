using System;

namespace OCCPort
{
    public class Graphic3d_Structure
    {
        //
        //! Alias for porting code.
        //typedef Graphic3d_Structure Prs3d_Presentation;
        //! Returns the groups sequence included in this structure.
        public Graphic3d_SequenceOfGroup Groups()
        {
            return myCStructure.Groups();
        }


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
            return myCStructure != null && myCStructure.visible != 0;
        }

        internal Graphic3d_TransformPers TransformPersistence()
        {
            return myCStructure.TransformPersistence();
        }
        public Graphic3d_CStructure CStructure()
        {
            return myCStructure;
        }

        public Graphic3d_CStructure myCStructure;

        Graphic3d_StructureManager myStructureManager;

        public Graphic3d_Structure(Graphic3d_StructureManager theManager)
        {
            myStructureManager= theManager; ;
        }

        internal Graphic3d_Group NewGroup()
        {
            return myCStructure.NewGroup(this);
        }



        internal void SetInfiniteState(bool v)
        {
            throw new NotImplementedException();
        }

        public virtual void Compute()
        {

        }
        internal void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
