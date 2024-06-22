using OCCPort.Tester;
using System;


namespace OCCPort
{
	public abstract class Graphic3d_CStructure
	{
		public Graphic3d_BndBox3d myBndBox;
		internal int highlight;
		public int visible;
		int myId;
		//! Return structure visibility flag
		public bool IsVisible() { return visible != 0; }

		public bool IsInfinite { get; internal set; }
		public bool IsForHighlight;
		public Graphic3d_CStructure(Graphic3d_StructureManager theManager)
		{
			myGraphicDriver = (theManager.GraphicDriver());
			myId = (-1);
			/*myZLayer(Graphic3d_ZLayerId_Default),
			myPriority(Graphic3d_DisplayPriority_Normal),
			myPreviousPriority(Graphic3d_DisplayPriority_Normal),
			myIsCulled(Standard_True),
			myBndBoxClipCheck(Standard_True),
			myHasGroupTrsf(Standard_False),*/
			//
			IsInfinite = false;
			stick = (0);
			highlight = (0);
			visible = (1);
			/*HLRValidation(0),
			IsForHighlight(Standard_False),
			IsMutable(Standard_False),
			Is2dText(Standard_False)*/


			myId = myGraphicDriver.NewIdentification();
		}

		Graphic3d_GraphicDriver myGraphicDriver;

		//! @return bounding box of this presentation

		internal Graphic3d_BndBox3d BoundingBox()
		{
			return myBndBox;
		}

		protected Graphic3d_SequenceOfGroup myGroups;
		Graphic3d_TransformPers myTrsfPers;
		internal int stick;
		public int Identification()
		{
			return myId;
		}

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

		internal void SetGroupTransformPersistence(bool v)
		{
			//throw new NotImplementedException();
		}

		internal object ZLayer()
		{
			throw new NotImplementedException();
		}

		internal bool IsAlwaysRendered()
		{
			throw new NotImplementedException();
		}

		internal void MarkAsNotCulled()
		{

		}
		//! Return structure display priority.
		public Graphic3d_DisplayPriority Priority() { return myPriority; }

		Graphic3d_DisplayPriority myPriority;
		Graphic3d_DisplayPriority myPreviousPriority;

	}
}
