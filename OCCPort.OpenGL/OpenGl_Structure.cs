﻿using OCCPort.Tester;
using System;
using System.Linq;
using System.Text.RegularExpressions;


namespace OCCPort.OpenGL
{
    //! Implementation of low-level graphic structure.
    public class OpenGl_Structure : Graphic3d_CStructure
    {

        OpenGl_Structure myInstancedStructure;

        public OpenGl_Structure(Graphic3d_StructureManager theManager) : base(theManager)
        {
        }

        // =======================================================================
        public void renderGeometry(OpenGl_Workspace theWorkspace,
                                         ref bool theHasClosed)
        {
            if (myInstancedStructure != null)
            {
                myInstancedStructure.renderGeometry(theWorkspace, ref theHasClosed);
            }

            bool anOldCastShadows = false;
            OpenGl_Context aCtx = theWorkspace.GetGlContext();
            //for (OpenGl_Structure.GroupIterator aGroupIter = new GroupIterator(myGroups); aGroupIter.More(); aGroupIter.Next())
            foreach (var aGroup in myGroups.OfType<OpenGl_Group>())
            {
                //OpenGl_Group aGroup = aGroupIter.Value();

                Graphic3d_TransformPers aTrsfPers = aGroup.TransformPersistence();
                if (aTrsfPers != null)
                {
                    applyPersistence(aCtx, aTrsfPers, true, anOldCastShadows);
                    aCtx.ApplyModelViewMatrix();
                }

                theHasClosed = theHasClosed || aGroup.IsClosed();
                aGroup.Render(theWorkspace);

                if (aTrsfPers != null)
                {
                    revertPersistence(aCtx, aTrsfPers, true, anOldCastShadows);
                    aCtx.ApplyModelViewMatrix();
                }
            }

        }

        private void revertPersistence(OpenGl_Context aCtx, Graphic3d_TransformPers aTrsfPers, bool v, bool anOldCastShadows)
        {
            throw new NotImplementedException();
        }

        private void applyPersistence(OpenGl_Context aCtx, Graphic3d_TransformPers aTrsfPers, bool v, bool anOldCastShadows)
        {
            throw new NotImplementedException();
        }

        public void Render(OpenGl_Workspace theWorkspace)
        {
            // Process the structure only if visible
            if (visible == 0)
                return;


            // True if structure is fully clipped
            bool isClipped = false;
            bool hasDisabled = false;
            // Render groups
            bool hasClosedPrims = false;
            if (!isClipped)
            {
                renderGeometry(theWorkspace, ref hasClosedPrims);
            }
        }

        internal OpenGl_GraphicDriver GlDriver()
        {
            throw new NotImplementedException();
        }

        internal void UpdateStateIfRaytracable(bool v)
        {
            throw new NotImplementedException();
        }

        public override Graphic3d_Group NewGroup(Graphic3d_Structure theStruct)
        {
            OpenGl_Group aGroup = new OpenGl_Group(theStruct);
            myGroups.Append(aGroup);
            return aGroup;

        }


        internal bool IsRaytracable()
        {
            if (!myGroups.IsEmpty()
              && myIsRaytracable
              && myTrsfPers == null)
            {
                return true;
            }

            return myInstancedStructure != null
               && myInstancedStructure.IsRaytracable();

        }

        bool myIsRaytracable;

        private class GroupIterator
        {
            public GroupIterator(Graphic3d_SequenceOfGroup myGroups)
            {
            }

            internal bool More()
            {
                throw new NotImplementedException();
            }

            internal object Next()
            {
                throw new NotImplementedException();
            }

            internal OpenGl_Group Value()
            {
                throw new NotImplementedException();
            }
        }

        internal class StructIterator
        {
            public StructIterator(Graphic3d_IndexedMapOfStructure aStructures)
            {
            }

            internal bool More()
            {
                throw new NotImplementedException();
            }

            internal object Next()
            {
                throw new NotImplementedException();
            }

            internal OpenGl_Structure Value()
            {
                throw new NotImplementedException();
            }
        }
    }
}


