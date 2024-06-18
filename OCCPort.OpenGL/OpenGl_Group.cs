using OCCPort.Tester;
using System;
using System.CodeDom;


namespace OCCPort.OpenGL
{
    //! Implementation of low-level graphic group.
    public class OpenGl_Group : Graphic3d_Group
    {


        //! Add primitive array element
        public override void AddPrimitiveArray(Graphic3d_TypeOfPrimitiveArray theType,
                                                   Graphic3d_IndexBuffer theIndices,
                                                   Graphic3d_Buffer theAttribs,
                                                   Graphic3d_BoundBuffer theBounds,
                                                   bool theToEvalMinMax)
        {
            if (IsDeleted()
   || theAttribs == null)
            {
                return;
            }

            OpenGl_Structure aStruct = GlStruct();
            OpenGl_GraphicDriver aDriver = aStruct.GlDriver();

            OpenGl_PrimitiveArray anArray = new OpenGl_PrimitiveArray(aDriver, theType, theIndices, theAttribs, theBounds);
            AddElement(anArray);

			base.AddPrimitiveArray(theType, theIndices, theAttribs, theBounds, theToEvalMinMax);
        }
        OpenGl_Aspects myAspects;
        OpenGl_ElementNode myFirst;
        OpenGl_ElementNode myLast;
        bool myIsRaytracable;
        public void AddElement(OpenGl_Element theElem)
        {
            OpenGl_ElementNode aNode = new OpenGl_ElementNode();

            aNode.elem = theElem;
            aNode.next = null;
            if (myLast != null)
                myLast.next = aNode;
            else
                myFirst = aNode;

            myLast = aNode;

            if (OpenGl_Raytrace.IsRaytracedElement(aNode) && !HasPersistence())
            {
                myIsRaytracable = true;

                OpenGl_Structure aStruct = GlStruct();
                if (aStruct != null)
                {
                    aStruct.UpdateStateIfRaytracable(false);
                }
            }
        }

        private bool HasPersistence()
        {
            throw new NotImplementedException();
        }
        OpenGl_Structure GlStruct()
        {
            return (OpenGl_Structure)(myStructure.CStructure());
        }


        private bool IsDeleted()
        {
            throw new NotImplementedException();
        }

		internal Graphic3d_TransformPers TransformPersistence()
		{
			throw new NotImplementedException();
		}
		public bool renderFiltered(OpenGl_Workspace theWorkspace,
									   OpenGl_Element theElement)
		{
			if (!theWorkspace.ShouldRender(theElement, this))
			{
				return false;
			}

			theElement.Render(theWorkspace);
			return true;
		}
		bool myIsClosed;

		public OpenGl_Group(Graphic3d_Structure theStruct)
		{
		}

		internal void Render(OpenGl_Workspace theWorkspace)
		{
			// Setup aspects
//			theWorkspace.SetAllowFaceCulling(myIsClosed
									//	   && !theWorkspace.GetGlContext().Clipping().IsClippingOrCappingOn());
			OpenGl_Aspects aBackAspects = theWorkspace.Aspects();
			bool isAspectSet = myAspects != null && renderFiltered(theWorkspace, myAspects);

			// Render group elements
			for (OpenGl_ElementNode aNodeIter = myFirst; aNodeIter != null; aNodeIter = (OpenGl_ElementNode)aNodeIter.next)
			{
				renderFiltered(theWorkspace, aNodeIter.elem);
			}

			// Restore aspects
			if (isAspectSet)
				theWorkspace.SetAspects(aBackAspects);

		}

        internal bool IsClosed()
        {
            throw new NotImplementedException();
        }
    }
}
