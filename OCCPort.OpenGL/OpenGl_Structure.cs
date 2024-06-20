using OCCPort.Tester;
using System;
using System.Text.RegularExpressions;


namespace OCCPort.OpenGL
{
    //! Implementation of low-level graphic structure.
    public class OpenGl_Structure : Graphic3d_CStructure
    {
        private bool isClipped;
        OpenGl_Structure myInstancedStructure;

		public OpenGl_Structure(Graphic3d_StructureManager theManager):base(theManager)
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
            for (OpenGl_Structure.GroupIterator aGroupIter = new GroupIterator(myGroups); aGroupIter.More(); aGroupIter.Next())
            {
                OpenGl_Group aGroup = aGroupIter.Value();

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
            //if (!visible)
            {
                return;
            }

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

        internal bool IsCulled()
        {
            throw new NotImplementedException();
        }

        internal bool IsVisible(int aViewId)
        {
            throw new NotImplementedException();
        }

		internal Graphic3d_ZLayerId ZLayer()
        {
            throw new NotImplementedException();
        }

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


