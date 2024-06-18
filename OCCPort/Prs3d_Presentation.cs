using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace OCCPort.Tester
{
    public class Prs3d_Presentation
    {
		//
		//! Alias for porting code.
		//typedef Graphic3d_Structure Prs3d_Presentation;
		public class Graphic3d_Structure : Prs3d_Presentation
		{
			public Graphic3d_Structure(Graphic3d_StructureManager theManager):base(theManager)
			{
				//const Handle(Graphic3d_Structure)&theLinkPrs = Handle(Graphic3d_Structure)());
			}



			internal void Network(Graphic3d_Structure theStructure,

									Graphic3d_TypeOfConnection theType,
								   List<Graphic3d_Structure> theSet)
			{

				theSet.Add(theStructure);
				switch (theType)
				{
					case Graphic3d_TOC_DESCENDANT:
						{
							for (NCollection_IndexedMap<Graphic3d_Structure*>::Iterator anIter (theStructure->myDescendants); anIter.More(); anIter.Next())
							{
								Graphic3d_Structure.Network(anIter.Value(), theType, theSet);
							}
							break;
						}
					case Graphic3d_TOC_ANCESTOR:
						{
							for (NCollection_IndexedMap<Graphic3d_Structure*>::Iterator anIter (theStructure->myAncestors); anIter.More(); anIter.Next())
							{
								Graphic3d_Structure.Network(anIter.Value(), theType, theSet);
							}
							break;
						}
				}

			}
		}

		public Graphic3d_CStructure CStructure()
		{
			return myCStructure;
		}

		public Graphic3d_CStructure myCStructure;

		Graphic3d_StructureManager myStructureManager;

		public Prs3d_Presentation(Graphic3d_StructureManager m)
		{
			myStructureManager = m;
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
			return myCStructure != null
	   && myCStructure.visible != 0;
		}

		internal Graphic3d_TransformPers TransformPersistence()
		{
			return myCStructure.TransformPersistence();
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