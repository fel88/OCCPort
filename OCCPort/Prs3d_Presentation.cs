using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace OCCPort
{
  
    public class Prs3d_Presentation : Graphic3d_Structure
    {
        public Prs3d_Presentation(Graphic3d_StructureManager theManager) : base(theManager)
        {
            //const Handle(Graphic3d_Structure)&theLinkPrs = Handle(Graphic3d_Structure)());
        }



        //! Returns the last created group or creates new one if list is empty.
        public Graphic3d_Group CurrentGroup()
        {
            if (Groups().IsEmpty())
            {
                return NewGroup();
            }
            return Groups().Last();
        }

        
        internal void Network(Graphic3d_Structure theStructure,

                                Graphic3d_TypeOfConnection theType,
                               List<Graphic3d_Structure> theSet)
        {

            theSet.Add(theStructure);
            //switch (theType)
            //{
            //    case Graphic3d_TOC_DESCENDANT:
            //        {
            //            for (NCollection_IndexedMap<Graphic3d_Structure*>::Iterator anIter (theStructure->myDescendants); anIter.More(); anIter.Next())
            //            {
            //                Graphic3d_Structure.Network(anIter.Value(), theType, theSet);
            //            }
            //            break;
            //        }
            //    case Graphic3d_TOC_ANCESTOR:
            //        {
            //            for (NCollection_IndexedMap<Graphic3d_Structure*>::Iterator anIter (theStructure->myAncestors); anIter.More(); anIter.Next())
            //            {
            //                Graphic3d_Structure.Network(anIter.Value(), theType, theSet);
            //            }
            //            break;
            //        }
            //}

        }
    }


}